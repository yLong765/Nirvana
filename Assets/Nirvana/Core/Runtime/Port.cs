using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public enum PortType
    {
        In,
        Out,
    }
    
    public partial class Port
    {
        private string _name;
        private string _fieldName;
        private Type _fieldType;
        private Node _node;

        public string name
        {
            get => _name;
            set => _name = value;
        }
        
        public string fieldName
        {
            get => _fieldName;
            set => _fieldName = value;
        }

        public Type fieldType
        {
            get => _fieldType;
            set => _fieldType = value;
        }
        
        public Node node
        {
            get => _node;
            set => _node = value;
        }
        
        public static Port Create(Node node, string portName, string fieldName, Type fieldType, PortAttribute portAtt, PortType portType)
        {
            var newPort = new Port();
            newPort.node = node;
            newPort.name = portName;
            newPort.fieldName = fieldName;
            newPort.fieldType = fieldType;
            newPort.order = portAtt.order;
            newPort.linkCount = 0;
            newPort.maxLinkCount = portAtt.maxLink;
            newPort.canDragLink = portAtt.canDragLink;
            newPort.portType = portType;
            return newPort;
        }
    }
}