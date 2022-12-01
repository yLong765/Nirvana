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
    
    public abstract partial class Port
    {
        public string ID { get; set; }
        public string fieldName { get; private set; }
        public abstract Type type { get; }
        public Node node { get; set; }
        public int linkCount { get; set; }
        public int maxLinkCount { get; set; }
        
        public bool IsLink => linkCount > 0;
        public bool IsFullLink => linkCount == maxLinkCount;
    }
    
    public abstract class InPort : Port
    {
    }

    public abstract class OutPort : Port
    {
        public abstract object GetValue();
    }

    public class InPort<T> : InPort
    {
        public Func<T> outValue;
        private T _value;
        
        public T value => outValue != null ? outValue() : _value;
        public override Type type => typeof(T);

        public void BintTo(OutPort source)
        {
            if (source is OutPort<T> port)
            {
                outValue = port.getValue;
                return;
            }
            
            if (type.IsAssignableFrom(source.type))
            {
                outValue = () => (T) source.GetValue();
            }
        }

        public InPort(Node node, string ID)
        {
            this.node = node;
            this.ID = ID;
            this.maxLinkCount = 1;
        }
    }

    public class OutPort<T> : OutPort
    {
        public Func<T> getValue;

        public override Type type => typeof(T);
        public override object GetValue()
        {
            return getValue();
        }

        public OutPort(Node node, string ID, Func<T> getValue)
        {
            this.node = node;
            this.ID = ID;
            this.maxLinkCount = 100;
            this.getValue = getValue;
        }
    }
}