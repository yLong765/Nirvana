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
        private string _sourcePortID;
        private string _targetPortID;
        private Port _sourcePort;
        private Port _targetPort;

        [JsonIgnore]
        public Port sourcePort
        {
            get => _sourcePort ??= _sourceNode.GetOutPort(sourcePortId);
            set => _sourcePort = value;
        }

        [JsonIgnore]
        public Port targetPort
        {
            get => _targetPort ??= _targetNode.GetInPort(_targetPortID);
            set => _targetPort = value;
        }

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

        public string sourcePortId
        {
            get => _sourcePort != null ? _sourcePort.ID : _sourcePortID;
            set => _sourcePortID = value;
        }

        public string targetPortId
        {
            get => _targetPort != null ? _targetPort.ID : _targetPortID;
            set => _targetPortID = value;
        }

        public void SetSourcePort(Port port)
        {
            sourcePort = port;
            sourceNode = port.node;
            sourcePortId = port.ID;
            sourcePort.node.outLinks.Add(this);
            sourcePort.linkCount++;
        }

        public void SetTargetPort(Port port)
        {
            targetPort = port;
            targetNode = port.node;
            targetPortId = port.ID;
            targetPort.node.inLinks.Add(this);
            targetPort.linkCount++;
        }

        public void RefreshSourcePort()
        {
            _sourcePort = null;
            if (sourcePort != null)
            {
                sourcePort.linkCount++;
            }
        }

        public void RefreshTargetPort()
        {
            _targetPort = null;
            if (targetPort != null)
            {
                targetPort.linkCount++;
            }
            
            if (sourcePort is InPort inPort && targetPort is OutPort outPort)
            {
                inPort.BindTo(outPort);
            }
        }

        public void Refresh()
        {
            
        }

        public static Link Create(Port sourcePort, Port targetPort)
        {
            var newLink = new Link();
            newLink.SetSourcePort(sourcePort);
            newLink.SetTargetPort(targetPort);
            if (sourcePort is InPort inPort && targetPort is OutPort outPort)
            {
                inPort.BindTo(outPort);
            }
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