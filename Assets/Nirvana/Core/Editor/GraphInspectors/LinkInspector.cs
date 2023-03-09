
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public static class LinkInspector
    {
        /// <summary>
        /// 绘制Link Inspector内容
        /// </summary>
        public static void DrawGUI(Link link)
        {
            EditorGUI.BeginChangeCheck();

            link.DrawInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                GraphUtils.willSetDirty = true;
            }
        }
    }
}