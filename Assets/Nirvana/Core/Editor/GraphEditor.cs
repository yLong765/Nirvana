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
        
        public Graph currentGraph { get; private set; }

        private void OnEnable()
        {
            currentGraph ??= new Graph();
        }

        private void OnGUI()
        {
            _graphRect = new(0, 0, position.width, position.height);
            
            DrawGrid(_graphRect, _offset);

            GUI.BeginClip(_graphRect, _offset, default, false);
            BeginWindows();
            DrawNodesGUI(currentGraph);
            EndWindows();
            GUI.EndClip();

            var e = Event.current;

            if (e.button == 1 && e.type == EventType.MouseDown)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("测试"), false, () =>
                {
                    currentGraph.AddNode(e.mousePosition - _offset);
                });
                menu.AddItem(new GUIContent("鼠标位置"), false, () =>
                {
                    Debug.Log(e.mousePosition);
                });
                menu.ShowAsContext();
                e.Use();
            }

            if (e.type == EventType.MouseDrag)
            {
                _offset += e.delta;
                e.Use();
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