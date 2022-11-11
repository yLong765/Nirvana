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
            GetWindow<GraphEditor>().Show();
        }

        private Vector2 _offset;
        private Rect _graphRect;
        private Event e;
        
        private Vector2 mousePosInGraph => e.mousePosition - _offset;

        public Graph currentGraph { get; private set; }

        private void OnEnable()
        {
            currentGraph ??= new Graph();
        }

        private void OnGUI()
        {
            _graphRect = new(0, 0, position.width, position.height);
            e = Event.current;
            
            EditorUtils.DrawBox(_graphRect, ColorUtils.gray13, Styles.normalBG);
            
            DrawGrid(_graphRect, _offset);
            
            GUI.BeginClip(_graphRect, _offset, default, false);
            
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            
            if (e.type == EventType.MouseDown && e.button == 0 && _graphRect.Contains(mousePosInGraph)) {
                if ( e.clickCount == 1 ) {
                    GraphUtils.activeNodes = null;
                    e.Use();
                }
            }
            
            GUI.EndClip();
            
            
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 1)
                {
                    GenericMenuPopup menu = new GenericMenuPopup("Fields");
                    menu.AddItem("测试", () => { currentGraph.AddNode(mousePosInGraph); });
                    menu.AddItem("鼠标位置", () => { Debug.Log(e.mousePosition); });
                    menu.Show();
                    e.Use();
                }
            }

            if (e.button == 2 && e.type == EventType.MouseDrag)
            {
                _offset += e.delta;
                e.Use();
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
    }
}