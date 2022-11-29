
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public static class LinkInspector
    {
        public static void DrawInspector(Rect rect, Link link)
        {
            EditorGUI.BeginChangeCheck();
                
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, rect.width), ColorUtils.gray21, StyleUtils.normalBG);
            var titleHeight = StyleUtils.panelTitle.CalcSize("Link").y;
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
            GUILayout.Label("Link", StyleUtils.panelTitle);
            GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 4, rect.xMax - 4, rect.yMax - 2));
            
            link.DrawInspectorGUI();

            GUILayout.EndArea();
            if (EditorGUI.EndChangeCheck())
            {
                GraphUtils.willSetDirty = true;
            }
        }
    }
}