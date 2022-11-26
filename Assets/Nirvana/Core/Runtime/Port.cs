using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public partial class Port
    {
        private string _name;
        private string _fieldName;
        private Type _fieldFieldType;

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
            get => _fieldFieldType;
            set => _fieldFieldType = value;
        }
        
        public static Port Create(Node node, string portName, string fieldName, Type fieldType, int order)
        {
            var newPort = new Port();
            newPort.node = node;
            newPort.name = portName;
            newPort.fieldName = fieldName;
            newPort.fieldType = fieldType;
            newPort.order = order;
            return newPort;
        }
    }
}