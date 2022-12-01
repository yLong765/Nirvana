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
        private Port _sourcePort;
        private Port _targetPort;

        public Port sourcePort
        {
            get => _sourcePort;
            set => _sourcePort = value;
        }

        public Port targetPort
        {
            get => _targetPort;
            set => _targetPort = value;
        }

        [JsonIgnore] public Node sourceNode => _sourcePort.node;
        [JsonIgnore] public Node targetNode => _targetPort.node;
        [JsonIgnore] public string sourcePortId => _sourcePort.ID;
        [JsonIgnore] public string targetPortId => _targetPort.ID;

        public void SetSourcePort(Port port)
        {
            _sourcePort = port;
            _sourcePort.node.outLinks.Add(this);
            _sourcePort.linkCount++;
        }

        public void SetTargetPort(Port port)
        {
            _targetPort = port;
            _targetPort.node.inLinks.Add(this);
            _targetPort.linkCount++;
        }

        public static Link Create(Port sourcePort, Port targetPort)
        {
            var newLink = new Link();
            newLink.SetSourcePort(sourcePort);
            newLink.SetTargetPort(targetPort);
            return newLink;
        }

#if UNITY_EDITOR
        public void DrawInspectorGUI()
        {
            GUILayout.Label("Link Node: " + sourcePort.node.title + " link to " + sourcePort.node.title);
        }
#endif
    }
}