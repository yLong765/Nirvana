using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class Link
    {
        private Node _sourceNode;
        private Node _targetNode;
        private string _sourcePort;
        private string _targetPort;

        public Node sourceNode => _sourceNode;

        public Node targetNode => _targetNode;

        public string sourcePort => _sourcePort;

        public string targetPort => _targetPort;

        public void SetSourceNode(Node node, string portName)
        {
            _sourceNode = node;
            _sourcePort = portName;
            _sourceNode.outLinks.Add(this);
        }
        
        public void SetTargetNode(Node node, string portName)
        {
            _targetNode = node;
            _targetPort = portName;
            _targetNode.inLinks.Add(this);
        }
    }
}