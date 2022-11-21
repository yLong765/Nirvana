using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class NodeWindow
    {
        public static void DrawNodeGUI(Node node)
        {
            node.rect = EditorUtils.Window(0, node.rect, id =>
            {
                DrawNodeWindowGUI(id, node);
            }, ColorUtils.gray21, StyleUtils.normalBG);

            if (node.isSelected)
            {
                EditorUtils.DrawBox(node.rect, ColorUtils.mediumPurple, StyleUtils.windowHeightLine);
            }
            
            DrawTag(node);
        }
        
        private static void DrawNodeWindowGUI(int id, Node node)
        {
            var titleHeight = StyleUtils.windowTitle.CalcSize(node.title).y;
            EditorUtils.DrawBox(new Rect(0, 0, node.rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
            GUILayout.Label(node.title, StyleUtils.windowTitle);
            
            node.DrawNodeGUI();

            var e = Event.current;
            if (GraphUtils.allowClick)
            {
                if (e.type == EventType.MouseDown && e.button != 2)
                {
                    //if (e.button == 0 || e.button == 1)
                    {
                        GraphUtils.activeNodes = null;
                        GraphUtils.AddActiveNode(node);
                        GUIUtility.keyboardControl = 0;
                    }

                    e.Use();
                }

                if (e.type == EventType.MouseDrag)
                {
                    foreach (var t in GraphUtils.activeNodes)
                    {
                        t.position += e.delta;
                    }

                    e.Use();
                }

                if (e.type == EventType.MouseUp && e.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        node.graph.RemoveNode(node);
                        GraphUtils.activeNodes = null;
                    });
                    menu.ShowAsContext();

                    e.Use();
                }
            }
        }
        
        private static void DrawTag(Node node)
        {
            if (!string.IsNullOrEmpty(node.tag))
            {
                var tagText = "Tag:" + node.tag;
                var tagHeight = StyleUtils.tagText.CalcHeight(tagText, node.rect.width);
                var footRect = new Rect(node.rect.x, node.rect.y - tagHeight - 2, node.rect.width, tagHeight);
                GUI.Box(footRect, tagText, StyleUtils.tagText);
            }
        }
    }
}