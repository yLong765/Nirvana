using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class GraphEditor : EditorWindow
    {
        [MenuItem("Nirvana Tools/Graph")]
        private static void Open()
        {
            var window = GetWindow<GraphEditor>();
            window.titleContent = new GUIContent("Graph Canvas");
            window.Focus();
            window.Show();
        }

        private static Event e;
        private static Vector2 realMousePosition;
        private static Vector2 graphMousePosition;
        private static Rect graphRect;

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

        private static float GRAPH_TOP = 21;
        private static float GRAPH_LEFT = 200;
        private static float GRAPH_RIGHT = 2;
        private static float GRAPH_BOTTOM = 2;

        public static Graph currentGraph { get; private set; }

        private static Vector2 MousePosToGraph(Vector2 mousePos)
        {
            var offset = mousePos - graphOffset;
            offset.x -= GRAPH_LEFT;
            offset.y -= GRAPH_TOP;
            return offset;
        }

        private void OnEnable()
        {
            
            currentGraph ??= new Graph();
            currentGraph.name = "Graph Canvas";
            currentGraph.blackboard = new Blackboard();
            
        }

        private void OnGUI()
        {
            graphRect = Rect.MinMaxRect(GRAPH_LEFT, GRAPH_TOP, position.width - GRAPH_RIGHT, position.height - GRAPH_BOTTOM);

            e = Event.current;
            realMousePosition = e.mousePosition;
            graphMousePosition = MousePosToGraph(realMousePosition);

            EditorUtils.DrawBox(graphRect, ColorUtils.gray13, Styles.normalBG);
            DrawGrid(graphRect, graphOffset);
            NodesWindowPrevEvent();

            GUI.BeginClip(graphRect, graphOffset, default, false);
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            GUI.EndClip();

            NodesWindowPostEvent();
            DrawToolbar(currentGraph);
            var inspectorRect = Rect.MinMaxRect(0, GRAPH_TOP, GRAPH_LEFT, position.height);
            DrawInspector(inspectorRect);
            var blackboardRect = DrawBlackboard();
            GraphUtils.allowClick = graphRect.Contains(realMousePosition) && !inspectorRect.Contains(realMousePosition) &&
                                    !blackboardRect.Contains(realMousePosition);
        }

        private static void NodesWindowPrevEvent()
        {
            if (GraphUtils.allowClick && e.type == EventType.MouseDrag && e.button == 2)
            {
                graphOffset += e.delta;
                e.Use();
            }
        }

        private static void NodesWindowPostEvent()
        {
            if (GraphUtils.allowClick && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if (e.clickCount == 1)
                    {
                        GraphUtils.activeNodes = null;
                        e.Use();
                    }
                }
                else if (e.button == 1)
                {
                    var menu = new GenericMenuPopup("Fields");
                    var types = TypeUtils.GetChildTypes(typeof(object));
                    foreach (var t in types)
                    {
                        menu.AddItem(t.Name, () => { currentGraph.AddNode(t, graphMousePosition); });
                    }

                    menu.Show();
                    e.Use();
                }
            }

            if (GUIUtility.keyboardControl == 0)
            {
                if (e.type == EventType.ValidateCommand)
                {
                    if (e.commandName == "Copy" || e.commandName == "Cut" || e.commandName == "Paste" || e.commandName == "SoftDelete" ||
                        e.commandName == "Delete" || e.commandName == "Duplicate")
                    {
                        e.Use();
                    }
                }

                if (e.type == EventType.ExecuteCommand)
                {
                    if (e.commandName == "SoftDelete" || e.commandName == "Delete")
                    {
                        foreach (var node in GraphUtils.activeNodes.ToArray())
                        {
                            currentGraph.RemoveNode(node);
                        }

                        GraphUtils.activeNodes = null;
                    }

                    e.Use();
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

        private static Rect DrawBlackboard()
        {
            var rect = default(Rect);
            if (currentGraph.blackboard == null) return rect;
            if (!GraphUtils.showBlackboardPanel) return rect;

            var blackboardWidth = 200f;
            var blackboardHeight = 50f;
            rect.x = graphRect.xMax - blackboardWidth;
            rect.y = graphRect.y;
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
                currentGraph.AddVariable();
            }
            GUILayout.EndArea();
            GUILayout.EndArea();
            GUI.EndClip();
            return rect;
        }
    }
}