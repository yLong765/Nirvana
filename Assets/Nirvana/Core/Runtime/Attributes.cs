using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class InPortAttribute : Attribute
    {
        public string name;

        public InPortAttribute() { }
        
        public InPortAttribute(string name)
        {
            this.name = name;
        }
    }
    
    public class OutPortAttribute : Attribute
    {
        public string name;

        public OutPortAttribute() { }
        
        public OutPortAttribute(string name)
        {
            this.name = name;
        }
    }
}