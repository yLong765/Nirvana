using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace Nirvana
{
    public abstract partial class Node
    {
        protected bool isOverwriteOnGraphStartMethod = false;
        
        private Graph _graph;

        private List<Link> _inLinks = new List<Link>();
        private List<Link> _outLinks = new List<Link>();

        private Dictionary<string, Port> _inPorts = new Dictionary<string, Port>();
        private Dictionary<string, Port> _outPorts = new Dictionary<string, Port>();
        
        /// <summary>
        /// 所有的入Link
        /// </summary>
        public List<Link> inLinks => _inLinks;
        /// <summary>
        /// 所有的出Link
        /// </summary>
        public List<Link> outLinks => _outLinks;

        /// <summary>
        /// 所有的入端口字典
        /// </summary>
        [JsonIgnore] public Dictionary<string, Port> inPorts => _inPorts;
        /// <summary>
        /// 所有的出端口字典
        /// </summary>
        [JsonIgnore] public Dictionary<string, Port> outPorts => _outPorts;
        /// <summary>
        /// 所有的入端口List
        /// </summary>
        [JsonIgnore] public List<Port> inPortList => _inPorts.Values.ToList();
        /// <summary>
        /// 所有的出端口List
        /// </summary>
        [JsonIgnore] public List<Port> outPortList => _outPorts.Values.ToList();

        /// <summary>
        /// 所有的端口
        /// </summary>
        [JsonIgnore]
        public List<Port> allPorts
        {
            get
            {
                var ports = new List<Port>();
                ports.AddRange(inPortList);
                ports.AddRange(outPortList);
                return ports;
            }
        }

        /// <summary>
        /// 关联的Graph
        /// </summary>
        [JsonIgnore] public Graph graph
        {
            get => _graph;
            set => _graph = value;
        }

        /// <summary>
        /// 通过端口ID获取入端口
        /// </summary>
        public Port GetInPort(string ID)
        {
            return _inPorts.ContainsKey(ID) ? _inPorts[ID] : null;
        }
        
        /// <summary>
        /// 通过端口ID获取出端口
        /// </summary>
        public Port GetOutPort(string ID)
        {
            return _outPorts.ContainsKey(ID) ? _outPorts[ID] : null;
        }

        /// <summary>
        /// 通过端口获取入Link
        /// </summary>
        public Link GetInLink(Port port)
        {
            return inLinks.FirstOrDefault(link => link.targetPortId == port.ID);
        }
        
        /// <summary>
        /// 通过端口获取出Link
        /// </summary>
        public Link GetOutLink(Port port)
        {
            return outLinks.FirstOrDefault(link => link.sourcePortId == port.ID);
        }

        /// <summary>
        /// Node创建时调用
        /// </summary>
        public void Create()
        {
            isOverwriteOnGraphStartMethod = !(GetType().GetMethod("OnGraphStart")?.DeclaringType == typeof(Node));
            OnCreate();
        }

        #region Virtual

        protected virtual void OnCreate() { }

        protected virtual void OnRefresh() { }
        
        public virtual void OnGraphStart() {}

        public virtual void OnGraphStop() { }

        public virtual void OnDestroy()
        {
            for (int i = _inLinks.Count - 1; i >= 0; i--)
            {
                graph.RemoveLink(_inLinks[i]);
            }
            
            for (int i = outLinks.Count - 1; i >= 0; i--)
            {
                graph.RemoveLink(outLinks[i]);
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
        
        #endregion

        /// <summary>
        /// 创建Node
        /// </summary>
        public static Node Create(Graph graph, Type type, Vector2 pos)
        {
            var newNode = (Node) Activator.CreateInstance(type);
            newNode.graph = graph;
            newNode.position = pos;
            newNode.Create();
            return newNode;
        }

        /// <summary>
        /// 是否允许新Link创建
        /// </summary>
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