using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public abstract partial class Node 
    {
        private static Vector2 MIN_SIZE = new(80, 50);
        
        private string _title;
        private string _tag;
        private Vector2 _position;
        private Vector2 _size;
        private Graph _graph;

        private List<Link> _inLinks = new List<Link>();
        private List<Link> _outLinks = new List<Link>();

        private Dictionary<string, Port> _inPorts = new Dictionary<string, Port>();
        private Dictionary<string, Port> _outPorts = new Dictionary<string, Port>();
        
        public List<Link> inLinks => _inLinks;
        public List<Link> outLinks => _outLinks;

        [JsonIgnore] public Dictionary<string, Port> inPorts => _inPorts;
        [JsonIgnore] public Dictionary<string, Port> outPorts => _outPorts;
        [JsonIgnore] public List<Port> inPortList => _inPorts.Values.ToList();
        [JsonIgnore] public List<Port> outPortList => _outPorts.Values.ToList();

        public string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = GetType().Name;
                }

                return _title;
            }
        }

        public string tag
        {
            get => _tag;
            set => _tag = value;
        }

        [JsonIgnore] public Graph graph
        {
            get => _graph;
            set => _graph = value;
        }

        public Vector2 position
        {
            get => _position;
            set => _position = value;
        }

        public Rect rect
        {
            get => new(_position.x, _position.y, _size.x, _size.y);
            set
            {
                _position.x = value.x;
                _position.y = value.y;
                _size.x = Mathf.Max(value.width, MIN_SIZE.x);
                _size.y = Mathf.Max(value.height, MIN_SIZE.y);
            }
        }
        
        public Port GetInPort(string ID)
        {
            return _inPorts.ContainsKey(ID) ? _inPorts[ID] : null;
        }
        
        public Port GetOutPort(string ID)
        {
            return _outPorts.ContainsKey(ID) ? _outPorts[ID] : null;
        }

        public Link GetInLink(Port port)
        {
            return inLinks.FirstOrDefault(link => link.targetPortId == port.ID);
        }
        
        public Link GetOutLink(Port port)
        {
            return outLinks.FirstOrDefault(link => link.sourcePortId == port.ID);
        }

        public virtual void OnCreate() { }
        
        public virtual void OnRefresh() { }

        public virtual void OnDelete()
        {
            for (int i = _inLinks.Count - 1; i >= 0; i--)
            {
                graph.DelLink(_inLinks[i]);
            }
            
            for (int i = outLinks.Count - 1; i >= 0; i--)
            {
                graph.DelLink(outLinks[i]);
            }
        }

        public virtual bool CanLinkToTarget()
        {
            return true;
        }

        public virtual bool CanLinkFromSource()
        {
            return true;
        }

        public static Node Create(Graph graph, Type type, Vector2 pos)
        {
            var newNode = (Node) Activator.CreateInstance(type);
            newNode.graph = graph;
            newNode.position = pos;
            newNode.OnCreate();
            return newNode;
        }

        public static bool IsNewLinkAllowed(Port sourcePort, Port targetPort)
        {
            if (sourcePort.linkCount == sourcePort.maxLinkCount)
            {
                LogUtils.Error($"Port [{sourcePort.node.title}.{sourcePort.ID}] link is full");
                return false;
            }
            
            if (targetPort.linkCount == targetPort.maxLinkCount)
            {
                LogUtils.Error($"Port [{targetPort.node.title}.{targetPort.ID}] link is full");
                return false;
            }

            if (!(sourcePort is FlowOutPort && targetPort is FlowInPort) && !targetPort.type.IsAssignableFrom(sourcePort.type))
            {
                LogUtils.Error("Non-identical or inherited types");
                return false;
            }

            if (sourcePort.node == targetPort.node)
            {
                LogUtils.Error($"Cannot connect to the same node [{sourcePort.node.title}]");
                return false;
            }

            var res = true;
            res &= sourcePort.node.CanLinkToTarget();
            res &= targetPort.node.CanLinkFromSource();
            return res;
        }
    }
}