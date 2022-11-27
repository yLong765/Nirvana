using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public static class NodeUtils
    {
        public static bool TryGetInPort(this Node node, string portFieldName, out Port port)
        {
            if (node is FlowNode flowNode)
            {
                port = flowNode.GetInPort(portFieldName);
                return port != null;
            }

            port = null;
            return false;
        }
        
        public static bool TryGetOutPort(this Node node, string portFieldName, out Port port)
        {
            if (node is FlowNode flowNode)
            {
                port = flowNode.GetOutPort(portFieldName);
                return port != null;
            }

            port = null;
            return false;
        }
    }
}