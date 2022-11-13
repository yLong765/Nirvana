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
        private static Rect graphRect;
        private static Rect inspectorRect;

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
            return mousePos - graphOffset;
        }

        private void OnEnable()
        {
            currentGraph ??= new Graph();
            currentGraph.name = "Graph Canvas";
        }

        private void OnGUI()
        {
            inspectorRect = Rect.MinMaxRect(2, GRAPH_TOP + 2, GRAPH_LEFT, position.height - GRAPH_BOTTOM);
            graphRect = Rect.MinMaxRect(GRAPH_LEFT, GRAPH_TOP, position.width - GRAPH_RIGHT, position.height - GRAPH_BOTTOM);
            e = Event.current;

            EditorUtils.DrawBox(graphRect, ColorUtils.gray13, Styles.normalBG);
            DrawGrid(graphRect, graphOffset);
            NodesWindowPrevEvent();
            
            GUI.BeginClip(graphRect, graphOffset, default, false);
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            GUI.EndClip();

            NodesWindowPostEvent(MousePosToGraph(e.mousePosition));
            DrawToolbar(currentGraph);
            DrawInspector(inspectorRect);
        }

        private static void NodesWindowPrevEvent()
        {
            if (graphRect.Contains(MousePosToGraph(e.mousePosition)) && e.type == EventType.MouseDrag && e.button == 2)
            {
                graphOffset += e.delta;
                e.Use();
            }
        }

        private static void NodesWindowPostEvent(Vector2 mousePosInGraph)
        {
            if (graphRect.Contains(MousePosToGraph(e.mousePosition)) && e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    if ( e.clickCount == 1 ) {
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
                        menu.AddItem(t.Name, () => { currentGraph.AddNode(t, mousePosInGraph); });
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
                menu.AddItem(new GUIContent("Export Json"), false, () =>
                {
                    Debug.Log("Export Json");
                });
                menu.ShowAsContext();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label(graph.name, Styles.graphTitle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Blackboard", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
            {
                
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawInspector(Rect rect)
        {
            //EditorUtils.DrawBox(rect, Color.blue, Styles.normalBG);
            GUI.BeginClip(new Rect(0, 0, rect.width, rect.height));
            GUILayout.BeginArea(new Rect(2, GRAPH_TOP + 2, rect.width - 2, rect.height - GRAPH_TOP - 2));
            
            var currentNode = GraphUtils.activeNodes.Count == 1 ? GraphUtils.activeNodes[0] : null;
            if (currentNode == null)
            { 
                EditorGUILayout.HelpBox("No select one node in graph!", MessageType.Info);
            }
            else
            {
                var titleHeight = Styles.CalcSize(Styles.inspectorTitle, currentNode.title).y;
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, Styles.normalBG);
                GUILayout.Label(currentNode.title, Styles.inspectorTitle);
                var lastRect = GUILayoutUtility.GetLastRect().ModifyWitch(18);
                if (GUI.Button(lastRect, "◂", Styles.symbolText))
                {
                    // Open InspectorGUI
                }
                GUILayout.Space(4f);
                currentNode.tag = GUILayout.TextField(currentNode.tag);
                EditorUtils.DefaultTextField(currentNode.tag, "Tag...");
            }

            GUILayout.EndArea();
            GUI.EndClip();
        }
    }
}