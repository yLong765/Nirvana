using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    /// <summary>
    /// 标题特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class TitleAttribute : Attribute
    {
        public string title;

        public TitleAttribute(string title)
        {
            this.title = title;
        }
    }
    
    /// <summary>
    /// 描述特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

    /// <summary>
    /// 端口特性
    /// </summary>
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

    /// <summary>
    /// NodeInspector忽略属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HideInInspectorAttribute : Attribute
    {
        
    }
}