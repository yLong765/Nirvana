using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public abstract partial class Port
    {
        private string _ID;
        private string _name;
        private Node _node;
        private int _linkCount;
        private int _maxLinkCount;

        public string ID
        {
            get => _ID;
            set => _ID = value;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public Node node
        {
            get => _node;
            set => _node = value;
        }

        public int linkCount
        {
            get => _linkCount;
            set => _linkCount = value;
        }

        public int maxLinkCount
        {
            get => _maxLinkCount;
            set => _maxLinkCount = value;
        }

        public Port(Node node, string ID, string name)
        {
            _node = node;
            _ID = ID;
            _name = name;
        }
        
        [JsonIgnore] public abstract Type type { get; }
        [JsonIgnore] public bool isLink => linkCount > 0;
        [JsonIgnore] public bool isFullLink => linkCount == maxLinkCount;
    }
    
    public class FlowInPort : Port
    {
        public FlowInPort(Node node, string ID, string name) : base(node, ID, name)
        {
            maxLinkCount = 1;
        }

        public override Type type => typeof(FlowInPort);
    }

    public class FlowOutPort : Port
    {
        public FlowOutPort(Node node, string ID, string name) : base(node, ID, name)
        {
            maxLinkCount = 100;
        }
        
        public override Type type => typeof(FlowOutPort);
    }

    public abstract class InPort : Port
    {
        public InPort(Node node, string ID, string name) : base(node, ID, name) { }
    }

    public abstract class OutPort : Port
    {
        public OutPort(Node node, string ID, string name) : base(node, ID, name) { }
        
        public abstract object GetValue();
    }

    public class InPort<T> : InPort
    {
        [JsonIgnore] public Func<T> outValue;
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

        public InPort(Node node, string ID, string name) : base(node, ID, name)
        {
            maxLinkCount = 1;
        }
    }

    public class OutPort<T> : OutPort
    {
        [JsonIgnore] public Func<T> getValue;

        public override Type type => typeof(T);
        public override object GetValue()
        {
            return getValue();
        }

        public OutPort(Node node, string ID, string name, Func<T> getValue) : base(node, ID, name)
        {
            maxLinkCount = 100;
            this.getValue = getValue;
        }
    }
}