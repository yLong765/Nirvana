using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeNameAttribute : Attribute
    {
        public string name;

        public NodeNameAttribute(string name)
        {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InPortAttribute : Attribute
    {
        public string name;
        public int order = 100;
        
        public InPortAttribute() {}

        public InPortAttribute(string name)
        {
            this.name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class OutPortAttribute : Attribute
    {
        public string name;
        public int order = 100;
        
        public OutPortAttribute() {}

        public OutPortAttribute(string name)
        {
            this.name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreInNodeInspectorAttribute : Attribute
    {
        
    }
}