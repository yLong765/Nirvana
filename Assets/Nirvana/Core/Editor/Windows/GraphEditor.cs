using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class GraphEditor : EditorWindow
    {
        private static Event _e;
        private static Vector2 _realMousePosition;
        private static Rect _canvasRect;

        private static float _lastUpdateTime;
        private static Vector2? _smoothOffset;
        private static Vector2 _offsetVelocity = Vector2.one;
        private static float? _smoothZoom;
        private static float _zoomVelocity = 1;
        public static Rect graphtRect => _canvasRect;

        private const float GRAPH_TOP = 21;
        private const float GRAPH_LEFT = 2;
        private const float GRAPH_RIGHT = 2;
        private const float GRAPH_BOTTOM = 2;
        private const float ZOOM_MIN = 0.25f;
        private const float ZOOM_MAX = 1.0f;

        private static bool _mulSelect;
        private static Vector3 _mulSelectStartPos;

        private Graph _rootGraph;
        private int _rootGraphID;

        /// <summary>
        /// 根Graph
        /// </summary>
        public Graph rootGraph
        {
            get
            {
                current ??= OpenWindow();
                current._rootGraph ??= EditorUtility.InstanceIDToObject(_rootGraphID) as Graph;
                return current._rootGraph;
            }
            set
            {
                current._rootGraph = value;
                current._rootGraphID = value != null ? value.GetInstanceID() : 0;
            }
        }

        /// <summary>
        /// 偏移值
        /// </summary>
        private static Vector2 graphOffset
        {
            get => currentGraph.offset;
            set
            {
                if (currentGraph != null)
                {
                    var t = value;
                    if (_smoothOffset == null)
                    {
                        t.x = Mathf.Round(t.x);
                        t.y = Mathf.Round(t.y);
                    }

                    currentGraph.offset = t;
                }
            }
        }

        /// <summary>
        /// 缩放值
        /// </summary>
        private static float graphZoom
        {
            get => currentGraph != null ? Mathf.Clamp(currentGraph.zoom, ZOOM_MIN, ZOOM_MAX) : ZOOM_MAX;
            set
            {
                if (currentGraph != null)
                {
                    currentGraph.zoom = Mathf.Clamp(value, ZOOM_MIN, ZOOM_MAX);
                }
            }
        }

        /// <summary>
        /// 窗口宽度
        /// </summary>
        private static float screenWidth => Screen.width / EditorGUIUtility.pixelsPerPoint;

        /// <summary>
        /// 窗口高度
        /// </summary>
        private static float screenHeight => Screen.height / EditorGUIUtility.pixelsPerPoint;

        /// <summary>
        /// 当前Graph的引用
        /// </summary>
        public static Graph currentGraph { get; private set; }
        /// <summary>
        /// 当前GraphEditor的引用
        /// </summary>
        public static GraphEditor current { get; private set; }

        private void InitData(Graph graph, BlackboardSource bbSource)
        {
            rootGraph = graph;
            if (bbSource != null) currentGraph.bbSource = bbSource;
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenAsset(int instanceID, int line)
        {
            var target = EditorUtility.InstanceIDToObject(instanceID) as Graph;
            if (target != null)
            {
                OpenWindow(target);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 打开Canvas窗口
        /// </summary>
        public static GraphEditor OpenWindow(Graph data = null, BlackboardSource bbSource = null)
        {
            var window = GetWindow<GraphEditor>();
            window.InitData(data, bbSource);
            if (data != null)
            {
                window.titleContent = new GUIContent($"{data.GetType().Name} Canvas", StyleUtils.flowIconTexture);
            }

            window.Focus();
            window.Show();
            return window;
        }

        private static Vector2 GUIViewToCanvas(Vector2 mousePos)
        {
            var offset = (mousePos - graphOffset) / graphZoom;
            offset.x -= GRAPH_LEFT;
            offset.y -= GRAPH_TOP;
            return offset;
        }
        
        private static Vector2 CanvasToGUIView(Vector2 mousePos)
        {
            var offset = mousePos;
            offset.x += GRAPH_LEFT;
            offset.y += GRAPH_TOP;
            offset = offset * graphZoom + graphOffset;
            return offset;
        }

        private void OnEnable()
        {
            current = this;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void Update()
        {
            var currentTime = Time.realtimeSinceStartup;
            var deltaTime = currentTime - _lastUpdateTime;
            _lastUpdateTime = currentTime;

            var needsRepaint = false;
            needsRepaint |= UpdateSmoothOffset(deltaTime);
            needsRepaint |= UpdateSmoothZoom(deltaTime);
            if (needsRepaint)
            {
                Repaint();
            }
        }

        private void OnInspectorUpdate()
        {
            if (!GraphUtils.willRepaint)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            _canvasRect = Rect.MinMaxRect(GRAPH_LEFT, GRAPH_TOP, position.width - GRAPH_RIGHT, position.height - GRAPH_BOTTOM);

            if (!CheckGraph()) return;

            _e = Event.current;
            _realMousePosition = _e.mousePosition;

            if (mouseOverWindow == current && (_e.isMouse || _e.isKey))
            {
                GraphUtils.willRepaint = true;
            }

            EditorUtils.DrawBox(_canvasRect, ColorUtils.gray13, StyleUtils.normalBG);
            DrawGrid(_canvasRect, graphOffset);
            NodesWindowPrevEvent();

            var originalGraphRect = _canvasRect;
            var originalMatrix = default(Matrix4x4);
            if (graphZoom != 1)
            {
                _canvasRect = BeginZoomArea(_canvasRect, graphZoom, out originalMatrix);
            }

            GUI.BeginClip(_canvasRect, graphOffset / graphZoom, default, false);
            {
                BeginWindows();
                DrawNodesGUI(currentGraph);
                EndWindows();
                DrawGraphSelection();
            }
            GUI.EndClip();

            if (graphZoom != 1 && originalMatrix != default)
            {
                EndZoomArea(originalMatrix);
                _canvasRect = originalGraphRect;
            }

            NodesWindowPostEvent();
            DrawToolbar(currentGraph);
            DrawPanels();
            DrawLogger();

            if (GraphUtils.willSetDirty)
            {
                GraphUtils.willSetDirty = false;
                if (rootGraph != null) EditorUtility.SetDirty(rootGraph);
            }

            if (GraphUtils.willRepaint)
            {
                Repaint();
            }

            if (_e.type == EventType.Repaint)
            {
                GraphUtils.willRepaint = false;
            }
        }

        /// <summary>
        /// 检查Graph是否更替
        /// </summary>
        private bool CheckGraph()
        {
            if (rootGraph == null)
            {
                ShowNotification(new GUIContent("Please Select One Graph Editor Data!"));
                return false;
            }

            if (currentGraph != rootGraph)
            {
                currentGraph = rootGraph;
            }

            return true;
        }

        /// <summary>
        /// 缩放区域开始
        /// </summary>
        static Rect BeginZoomArea(Rect container, float zoomFactor, out Matrix4x4 oldMatrix)
        {
            GUI.EndClip();
            container.y += GRAPH_TOP;
            container.width *= 1 / zoomFactor;
            container.height *= 1 / zoomFactor;
            oldMatrix = GUI.matrix;
            var matrix1 = Matrix4x4.TRS(new Vector2(container.x, container.y), Quaternion.identity, Vector3.one);
            var matrix2 = Matrix4x4.Scale(new Vector3(zoomFactor, zoomFactor, 1f));
            GUI.matrix = matrix1 * matrix2 * matrix1.inverse * GUI.matrix;
            return container;
        }

        /// <summary>
        /// 缩放区域结束，还原GUI.matrix
        /// </summary>
        static void EndZoomArea(Matrix4x4 oldMatrix)
        {
            GUI.matrix = oldMatrix;
            var recover = new Rect(0, GRAPH_TOP, screenWidth, screenHeight);
            GUI.BeginClip(recover);
        }

        /// <summary>
        /// 拖拽平滑过度
        /// </summary>
        bool UpdateSmoothOffset(float deltaTime)
        {
            if (_smoothOffset == null) return false;

            var targetOffset = (Vector2) _smoothOffset;
            if ((targetOffset - graphOffset).magnitude < 0.1f)
            {
                _smoothOffset = null;
                return false;
            }

            targetOffset = new Vector2(Mathf.FloorToInt(targetOffset.x), Mathf.FloorToInt(targetOffset.y));
            graphOffset = Vector2.SmoothDamp(graphOffset, targetOffset, ref _offsetVelocity, 0.05f, Mathf.Infinity, deltaTime);
            return true;
        }

        /// <summary>
        /// 缩放平滑过度
        /// </summary>
        private static bool UpdateSmoothZoom(float deltaTime)
        {
            if (_smoothZoom == null) return false;

            var targetZoom = (float) _smoothZoom;
            if (Mathf.Abs(targetZoom - graphZoom) < 0.00001f)
            {
                _smoothZoom = null;
                return false;
            }

            graphZoom = Mathf.SmoothDamp(graphZoom, targetZoom, ref _zoomVelocity, 0.05f, Mathf.Infinity, deltaTime);
            if (Mathf.Abs(1 - graphZoom) < 0.00001f) graphZoom = 1;

            return true;
        }

        /// <summary>
        /// Node绘制前事件处理
        /// </summary>
        private static void NodesWindowPrevEvent()
        {
            if (GraphUtils.allowClick)
            {
                if (_canvasRect.Contains(_e.mousePosition) && _e.type == EventType.MouseDrag && _e.button == 2)
                {
                    graphOffset += _e.delta;
                    _smoothOffset = null;
                    _smoothZoom = null;
                    _e.Use();
                }

                if (_e.type == EventType.ScrollWheel)
                {
                    var zoomDelta = _e.shift ? 0.1f : 0.25f;
                    var delta = -_e.delta.y > 0 ? zoomDelta : -zoomDelta;
                    if (graphZoom == 1 && delta > 0) return;
                    var offsetPoint = (_e.mousePosition - graphOffset) / graphZoom;
                    var newZ = graphZoom;
                    newZ += delta;
                    newZ = Mathf.Clamp(newZ, 0.25f, 1f);
                    _smoothZoom = newZ;

                    var a = offsetPoint * newZ + graphOffset;
                    var b = _e.mousePosition;
                    var diff = b - a;
                    _smoothOffset = graphOffset + diff;
                    
                    _e.Use();
                }
            }
        }

        /// <summary>
        /// Node绘制后事件处理
        /// </summary>
        private static void NodesWindowPostEvent()
        {
            if (GUIUtility.keyboardControl == 0)
            {
                if (_e.type == EventType.ValidateCommand)
                {
                    if (_e.commandName == "Copy" || _e.commandName == "Cut" || _e.commandName == "Paste" || _e.commandName == "SoftDelete" ||
                        _e.commandName == "Delete" || _e.commandName == "Duplicate")
                    {
                        _e.Use();
                    }
                }

                if (_e.type == EventType.ExecuteCommand)
                {
                    if (_e.commandName == "SoftDelete" || _e.commandName == "Delete")
                    {
                        foreach (var node in GraphUtils.activeNodes.ToArray())
                        {
                            currentGraph.RemoveNode(node);
                        }

                        if (GraphUtils.activeLink != null)
                        {
                            currentGraph.RemoveLink(GraphUtils.activeLink);
                        }

                        GraphUtils.willSetDirty = true;
                        GraphUtils.ClearSelect();
                    }

                    _e.Use();
                }
            }
            
            if (GraphUtils.allowClick)
            {
                if (_e.type == EventType.ContextClick)
                {
                    var graphMousePosition = GUIViewToCanvas(_e.mousePosition);
                    EditorUtils.ShowChildTypeGenericMenu(currentGraph.baseNodeType, type =>
                    {
                        currentGraph.AddNode(type, graphMousePosition);
                        GraphUtils.willSetDirty = true;
                    });
                    
                    _e.Use();
                }
            }
        }

        /// <summary>
        /// 绘制多选框
        /// </summary>
        private static void DrawGraphSelection()
        {
            if (GraphUtils.allowClick && _canvasRect.Contains(CanvasToGUIView(_e.mousePosition)) && _e.type == EventType.MouseDown && _e.button == 0)
            {
                if (_e.clickCount == 1)
                {
                    _mulSelect = true;
                    _mulSelectStartPos = _e.mousePosition;
                    GraphUtils.ClearSelect();
                    _e.Use();
                }
            }

            if (_mulSelect && _e.type == EventType.MouseUp)
            {
                var boxRect = RectUtils.GetBoundRect(_mulSelectStartPos, _e.mousePosition);
                var overlapNoes = currentGraph.allNodes.Where(node => boxRect.Overlaps(node.rect)).ToList();
                GraphUtils.Select(overlapNoes);
                _mulSelect = false;
                _e.Use();
            }

            if (_mulSelect)
            {
                var boxRect = RectUtils.GetBoundRect(_mulSelectStartPos, _e.mousePosition);
                if (boxRect.width > 5 && boxRect.height > 5)
                {
                    var color = new Color(0.5f, 0.5f, 1f, 0.5f);
                    EditorUtils.DrawBox(boxRect, color, StyleUtils.normalBG);
                    foreach (var node in currentGraph.allNodes.Where(node => boxRect.Overlaps(node.rect)))
                    {
                        EditorUtils.DrawBox(node.rect, color, StyleUtils.windowHeightLine);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制背景格
        /// </summary>
        private static void DrawGrid(Rect rect, Vector2 offset)
        {
            float step = 20f;

            Handles.color = new Color(0f, 0f, 0f, 0.3f);

            var xMin = rect.xMin + offset.x % step;
            var xMax = rect.xMax;
            for (float x = xMin; x < xMax; x += step)
            {
                if (x > rect.xMin)
                {
                    Handles.DrawLine(new Vector3(x, rect.yMin), new Vector3(x, rect.yMax));
                }
            }

            var yMin = rect.yMin + offset.y % step;
            var yMax = rect.yMax;
            for (float y = yMin; y < yMax; y += step)
            {
                if (y > rect.yMin)
                {
                    Handles.DrawLine(new Vector3(rect.xMin, y), new Vector3(rect.xMax, y));
                }
            }

            Handles.color = Color.white;
        }

        /// <summary>
        /// 绘制所有的Node
        /// </summary>
        private static void DrawNodesGUI(Graph graph)
        {
            foreach (var node in graph.allNodes)
            {
                NodeWindow.DrawNodeGUI(node);
            }
        }

        /// <summary>
        /// 绘制Toolbar
        /// </summary>
        private static void DrawToolbar(Graph graph)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("File", EditorStyles.toolbarButton, GUILayout.MaxWidth(50)))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Export Json"), false, () =>
                {
                    var json = graph.Serialize(Formatting.Indented);
                    var selectPath = EditorUtility.SaveFilePanel("Select Export Path", "Assets", graph.name, "json");
                    if (!string.IsNullOrEmpty(selectPath))
                    {
                        FileUtils.Write(selectPath, json);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                });
                menu.AddItem(new GUIContent("Import Json"), false, () =>
                {
                    var selectPath = EditorUtility.OpenFilePanel("Select Import Json File", "Assets", "json");
                    if (!string.IsNullOrEmpty(selectPath))
                    {
                        var json = FileUtils.Read(selectPath);
                        graph.Deserialize(json);
                        graph.offset = Vector2.zero;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                });
                menu.ShowAsContext();
            }

            GUILayout.FlexibleSpace();

            GUI.color = ColorUtils.darkOrang1;
            if (GUILayout.Button($"{graph.name} @ Nirvana Core", EditorStyles.toolbarButton))
            {
                Selection.activeObject = graph;
            }
            GUI.color = Color.white;
            
            if (GUILayout.Button("Blackboard", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
            {
                Prefs.showBlackboardPanel = !Prefs.showBlackboardPanel;
            }

            GUILayout.EndHorizontal();
        }

        private static void DrawPanels()
        {
            var inspectorRect = DrawNodeInspector();
            var blackboardRect = DrawBlackboard();
            GraphUtils.allowClick = !inspectorRect.Contains(_realMousePosition) && !blackboardRect.Contains(_realMousePosition);

        }

        private static float _nodeInspectorHeight = 200f;
        private static bool _isResizingNodeInspectorPanel = false;

        /// <summary>
        /// 绘制Node编辑面版
        /// </summary>
        private static Rect DrawNodeInspector()
        {
            var rect = default(Rect);
            if (GraphUtils.activeNodes.Count == 0 && GraphUtils.activeLink == null) return rect;

            var nodeInspectorWidth = Prefs.nodeInspectorPanelWidth;
            rect.x = _canvasRect.xMin;
            rect.y = _canvasRect.y;
            rect.width = nodeInspectorWidth;
            rect.height = _nodeInspectorHeight;
            var areaRect = Rect.MinMaxRect(2, 2, rect.width - 2, rect.height);
            
            // 拖拽面版大小
            var resizeRect = Rect.MinMaxRect(rect.xMax - 4, rect.yMin + 2, rect.xMax, rect.yMax);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
            if (_e.type == EventType.MouseDown && resizeRect.Contains(_e.mousePosition))
            {
                _isResizingNodeInspectorPanel = true;
                _e.Use();
            }

            if (_isResizingNodeInspectorPanel && _e.type == EventType.Layout) Prefs.nodeInspectorPanelWidth += _e.delta.x;
            if (_e.rawType == EventType.MouseUp) _isResizingNodeInspectorPanel = false;
            
            GUI.BeginClip(rect);
            GUILayout.BeginArea(areaRect);

            if (GraphUtils.activeNodes.Count == 1) NodeInspector.DrawGUI(areaRect, GraphUtils.activeNodes[0]);
            else NodeInspector.DrawGUI(areaRect, GraphUtils.activeNodes);
            if (GraphUtils.activeLink != null) LinkInspector.DrawInspector(areaRect, GraphUtils.activeLink);

            if (_e.type == EventType.Repaint)
            {
                _nodeInspectorHeight = GUILayoutUtility.GetLastRect().yMax + 32;
            }

            GUILayout.EndArea();
            GUILayout.EndArea();
            GUI.EndClip();

            return rect;
        }

        private static float _blackboardHeight = 50f;
        private static bool _isResizingBlackboardPanel = false;

        /// <summary>
        /// 绘制Blackboard面版
        /// </summary>
        private static Rect DrawBlackboard()
        {
            var rect = default(Rect);
            if (currentGraph.bbSource == null) return rect;
            if (!Prefs.showBlackboardPanel) return rect;

            var blackboardWidth = Prefs.blackboardWidth;
            rect.x = _canvasRect.xMax - blackboardWidth;
            rect.y = _canvasRect.y;
            rect.width = blackboardWidth;
            rect.height = _blackboardHeight;
            var areaRect = Rect.MinMaxRect(0, 2, rect.width - 2, rect.height);
            
            // 拖拽面版大小
            var resizeRect = Rect.MinMaxRect(rect.xMin, rect.yMin + 2, rect.xMin + 4, rect.yMax);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
            if (_e.type == EventType.MouseDown && resizeRect.Contains(_e.mousePosition))
            {
                _isResizingBlackboardPanel = true;
                _e.Use();
            }

            if (_isResizingBlackboardPanel && _e.type == EventType.Layout) Prefs.blackboardWidth -= _e.delta.x;
            if (_e.rawType == EventType.MouseUp) _isResizingBlackboardPanel = false;
            
            GUI.BeginClip(rect);
            GUILayout.BeginArea(areaRect);

            BlackboardInspector.DrawGUI(areaRect, currentGraph.bbSource, currentGraph);

            if (_e.type == EventType.Repaint)
            {
                _blackboardHeight = GUILayoutUtility.GetLastRect().yMax + 30;
            }

            GUILayout.EndArea();
            GUILayout.EndArea();
            GUI.EndClip();

            return rect;
        }

        private static float _loggerHeight = 0;

        /// <summary>
        /// 绘制Log
        /// </summary>
        private static void DrawLogger()
        {
            var rect = default(Rect);
            var loggerWidth = 250f;
            rect.x = _canvasRect.xMin;
            rect.y = _canvasRect.yMax - _loggerHeight;
            rect.width = loggerWidth;
            rect.height = _loggerHeight;
            var areaRect = Rect.MinMaxRect(2, 0, rect.width, rect.height);
            GUI.BeginClip(rect);
            GUILayout.BeginArea(areaRect);
            LogUtils.CheckAllLog();
            var heightCount = 0.0f;
            foreach (var log in LogUtils.allLogs)
            {
                var height = Mathf.Max(35f, StyleUtils.loggerBox.CalcHeight(log.value, 165f));
                EditorUtils.DrawBox(new Rect(0, heightCount, loggerWidth, height), ColorUtils.gray21, StyleUtils.normalBG);
                heightCount += height + 2f;

                var iconName = log.type switch
                {
                    LogType.Normal => "console.infoicon",
                    LogType.Warning => "console.warnicon",
                    _ => "console.erroricon"
                };

                GUILayout.BeginHorizontal();
                GUILayout.Label(EditorGUIUtility.IconContent(iconName), GUILayout.MaxWidth(35));
                GUILayout.Label(log.value, StyleUtils.loggerBox);
                GUILayout.EndHorizontal();
                GUILayout.Space(2f);
            }

            if (_e.type == EventType.Repaint)
            {
                _loggerHeight = heightCount;
            }

            GUILayout.EndArea();
            GUI.EndClip();
        }

        private static void UndoRedoPerformed()
        {
            currentGraph.UpdateNodeIDs();
        }
    }
}