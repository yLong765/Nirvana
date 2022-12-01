using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace Nirvana
{
    public class FlowNode : Node
    {
        // [Name("▶")][InPort(order = 0), IgnoreInNodeInspector] public Port inPort;
        // [Name("▶")][OutPort(order = 0), IgnoreInNodeInspector] public Port outPort;

        // private List<Port> _inPorts = new List<Port>();
        // private List<Port> _outPorts = new List<Port>();
        
        // [JsonIgnore] public List<Port> allPorts
        // {
        //     get
        //     {
        //         var ports = new List<Port>();
        //         ports.AddRange(_inPorts);
        //         ports.AddRange(_outPorts);
        //         return ports;
        //     }
        // }
        //
        // [JsonIgnore] public List<Port> inPorts => _inPorts;
        // [JsonIgnore] public List<Port> outPorts => _outPorts;

        private Dictionary<string, Port> _inPorts = new Dictionary<string, Port>();
        private Dictionary<string, Port> _outPorts = new Dictionary<string, Port>();

        private bool _updateLink = false;

        public Port GetInPort(string ID)
        {
            return _inPorts.ContainsKey(ID) ? _inPorts[ID] : null;
        }
        
        public Port GetOutPort(string ID)
        {
            return _outPorts.ContainsKey(ID) ? _outPorts[ID] : null;
        }

        public bool TryGetInPort(string fieldName, out Port port)
        {
            port = GetInPort(fieldName);
            return port != null;
        }
        
        public bool TryGetOutPort(string fieldName, out Port port)
        {
            port = GetOutPort(fieldName);
            return port != null;
        }

        public override void OnCreate()
        {
            //RefreshPorts();
            RegisterPorts();
        }

        public override void OnRefresh()
        {
            //RefreshPorts();
            RegisterPorts();
        }

        public virtual void RegisterPorts() { }

        private string GetPortID(string name, IDictionary dict)
        {
            var id = name;
            while (dict.Contains(id))
            {
                id += " ";
            }

            return id;
        }
        
        public void AddInPort<T>(string name)
        {
            var ID = GetPortID(name, _inPorts);
            _inPorts.Add(ID, new InPort<T>(this, ID));
        }

        public void AddOutPort<T>(string name, Func<T> getValue)
        {
            var ID = GetPortID(name, _outPorts);
            _outPorts.Add(ID, new OutPort<T>(this, ID, getValue));
        }

        public void AddFlowInPort()
        {
            
        }

        public void AddFlowOutPort()
        {
            
        }

        // private void UpdateLink()
        // {
        //     _updateLink = false;
        //     for (int i = outLinks.Count - 1; i >= 0; i--)
        //     {
        //         var snode = outLinks[i].sourceNode;
        //         var tnode = outLinks[i].targetNode;
        //         if (snode.TryGetOutPort(outLinks[i].sourceOutPort, out Port outPort) && tnode.TryGetInPort(outLinks[i].targetInPort, out Port inPort))
        //         {
        //             outPort.linkCount++;
        //             inPort.linkCount++;
        //         }
        //         else
        //         {
        //             graph.DelLink(outLinks[i]);
        //         }
        //     }
        // }

#if UNITY_EDITOR
        private class GUILink
        {
            public Node sourceNode;
            public Port sourcePort;
            public Node targetNode;
            public Port targetPort;

            public GUILink(Node sourceNode, Port sourcePort, Node targetNode = null, Port targetPort = null)
            {
                this.sourceNode = sourceNode;
                this.sourcePort = sourcePort;
                this.targetNode = targetNode;
                this.targetPort = targetPort;
            }
        }

        private static GUILink _clickLink;
        private static int _dragLinkMissNodeCount = 0;

        public override void DrawLinkGUI()
        {
            //------更新Link状态避免因修改Field而导致的Link失效------
            
            // if (_updateLink) UpdateLink();
            
            var e = Event.current;

            var yStart = rect.y + StyleUtils.windowTitle.CalcSize(title).y + 1;
            var portWidth = StyleUtils.portSymbol.CalcSize("●").x;
            
            // ------确定Port位置------

            int id = 0;
            foreach (var pair in _inPorts)
            {
                var portHeight = StyleUtils.inPortLabel.CalcSize(pair.Value.ID).y - 3;
                pair.Value.rect = new Rect(rect.x - portWidth, yStart + id * portHeight, portWidth, portHeight);
                ++id;
            }
            
            id = 0;
            foreach (var pair in _outPorts)
            {
                var portHeight = StyleUtils.inPortLabel.CalcSize(pair.Value.ID).y - 3;
                pair.Value.rect = new Rect(rect.x + rect.width, yStart + id * portHeight, portWidth, portHeight);
                ++id;
            }

            // ------处理鼠标抬起事件------
            
            if (_clickLink != null && e.type == EventType.MouseUp && e.button == 0)
            {
                bool mouseOnPort = false;
                if (_clickLink.sourcePort.IsInPort())
                {
                    foreach (var port in _outPorts.Select(pair => pair.Value).Where(port => port.rect.Contains(e.mousePosition)))
                    {
                        PortLink.Create(port, _clickLink.sourcePort);
                        mouseOnPort = true;
                        _clickLink = null;
                        e.Use();
                        break;
                    }
                }
                else if (_clickLink.sourcePort.IsOutPort())
                {
                    foreach (var port in _inPorts.Select(pair => pair.Value))
                    {
                        if (port.rect.Contains(e.mousePosition))
                        {
                            PortLink.Create(_clickLink.sourcePort, port);
                            mouseOnPort = true;
                            _clickLink = null;
                            e.Use();
                            break;
                        }
                    }
                }

                if (!mouseOnPort)
                {
                    _dragLinkMissNodeCount++;
                    if (_dragLinkMissNodeCount == graph.allNodes.Count)
                    {
                        if (_clickLink.targetNode != null && _clickLink.targetPort != null)
                        {
                            PortLink.Create(_clickLink.sourcePort, _clickLink.targetPort);
                        }

                        _clickLink = null;
                        e.Use();
                    }
                }
            }
            
            // ------绘制Port圆点------
            
            foreach (var pair in _inPorts)
            {
                DrawPortGUI(pair.Value, e);
            }
            
            foreach (var pair in _outPorts)
            {
                DrawPortGUI(pair.Value, e);
            }

            // ------绘制拖动的链接线------
            
            if (_clickLink != null)
            {
                GraphUtils.willRepaint = true;
                var texture = StyleUtils.LoadTexture2D("Textures/Bezier");
                Handles.DrawBezier(_clickLink.sourcePort.rect.center, e.mousePosition, _clickLink.sourcePort.rect.center, e.mousePosition, ColorUtils.orange1, texture, 3);
            }
            
            // ------绘制已经链接的线------

            foreach (var link in outLinks)
            {
                var portLink = link as PortLink;
                if (portLink != null)
                {
                    var sourcePort = portLink.sourcePort;
                    var targetPort = portLink.targetPort;
                    var height = GraphUtils.activeLink == link ? 5 : 3;
                    var texture = StyleUtils.LoadTexture2D("Textures/Bezier");
                    Handles.DrawBezier(sourcePort.rect.center, targetPort.rect.center, sourcePort.rect.center, targetPort.rect.center,
                        ColorUtils.orange1, texture, height);
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        if (CurveUtils.IsPosInCurve(e.mousePosition, sourcePort.rect.center, targetPort.rect.center, 5))
                        {
                            GraphUtils.Select(link);
                            e.Use();
                        }
                    }
                }
            }
        }

        private void DrawPortGUI(Port port, Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0 && port.rect.Contains(e.mousePosition))
            {
                if (!port.IsFullLink)
                {
                    GraphUtils.ClearSelect();
                    _dragLinkMissNodeCount = 0;
                    _clickLink = new GUILink(this, port);
                    e.Use();
                }
                else if (port.linkCount == 1)
                {
                    GraphUtils.ClearSelect();
                    _dragLinkMissNodeCount = 0;
                    foreach (var link in port.node.inLinks)
                    {
                        if (link is PortLink portLink)
                        {
                            if (portLink.targetPortId == port.fieldName)
                            {
                                _clickLink = new GUILink(link.sourceNode, portLink.sourcePort, link.targetNode, portLink.targetPort);
                                graph.DelLink(link);
                                e.Use();
                                break;
                            }
                        }
                    }
                }
            }
            
            if (port.rect.Contains(e.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(port.rect, MouseCursor.ArrowPlus);
            }
            
            if (_clickLink != null && _clickLink.sourcePort == port && _clickLink.sourceNode == this ||
                port.rect.Contains(e.mousePosition) ||
                port.IsLink)
            {
                GUI.color = ColorUtils.orange1;
                EditorGUI.LabelField(port.rect, "●", StyleUtils.portSymbol);
                GUI.color = Color.white;
            }
            else
            {
                EditorGUI.LabelField(port.rect, "●", StyleUtils.portSymbol);
            }
        }

        public override void DrawWindowGUI()
        {
            DrawPorts(_inPorts.Values.ToList(), _outPorts.Values.ToList());
        }
#endif
    }
}