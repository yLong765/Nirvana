#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class GraphEditor : EditorWindow
    {
        private static Event _e;
        private static Vector2 _realMousePosition;
        private static Rect _graphRect;
        
        private static float _lastUpdateTime;
        private static float? _smoothZoom;
        private static float _zoomVelocity = 1;
        public static Rect graphtRect => _graphRect;

        private static readonly float GRAPH_TOP = 21;
        private static readonly float GRAPH_LEFT = 2;
        private static readonly float GRAPH_RIGHT = 2;
        private static readonly float GRAPH_BOTTOM = 2;

        private static bool _mulSelect;
        private static Vector3 _mulSelectStartPos;
        
        private Graph _rootGraph;
        private int _rootGraphID;

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
        
        private static Vector2 graphOffset
        {
            get => currentGraph.offset;
            set
            {
                if (currentGraph != null)
                {
                    currentGraph.offset = value;
                }
            }
        }

        private static float graphZoom
        {
            get => currentGraph.zoom;
            set
            {
                if (currentGraph != null)
                {
                    currentGraph.zoom = value;
                }
            }
        }

        public static Graph currentGraph { get; private set; }
        public static GraphEditor current { get; private set; }

        private void InitData(Graph graph, BlackboardSource bbSource)
        {
            rootGraph = graph;
            if (bbSource != null) currentGraph.bbSource = bbSource;
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenAsset(int instanceID, int line) {
            var target = EditorUtility.InstanceIDToObject(instanceID) as Graph;
            if ( target != null ) {
                OpenWindow(target);
                return true;
            }
            return false;
        }

        public static GraphEditor OpenWindow(Graph data = null, BlackboardSource bbSource = null) {
            var window = GetWindow<GraphEditor>();
            window.InitData(data, bbSource);
            window.titleContent = new GUIContent("Graph Canvas");
            window.Focus();
            window.Show();
            return window;
        }
        
        private static Vector2 MousePosToGraph(Vector2 mousePos)
        {
            var offset = mousePos - graphOffset;
            offset.x -= GRAPH_LEFT;
            offset.y -= GRAPH_TOP;
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
            needsRepaint |= UpdateSmoothZoom(deltaTime);
            if ( needsRepaint ) {
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
            _graphRect = Rect.MinMaxRect(GRAPH_LEFT, GRAPH_TOP, position.width - GRAPH_RIGHT, position.height - GRAPH_BOTTOM);
            
            if (!CheckGraph()) return;

            _e = Event.current;
            _realMousePosition = _e.mousePosition;

            EditorUtils.DrawBox(_graphRect, ColorUtils.gray13, StyleUtils.normalBG);
            DrawGrid(_graphRect, graphOffset);
            NodesWindowPrevEvent();

            GUI.BeginClip(_graphRect, graphOffset / graphZoom, default, false);
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            GUI.EndClip();
            
            NodesWindowPostEvent();
            DrawToolbar(currentGraph);
            var inspectorRect = DrawInspector();
            var blackboardRect = DrawBlackboard();
            GraphUtils.allowClick = _graphRect.Contains(_realMousePosition) && !inspectorRect.Contains(_realMousePosition) &&
                                    !blackboardRect.Contains(_realMousePosition);

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

        private bool CheckGraph()
        {
            if (rootGraph == null)
            {
                // var graphCenter = _graphRect.center;
                // var size = StyleUtils.symbolText.CalcSize("Please Select One Graph Editor Data!");
                // var popup = new Rect(graphCenter.x - size.x / 2f, graphCenter.y - size.y / 2f, size.x, size.y);
                // EditorGUI.LabelField(popup, "Please Select One Graph Editor Data!", StyleUtils.symbolText);
                ShowNotification(new GUIContent("Please Select One Graph Editor Data!"));
                return false;
            }

            if (currentGraph != rootGraph)
            {
                currentGraph = rootGraph;
            }

            return true;
        }

        private static bool UpdateSmoothZoom(float deltaTime)
        {
            if (_smoothZoom == null) return false;

            var targetZoom = (float) _smoothZoom;
            if ( Mathf.Abs(targetZoom - graphZoom) < 0.00001f )
            {
                _smoothZoom = null;
                return false;
            }

            graphZoom = Mathf.SmoothDamp(graphZoom, targetZoom, ref _zoomVelocity, 0.08f, Mathf.Infinity, deltaTime);
            if ( Mathf.Abs(1 - graphZoom) < 0.00001f ) { graphZoom = 1; }
            return true;
        }

        private static void NodesWindowPrevEvent()
        {
            if (GraphUtils.allowClick && _e.type == EventType.MouseDrag && _e.button == 2)
            {
                graphOffset += _e.delta;
                _e.Use();
            }
            
            if (GraphUtils.allowClick && _e.type == EventType.ScrollWheel && _graphRect.Contains(_e.mousePosition)) {
                var zoomDelta = _e.shift ? 0.1f : 0.25f;
                var delta = -_e.delta.y > 0 ? zoomDelta : -zoomDelta;
                var center = _e.mousePosition;
                if (graphZoom == 1 && delta > 0) return;
                var pinPoint = (center - graphOffset) / graphZoom;
                var newZ = graphZoom;
                newZ += delta;
                newZ = Mathf.Clamp(newZ, 0.25f, 1f);
                _smoothZoom = newZ;
            }
        }

        private static void NodesWindowPostEvent()
        {
            if (GraphUtils.allowClick && _e.type == EventType.MouseDown)
            {
                if (_e.button == 0)
                {
                    if (_e.clickCount == 1)
                    {
                        _mulSelect = true;
                        _mulSelectStartPos = _e.mousePosition;
                        GraphUtils.ClearSelect();
                        _e.Use();
                    }
                }
                else if (_e.button == 1)
                {
                    var menu = new GenericMenuPopup("Nodes");
                    var types = TypeUtils.GetSubClassTypes(typeof(FlowNode));
                    var graphMousePosition = MousePosToGraph(_e.mousePosition);
                    foreach (var t in types)
                    {
                        menu.AddItem(t.Name, () =>
                        {
                            currentGraph.AddNode(t, graphMousePosition);
                            GraphUtils.willSetDirty = true;
                        });
                    }

                    menu.Show();
                    _e.Use();
                }
            }

            if (_mulSelect && _e.type == EventType.MouseUp)
            {
                var boxRect = RectUtils.GetBoundRect(_mulSelectStartPos, _e.mousePosition);
                var overlapNoes = currentGraph.allNodes.Where(node => boxRect.Overlaps(node.rect)).ToList();
                GraphUtils.Select(overlapNoes);
                _mulSelect = false;
            }

            if (_mulSelect)
            {
                var boxRect = RectUtils.GetBoundRect(_mulSelectStartPos, _e.mousePosition);
                var color = new Color(0.5f, 0.5f, 1f, 0.3f);
                EditorUtils.DrawBox(boxRect, color, StyleUtils.normalBG);
            }

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
        }

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

        private static void DrawNodesGUI(Graph graph)
        {
            foreach (var node in graph.allNodes)
            {
                NodeWindow.DrawNodeGUI(node);
            }
        }

        private static void DrawToolbar(Graph graph)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("File", EditorStyles.toolbarButton, GUILayout.MaxWidth(50)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Export Json"), false, () =>
                {
                    var json = currentGraph.Serialize(Formatting.Indented);
                    var selectPath = EditorUtility.SaveFilePanel("Select Export Path", "Assets", currentGraph.title, "json");
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
                        currentGraph.Deserialize(json);
                        currentGraph.offset = Vector2.zero;
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                });
                menu.ShowAsContext();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(graph.title, StyleUtils.graphTitle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Blackboard", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
            {
                GraphUtils.showBlackboardPanel = !GraphUtils.showBlackboardPanel;
            }

            GUILayout.EndHorizontal();
        }

        private static float _nodeInspectorHeight = 200f;
        
        private static Rect DrawInspector()
        {
            var rect = default(Rect);
            if (GraphUtils.activeNodes.Count == 0 && GraphUtils.activeLink == null) return rect;

            var nodeInspectorWidth = 300f;
            rect.x = _graphRect.xMin;
            rect.y = _graphRect.y;
            rect.width = nodeInspectorWidth;
            rect.height = _nodeInspectorHeight;
            var areaRect = Rect.MinMaxRect(2, 2, rect.width - 2, rect.height);
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
        
        private static Rect DrawBlackboard()
        {
            var rect = default(Rect);
            if (currentGraph.bbSource == null) return rect;
            if (!GraphUtils.showBlackboardPanel) return rect;

            var blackboardWidth = 300f;
            rect.x = _graphRect.xMax - blackboardWidth;
            rect.y = _graphRect.y;
            rect.width = blackboardWidth;
            rect.height = _blackboardHeight;
            var areaRect = Rect.MinMaxRect(0, 2, rect.width - 2, rect.height);
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
        
        private static void DrawLogger()
        {
            var rect = default(Rect);
            var loggerWidth = 250f;
            rect.x = _graphRect.xMin;
            rect.y = _graphRect.yMax - _loggerHeight;
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
#endif