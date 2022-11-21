using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.Attributes
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
        
    }
    
    public class OutPortAttribute : Attribute
    {
        
    }
}