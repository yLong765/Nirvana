using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Nirvana.Editor
{
    public class NodeEditor
    {
        public static void DrawNodeGUI(Node node)
        {
            node.rect = GUILayout.Window(node.ID, node.rect, id => { DrawNodeWindowGUI(id, node); }, string.Empty,
                GUILayout.MinWidth(Node.MIN_SIZE.x), GUILayout.MinHeight(Node.MIN_SIZE.y));
        }

        private static void DrawNodeWindowGUI(int id, Node node)
        {
            GUI.Box(node.rect, string.Empty);
            GUILayout.Label("title");

            var e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    GraphUtils.activeNodes = null;
                    GraphUtils.AddActiveNode(node);
                }
            }

            if (e.type == EventType.MouseDrag)
            {
                var activeNodes = GraphUtils.activeNodes;
                for (int i = 0; i < activeNodes.Count; i++)
                {
                    activeNodes[i].position += e.delta;
                }
                e.Use();
            }
        }
    }
}