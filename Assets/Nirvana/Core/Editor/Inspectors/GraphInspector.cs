using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    [CustomEditor(typeof(Graph), true)]
    public class GraphInspector : UnityEditor.Editor
    {
        private Graph _asset;

        private void OnEnable()
        {
            _asset = target as Graph;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Debug"))
            {
                Prefs.showDebugModel = !Prefs.showDebugModel;
            }

            if (Prefs.showDebugModel)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector2Field("拖拽原点偏移量", _asset.offset);
                EditorGUILayout.FloatField("缩放", _asset.zoom);
                EditorGUILayout.LabelField("Graph Json Data");
                EditorGUILayout.TextArea(_asset.serializedData);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}