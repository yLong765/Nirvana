using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class NodeInspector : EditorWindow
    {
        private static NodeInspector window;

        private void OnGUI()
        {
            DrawGUI(position, GraphUtils.activeNodes.Count == 1 ? GraphUtils.activeNodes[0] : null);
        }

        public static void OpenWindow()
        {
            var window = GetWindow<NodeInspector>();
            window.maxSize = new Vector2(200, 600);
            window.Show();
        }

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
                
                var titleHeight = Styles.CalcSize(Styles.panelTitle, node.title).y;
                EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, Styles.normalBG);
                GUILayout.Label(node.title, Styles.panelTitle);
                if (!GraphUtils.isInspectorPanel)
                {
                    // var lastRect = GUILayoutUtility.GetLastRect().ModifyWitch(18);
                    // if (GUI.Button(lastRect, "◂", Styles.symbolText))
                    // {
                    //     // Open InspectorGUI
                    //     GraphUtils.isInspectorPanel = true;
                    //     OpenWindow();
                    // }
                }
                
                GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 4, rect.xMax - 2, rect.yMax - 2));
                node.tag = EditorGUILayout.TextField(node.tag);
                EditorUtils.DefaultTextField(node.tag, "Tag...");
                GUILayout.EndArea();

                if (EditorGUI.EndChangeCheck())
                {
                    GraphUtils.willSetDirty = true;
                }
            }
        }
    }
}