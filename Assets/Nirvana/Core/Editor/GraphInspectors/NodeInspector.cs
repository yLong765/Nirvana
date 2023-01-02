using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class NodeInspector
    {
        public static void DrawGUI(Rect rect, Node node)
        {
            if (node == null)
            {
                GUILayout.BeginArea(Rect.MinMaxRect(2, 2, rect.xMax - 2, rect.yMax - 2));
                EditorGUILayout.HelpBox("No select one node in graph!", MessageType.Info);
                GUILayout.EndArea();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, rect.width), ColorUtils.gray21, StyleUtils.normalBG);
                var titleHeight = StyleUtils.panelTitle.CalcSize(node.title).y;
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
                GUILayout.Label(node.title, StyleUtils.panelTitle);
                GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 4, rect.xMax - 4, rect.yMax - 2));
                
                node.tag = EditorUtils.DefaultTextField(node.tag, "Tag...");
                
                node.DrawInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    GraphUtils.willSetDirty = true;
                }
            }
        }

        public static void DrawGUI(Rect rect, List<Node> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                GUILayout.BeginArea(Rect.MinMaxRect(2, 2, rect.xMax - 2, rect.yMax - 2));
                EditorGUILayout.HelpBox("No select one node in graph!", MessageType.Info);
                GUILayout.EndArea();
            }
            else
            {
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, rect.width), ColorUtils.gray21, StyleUtils.normalBG);
                var titleHeight = StyleUtils.panelTitle.CalcSize("Multi-select").y;
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
                GUILayout.Label("Multi-select", StyleUtils.panelTitle);
                GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 4, rect.xMax - 4, rect.yMax - 2));

                foreach (var node in nodes)
                {
                    EditorGUILayout.LabelField(node.title);
                }
            }
        }
    }
}