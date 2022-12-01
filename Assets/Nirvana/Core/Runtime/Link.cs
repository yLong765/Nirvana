using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nirvana
{
    public class Link
    {
        private Node _sourceNode;
        private Node _targetNode;

        public Node sourceNode
        {
            get => _sourceNode;
            set => _sourceNode = value;
        }

        public Node targetNode
        {
            get => _targetNode;
            set => _targetNode = value;
        }

        public void SetSourceNode(Node node)
        {
            _sourceNode = node;
            _sourceNode.outLinks.Add(this);
        }

        public void SetTargetNode(Node node)
        {
            _targetNode = node;
            _targetNode.inLinks.Add(this);
        }

#if UNITY_EDITOR
        public void DrawInspectorGUI()
        {
            GUILayout.Label("Link Node: " + sourceNode.title + " link to " + targetNode.title);
        }
#endif
    }
}