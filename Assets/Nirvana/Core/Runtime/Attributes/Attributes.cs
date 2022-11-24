using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class NodeNameAttribute : Attribute
    {
        public string name;

        public NodeNameAttribute(string name)
        {
            this.name = name;
        }
    }

    public class LeftPortAttribute : Attribute
    {
        
    }

    public class RightPortAttribute : Attribute
    {
        
    }

    public class DownPortAttribute : Attribute
    {
        
    }
    
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

    public class IgnoreInNodeInspectorAttribute : Attribute
    {
        
    }
}