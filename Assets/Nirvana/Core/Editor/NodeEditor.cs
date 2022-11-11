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
            node.rect = EditorUtils.Window(node.ID, node.rect, id => { DrawNodeWindowGUI(id, node); }, ColorUtils.gray21, Styles.normalBG,
                GUILayout.MinWidth(Node.MIN_SIZE.x), GUILayout.MinHeight(Node.MIN_SIZE.y));

            if (node.isSelected)
            {
                EditorUtils.DrawBox(node.rect, ColorUtils.mediumPurple, Styles.windowHeightLine);
            }
        }

        private static void DrawNodeWindowGUI(int id, Node node)
        {
            var titleHeight = Styles.windowTitle.CalcSize(node.titleContent).y + 3;
            EditorUtils.DrawBox(new Rect(0, 0, node.rect.width, titleHeight), ColorUtils.darkOrang2, Styles.normalBG);
            GUILayout.Label(node.title, Styles.windowTitle);

            var e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                if (e.button == 0)
                {
                    GraphUtils.activeNodes = null;
                    GraphUtils.AddActiveNode(node);
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
        }
    }
}