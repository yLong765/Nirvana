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
        private string _sourceOutPort;
        private string _targetInPort;

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

        public string sourceOutPort
        {
            get => _sourceOutPort;
            set => _sourceOutPort = value;
        }

        public string targetInPort
        {
            get => _targetInPort;
            set => _targetInPort = value;
        }

        public Port GetSourceOutPort()
        {
            return _sourceNode.GetOutPort(_sourceOutPort);
        }

        public Port GetTargetInPort()
        {
            return _targetNode.GetInPort(_targetInPort);
        }

        public void SetSourceNode(Node node, string portName)
        {
            _sourceNode = node;
            _sourceOutPort = portName;
            _sourceNode.outLinks.Add(this);
            GetSourceOutPort().linkCount++;
        }

        public void SetTargetNode(Node node, string portName)
        {
            _targetNode = node;
            _targetInPort = portName;
            _targetNode.inLinks.Add(this);
            GetTargetInPort().linkCount++;
        }

#if UNITY_EDITOR
        public void DrawInspectorGUI()
        {
            GUILayout.Label("Link Node: " + sourceNode.title + " link to " + targetNode.title);
            GUILayout.Label("Link Port: " + sourceOutPort + " link to " + targetInPort);
        }
#endif
    }
}