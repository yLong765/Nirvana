using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class NodeInspector
    {
        /// <summary>
        /// 绘制Node Inspector内容（单选）
        /// </summary>
        public static void DrawGUI(Node node)
        {
            EditorGUI.BeginChangeCheck();

            node.tag = EditorUtils.DefaultTextField(node.tag, "Tag...");
            node.DrawInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                GraphUtils.willSetDirty = true;
            }
        }

        /// <summary>
        /// 绘制Node Inspector内容（多选）
        /// </summary>
        public static void DrawGUI(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                EditorGUILayout.LabelField(node.title);
            }
        }
    }
}