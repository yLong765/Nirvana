using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class GraphEditor : EditorWindow
    {
        private static Event _e;
        private static Vector2 _realMousePosition;
        private static Vector2 _graphMousePosition;
        private static Rect _graphRect;

        private static readonly float GRAPH_TOP = 21;
        private static readonly float GRAPH_LEFT = 200;
        private static readonly float GRAPH_RIGHT = 2;
        private static readonly float GRAPH_BOTTOM = 2;
        
        private GraphEditorData _data;
        private int _dataID;

        public GraphEditorData data
        {
            get
            {
                current ??= OpenWindow();
                current._data ??= EditorUtility.InstanceIDToObject(_dataID) as GraphEditorData;
                return current._data;
            }
            set
            {
                current._data = value;
                current._dataID = value != null ? value.GetInstanceID() : 0;
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

        public static Graph currentGraph { get; private set; }
        public static GraphEditor current { get; private set; }

        private void InitData(GraphEditorData data, Blackboard blackboard)
        {
            this.data = data;
            currentGraph = data.graph;
            currentGraph.blackboard = blackboard ?? new Blackboard();
        }
        
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenAsset(int instanceID, int line) {
            var target = EditorUtility.InstanceIDToObject(instanceID) as GraphEditorData;
            if ( target != null ) {
                OpenWindow(target);
                return true;
            }
            return false;
        }

        public static GraphEditor OpenWindow(GraphEditorData data = null, Blackboard blackboard = null) {
            var window = GetWindow<GraphEditor>();
            window.InitData(data, blackboard);
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

            if (GraphUtils.isInspectorPanel)
            {
                NodeInspector.OpenWindow();
            }
        }

        private void OnGUI()
        {
            CheckGraph();
            
            _graphRect = Rect.MinMaxRect(GRAPH_LEFT, GRAPH_TOP, position.width - GRAPH_RIGHT, position.height - GRAPH_BOTTOM);

            _e = Event.current;
            _realMousePosition = _e.mousePosition;
            _graphMousePosition = MousePosToGraph(_realMousePosition);

            EditorUtils.DrawBox(_graphRect, ColorUtils.gray13, Styles.normalBG);
            DrawGrid(_graphRect, graphOffset);
            NodesWindowPrevEvent();

            GUI.BeginClip(_graphRect, graphOffset, default, false);
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            GUI.EndClip();
            
            NodesWindowPostEvent();
            DrawToolbar(currentGraph);
            var inspectorRect = Rect.MinMaxRect(0, GRAPH_TOP, GRAPH_LEFT, position.height);
            DrawInspector(inspectorRect);
            var blackboardRect = DrawBlackboard();
            GraphUtils.allowClick = _graphRect.Contains(_realMousePosition) && !inspectorRect.Contains(_realMousePosition) &&
                                    !blackboardRect.Contains(_realMousePosition);
        }

        private void CheckGraph()
        {
            
            if (currentGraph != data.graph)
            {
                currentGraph = data.graph;
            }
        }

        private static void NodesWindowPrevEvent()
        {
            if (GraphUtils.allowClick && _e.type == EventType.MouseDrag && _e.button == 2)
            {
                graphOffset += _e.delta;
                _e.Use();
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
                        GraphUtils.activeNodes = null;
                        _e.Use();
                    }
                }
                else if (_e.button == 1)
                {
                    var menu = new GenericMenuPopup("Fields");
                    var types = TypeUtils.GetChildTypes(typeof(object));
                    foreach (var t in types)
                    {
                        menu.AddItem(t.Name, () => { currentGraph.AddNode(t, _graphMousePosition); });
                    }

                    menu.Show();
                    _e.Use();
                }
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

                        GraphUtils.activeNodes = null;
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
            var allNodes = graph.allNodes;
            foreach (var node in allNodes)
            {
                NodeEditor.DrawNodeGUI(node);
            }
        }

        private static void DrawToolbar(Graph graph)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("File", EditorStyles.toolbarButton, GUILayout.MaxWidth(50)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Export Json"), false, () => { Debug.Log("Export Json"); });
                menu.ShowAsContext();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(graph.name, Styles.graphTitle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Blackboard", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
            {
                GraphUtils.showBlackboardPanel = !GraphUtils.showBlackboardPanel;
            }

            GUILayout.EndHorizontal();
        }

        private static void DrawInspector(Rect rect)
        {
            var clipRect = new Rect(0, GRAPH_TOP, rect.width, rect.height);
            var areaRect = Rect.MinMaxRect(0, 2, rect.width, rect.height - GRAPH_BOTTOM - 2);
            GUI.BeginClip(clipRect);
            GUILayout.BeginArea(areaRect);
            NodeInspector.DrawGUI(rect, GraphUtils.activeNodes.Count == 1 ? GraphUtils.activeNodes[0] : null);
            GUILayout.EndArea();
            GUI.EndClip();
        }

        private static float blackboardHeight = 50f;
        
        private static Rect DrawBlackboard()
        {
            var rect = default(Rect);
            if (currentGraph.blackboard == null) return rect;
            if (!GraphUtils.showBlackboardPanel) return rect;

            var blackboardWidth = 200f;
            rect.x = _graphRect.xMax - blackboardWidth;
            rect.y = _graphRect.y;
            rect.width = blackboardWidth;
            rect.height = blackboardHeight;
            var areaRect = Rect.MinMaxRect(0, 2, rect.width - 2, rect.height);
            GUI.BeginClip(rect);
            GUILayout.BeginArea(areaRect);
            EditorUtils.DrawBox(new Rect(0, 0, areaRect.width, areaRect.height), ColorUtils.gray21, Styles.normalBG);
            var titleHeight = Styles.CalcSize(Styles.panelTitle, "Blackboard").y;
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, Styles.normalBG);
            GUILayout.Label("Blackboard", Styles.panelTitle);
            GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 2, areaRect.xMax - 2, areaRect.yMax - 2));
            if (GUILayout.Button("Add Variable"))
            {
                var menu = new GenericMenuPopup("Fields");
                var types = TypeUtils.GetChildTypes(typeof(object));
                foreach (var t in types)
                {
                    menu.AddItem(t.Name, () => { currentGraph.AddVariable(t, t.Name); });
                }

                menu.Show();
            }

            var variables = currentGraph.blackboard.variables;
            foreach (var pair in variables)
            {
                GUILayout.BeginHorizontal();
                GUILayout.TextField(pair.Key);
                GUILayout.TextField(pair.Value.ToString());
                GUILayout.EndHorizontal();
            }
            
            if (_e.type == EventType.Repaint) {
                blackboardHeight = GUILayoutUtility.GetLastRect().yMax + 30;
            }
            else
            {
                
                //blackboardHeight = 50f;
            }
            
            GUILayout.EndArea();
            GUILayout.EndArea();
            GUI.EndClip();
            return rect;
        }
    }
}