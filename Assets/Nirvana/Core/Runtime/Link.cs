using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class Link
    {
        private Node _sourceNode;
        private Node _targetNode;
        private string _sourceOutPort;
        private string _targetInPort;

        public Node sourceNode => _sourceNode;

        public Node targetNode => _targetNode;

        public string sourceOutPort => _sourceOutPort;

        public string targetInPort => _targetInPort;

        public void SetSourceNode(Node node, string portName)
        {
            _sourceNode = node;
            _sourceOutPort = portName;
            _sourceNode.outLinks.Add(this);

            if (_sourceNode.TryGetOutPort(_sourceOutPort, out Port port))
            {
                port.isLink = true;
                port.linkCount++;
            }
        }

        public void SetTargetNode(Node node, string portName)
        {
            _targetNode = node;
            _targetInPort = portName;
            _targetNode.inLinks.Add(this);

            if (_targetNode.TryGetInPort(_targetInPort, out Port port))
            {
                port.isLink = true;
                port.linkCount++;
            }
        }
    }
}