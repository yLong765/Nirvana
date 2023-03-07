using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class NodeWindow
    {
        /// <summary>
        /// 绘制Node
        /// </summary>
        public static void DrawNodeGUI(Node node)
        {
            node.rect = EditorUtils.Window(node.ID, node.rect, id => { DrawNodeWindowGUI(id, node); }, ColorUtils.gray21, StyleUtils.normalBG,
                GUILayout.MaxWidth(Node.MIN_SIZE.x), GUILayout.MaxHeight(Node.MIN_SIZE.y));

            if (node.isSelected)
            {
                EditorUtils.DrawBox(node.rect, ColorUtils.mediumPurple, StyleUtils.windowHeightLine);
            }

            DrawTag(node);

            node.DrawLinkGUI();
        }

        /// <summary>
        /// 绘制NodeWindow内部
        /// </summary>
        private static void DrawNodeWindowGUI(int id, Node node)
        {
            var e = Event.current;

            DrawTitleGUI(node);
            node.DrawWindowGUI();
            HandleEvents(node, e);
        }

        /// <summary>
        /// 绘制Node的标题
        /// </summary>
        /// <param name="node"></param>
        private static void DrawTitleGUI(Node node)
        {
            var titleHeight = StyleUtils.windowTitle.CalcSize(node.title).y;
            EditorUtils.DrawBox(new Rect(0, 0, node.rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
            GUILayout.Label(node.title, StyleUtils.windowTitle);
        }

        /// <summary>
        /// Node Rect内相关的事件处理
        /// </summary>
        private static void HandleEvents(Node node, Event e)
        {
            if (GraphUtils.allowClick && e.type == EventType.MouseDown && e.button != 2)
            {
                Undo.RegisterCompleteObjectUndo(node.graph, "Move Node");
                GraphUtils.Select(node);
                e.Use();
            }

            // right click MouseUp menu
            if (GraphUtils.allowClick && (e.type == EventType.MouseUp && e.button == 1 || e.type == EventType.ContextClick))
            {
                var menu = new GenericMenu();
                if (GraphUtils.activeNodes.Count > 1)
                {
                    menu.AddItem(new GUIContent("Delete Selected Nodes"), false, () =>
                    {
                        foreach (var node in GraphUtils.activeNodes)
                        {
                            node.graph.RemoveNode(node);
                        }
                        GraphUtils.ClearSelect();
                    });
                    menu.ShowAsContext();
                }
                else
                {
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        node.graph.RemoveNode(node);
                        GraphUtils.ClearSelect();
                    });
                    menu.ShowAsContext();
                }

                menu.ShowAsContext();
                e.Use();
            }

            if (GraphUtils.allowClick && e.button != 2)
            {
                if (e.type == EventType.MouseDrag && GraphUtils.activeNodes.Count > 1)
                {
                    foreach (var n in GraphUtils.activeNodes)
                    {
                        n.position += e.delta;
                    }

                    return;
                }

                if (node.isSelected && e.type == EventType.MouseDrag)
                {
                    node.position += e.delta;
                }
            }
        }

        /// <summary>
        /// 绘制Node的Tag内容
        /// </summary>
        private static void DrawTag(Node node)
        {
            if (!string.IsNullOrEmpty(node.tag))
            {
                var tagText = "Tag:" + node.tag;
                var tagHeight = StyleUtils.tagText.CalcHeight(tagText, node.rect.width);
                var footRect = new Rect(node.rect.x, node.rect.y - tagHeight - 3, node.rect.width, tagHeight);
                GUI.Box(footRect, tagText, StyleUtils.tagText);
            }
        }
    }
}