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
        
        private Node _currentNode;
        public Node currentNode
        {
            set => _currentNode = value;
        }
        
        private void OnGUI()
        {
            if (_currentNode == null)
            { 
                EditorGUILayout.HelpBox(new GUIContent("No select one node in graph!"));
                return;
            }
            
            GUILayout.Label(_currentNode.title, Styles.menuTitle);
            _currentNode.tag = GUILayout.TextField(_currentNode.tag);
            EditorUtils.DefaultTextField(_currentNode.tag, "Introduction...");
        }

        public static void DrawGUI(Node node)
        {
            if (window == null)
            {
                window = GetWindow<NodeInspector>();
                window.titleContent = new GUIContent("Inspector");
                window.Show();
            }

            window.currentNode = node;
            window.Repaint();
        }
    }
}