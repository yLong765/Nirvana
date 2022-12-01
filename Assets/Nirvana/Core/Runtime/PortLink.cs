using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class PortLink : Link
    {
        private string _sourcePortId;
        private string _targetPortId;
        private Port _sourcePort;
        private Port _targetPort;

        public string sourcePortId
        {
            get => _sourcePortId;
            set => _sourcePortId = value;
        }

        public string targetPortId
        {
            get => _targetPortId;
            set => _targetPortId = value;
        }

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

        public void SetSourcePort(Port source)
        {
            sourcePort = source;
            sourcePortId = source.ID;
            sourcePort.node.outLinks.Add(this);
            sourcePort.linkCount++;
        }

        public void SetTargetPort(Port target)
        {
            targetPort = target;
            targetPortId = target.ID;
            targetPort.node.inLinks.Add(this);
            targetPort.linkCount++;
        }

        public static PortLink Create(Port source, Port target)
        {
            var newLink = new PortLink();
            newLink.SetSourcePort(source);
            newLink.SetTargetPort(target);
            return newLink;
        }
    }
}