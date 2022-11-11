using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public static class EditorUtils
    {
        public static void DrawBox(Rect rect, Color color, GUIStyle style)
        {
            GUI.color = color;
            GUI.Box(rect, string.Empty, style);
            GUI.color = Color.white;
        }

        public static Rect Window(int id, Rect rect, GUI.WindowFunction func, Color color, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            GUI.color = color;
            var newRect = GUILayout.Window(id, rect, func, string.Empty, style, layoutOptions);
            GUI.color = Color.white;
            return newRect;
        }

        private static string search = string.Empty;
        public static string SearchField(string search)
        {
            GUILayout.BeginHorizontal();
            search = EditorGUILayout.TextField(search, Styles.toolbarSearchTextField);
            if (!string.IsNullOrEmpty(search) && GUILayout.Button(string.Empty, Styles.toolbarSearchCancelButton))
            {
                search = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.EndHorizontal();
            return search;
        }
    }
}