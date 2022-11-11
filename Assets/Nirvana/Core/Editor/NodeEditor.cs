using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Editor;
using UnityEngine;
using UnityEditor;

namespace Nirvana.Editor
{
    public class NodeEditor
    {
        public static void DrawNodeGUI(Node node)
        {
            GUI.color = Color.white.SetRGB(0.3f);

            node.rect = GUILayout.Window(node.ID, node.rect, id => { DrawNodeWindowGUI(id, node); }, string.Empty, Styles.nodeWindow,
                GUILayout.MinWidth(Node.MIN_SIZE.x), GUILayout.MinHeight(Node.MIN_SIZE.y));
            
            GUI.color = Color.white;

            if (node.isSelected)
            {
                GUI.color = Color.green;
                GUI.Box(node.rect, string.Empty, Styles.nodeWindowHeightLine);
                GUI.color = Color.white;
            }
        }

        private static void DrawNodeWindowGUI(int id, Node node)
        {
            GUILayout.BeginHorizontal(Styles.nodeWindowTitleBg);
            GUILayout.Label(node.title, Styles.nodeWindowTitle);
            GUILayout.EndHorizontal();

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