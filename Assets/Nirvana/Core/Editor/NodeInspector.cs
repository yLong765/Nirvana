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
                GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 4, rect.xMax - 2, rect.yMax - 2));

                node.DrawInspectorGUI();
                
                GUILayout.EndArea();
                if (EditorGUI.EndChangeCheck())
                {
                    GraphUtils.willSetDirty = true;
                }
            }
        }
    }
}