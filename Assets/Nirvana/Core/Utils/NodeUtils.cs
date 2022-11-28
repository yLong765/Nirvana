using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public static class NodeUtils
    {
        public static Port GetInPort(this Node node, string portFieldName)
        {
            if (node is FlowNode flowNode)
            {
                return flowNode.GetInPort(portFieldName);
            }

            return null;
        }
        
        public static Port GetOutPort(this Node node, string portFieldName)
        {
            if (node is FlowNode flowNode)
            {
                return flowNode.GetOutPort(portFieldName);
            }

            return null;
        }
        
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