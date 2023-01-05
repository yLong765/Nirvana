using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class TitleAttribute : Attribute
    {
        public string title;

        public TitleAttribute(string title)
        {
            this.title = title;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PortAttribute : Attribute
    {
        public int order = 100;
        public int maxLink = 1;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InPortAttribute : PortAttribute
    {
        public InPortAttribute() { }
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
            maxLink = 100;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreInNodeInspectorAttribute : Attribute
    {
        
    }
}