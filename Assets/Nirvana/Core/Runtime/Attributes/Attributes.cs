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
    public class PortAttribute : Attribute
    {
        public string name;
        public int order = 100;
        public int maxLink = 1;
        public bool canDragLink = true;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InPortAttribute : PortAttribute
    {
        public InPortAttribute()
        {
            canDragLink = false;
        }

        public InPortAttribute(string name)
        {
            this.name = name;
            canDragLink = false;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class OutPortAttribute : PortAttribute
    {
        public OutPortAttribute()
        {
            maxLink = 100;
        }

        public OutPortAttribute(string name)
        {
            this.name = name;
            maxLink = 100;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreInNodeInspectorAttribute : Attribute
    {
        
    }
}