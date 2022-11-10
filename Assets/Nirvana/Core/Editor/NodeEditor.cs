using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Nirvana.Editor
{
    public class NodeEditor : EditorWindow
    {
        public static void DrawNodeGUI(Node node)
        {
            node.rect = GUILayout.Window(node.ID, node.rect, id =>
            {
                DrawNodeWindowGUI(id, node.rect);
            }, string.Empty, GUILayout.MinWidth(Node.MIN_SIZE.x), GUILayout.MinHeight(Node.MIN_SIZE.y));
        }

        private static void DrawNodeWindowGUI(int id, Rect windowRect)
        {
            GUI.Box(windowRect, string.Empty);
            GUILayout.Label("title");
        }
    }
}