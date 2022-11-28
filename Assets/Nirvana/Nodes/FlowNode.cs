using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nirvana
{
    public class FlowNode : Node
    {
        [InPort("▶", order = 0, canDragLink = true), IgnoreInNodeInspector] public Port InPort;
        [OutPort("▶", order = 0), IgnoreInNodeInspector] public Port OutPort;

        private List<Port> _ports = new List<Port>();
        private List<Port> _inPorts = new List<Port>();
        private List<Port> _outPorts = new List<Port>();
        
        [JsonIgnore] public List<Port> allPorts => _ports;
        [JsonIgnore] public List<Port> inPorts => _inPorts;
        [JsonIgnore] public List<Port> outPorts => _outPorts;

        private bool _updateLink = false;

        public Port GetPort(string fieldName)
        {
            return _ports.FirstOrDefault(port => port.fieldName == fieldName);
        }

        public Port GetInPort(string fieldName)
        {
            return _inPorts.FirstOrDefault(port => port.fieldName == fieldName);
        }
        
        public Port GetOutPort(string fieldName)
        {
            return _outPorts.FirstOrDefault(port => port.fieldName == fieldName);
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

        private static int ComparerPort(Port p1, Port p2)
        {
            return p1.order < p2.order ? -1 : p1.order == p2.order ? 0 : 1;
        }
        
        private void RefreshPorts()
        {
            _ports.Clear();
            _inPorts.Clear();
            _outPorts.Clear();
            var fields = GetType().GetAllFields();
            foreach (var info in fields)
            {
                if (info.TryGetAttribute<InPortAttribute>(out var inAtt))
                {
                    var portName = string.IsNullOrEmpty(inAtt.name) ? info.Name : inAtt.name;
                    var port = Port.Create(this, portName, info.Name, info.FieldType, inAtt, PortType.In);
                    _ports.Add(port);
                    _inPorts.Add(port);
                }
                else if (info.TryGetAttribute<OutPortAttribute>(out var outAtt))
                {
                    var portName = string.IsNullOrEmpty(outAtt.name) ? info.Name : outAtt.name;
                    var port = Port.Create(this, portName, info.Name, info.FieldType, outAtt, PortType.Out);
                    _ports.Add(port);
                    _outPorts.Add(port);
                }
            }
            
            _ports.Sort(ComparerPort);
            _inPorts.Sort(ComparerPort);
            _outPorts.Sort(ComparerPort);

            _updateLink = true;
        }
        
        
        public override void OnCreate()
        {
            RefreshPorts();
        }

        public override void OnRefresh()
        {
            RefreshPorts();
        }

        private void UpdateLink()
        {
            _updateLink = false;
            for (int i = outLinks.Count - 1; i >= 0; i--)
            {
                var snode = outLinks[i].sourceNode;
                var tnode = outLinks[i].targetNode;
                if (snode.TryGetOutPort(outLinks[i].sourceOutPort, out Port outPort) && tnode.TryGetInPort(outLinks[i].targetInPort, out Port inPort))
                {
                    outPort.linkCount++;
                    inPort.linkCount++;
                }
                else
                {
                    graph.DelLink(outLinks[i]);
                }
            }
        }

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
            
            if (_updateLink) UpdateLink();
            
            var e = Event.current;

            var yStart = rect.y + StyleUtils.windowTitle.CalcSize(title).y + 1;
            var portWidth = StyleUtils.portSymbol.CalcSize("●").x;
            
            // ------确定Port位置------
            
            for (int i = 0; i < _inPorts.Count; i++)
            {
                var portHeight = StyleUtils.inPortLabel.CalcSize(_inPorts[i].name).y - 3;
                _inPorts[i].rect = new Rect(rect.x - portWidth, yStart + i * portHeight, portWidth, portHeight);
            }
            
            for (int i = 0; i < _outPorts.Count; i++)
            {
                var portHeight = StyleUtils.outPortLabel.CalcSize(_outPorts[i].name).y - 3;
                _outPorts[i].rect = new Rect(rect.x + rect.width, yStart + i * portHeight, portWidth, portHeight);
            }

            // ------链接警告提示------

            if (_clickLink != null)
            {
                foreach (var port in _inPorts.Where(port => port.rect.Contains(e.mousePosition)))
                {
                    switch (port.portType)
                    {
                        case PortType.In:
                            if (!graph.CheckCanLink(_clickLink.sourceNode, this, _clickLink.sourcePort.fieldName, port.fieldName))
                            {
                                var size = StyleUtils.portWarningLabel.CalcSize("E: Non-identical or inherited types");
                                var warning = new Rect(0, 0, size.x, size.y);
                                warning.center = new Vector2(e.mousePosition.x - size.x * 0.5f, e.mousePosition.y - size.y);
                                EditorGUI.LabelField(warning, "E: Non-identical or inherited types", StyleUtils.portWarningLabel);
                            }
                            break;
                        case PortType.Out:
                            if (!graph.CheckCanLink(this, _clickLink.sourceNode, port.fieldName, _clickLink.sourcePort.fieldName))
                            {
                                var size = StyleUtils.portWarningLabel.CalcSize("E: Non-identical or inherited types");
                                var warning = new Rect(0, 0, size.x, size.y);
                                warning.center = new Vector2(e.mousePosition.x + size.x * 0.5f, e.mousePosition.y - size.y);
                                EditorGUI.LabelField(warning, "E: Non-identical or inherited types", StyleUtils.portWarningLabel);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            // ------处理鼠标抬起事件------
            
            if (_clickLink != null && e.type == EventType.MouseUp && e.button == 0)
            {
                bool mouseOnPort = false;
                foreach (var port in _ports.Where(port => port.rect.Contains(e.mousePosition)))
                {
                    Link newLink = null;
                    if (port.portType == PortType.In && _clickLink.sourcePort.portType == PortType.Out)
                    {
                        newLink = graph.AddLink(_clickLink.sourceNode, this, _clickLink.sourcePort.fieldName, port.fieldName);
                    }
                    else if (port.portType == PortType.Out && _clickLink.sourcePort.portType == PortType.In)
                    {
                        newLink = graph.AddLink(this, _clickLink.sourceNode, port.fieldName, _clickLink.sourcePort.fieldName);
                    }

                    if (newLink != null)
                    {
                        mouseOnPort = true;
                        _clickLink = null;
                        e.Use();
                        break;
                    }
                }

                if (!mouseOnPort)
                {
                    _dragLinkMissNodeCount++;
                    if (_dragLinkMissNodeCount == graph.allNodes.Count)
                    {
                        if (_clickLink.targetNode != null && _clickLink.targetPort != null)
                        {
                            graph.AddLink(_clickLink.sourceNode, _clickLink.targetNode, _clickLink.sourcePort.fieldName, _clickLink.targetPort.fieldName);
                        }
                        _clickLink = null;
                        e.Use();
                    }
                }
            }

            // ------绘制Port圆点------
            
            foreach (var port in _ports)
            {
                DrawPortGUI(port, e);
            }
            
            // ------绘制拖动的链接线------
            
            if (_clickLink != null)
            {
                GraphUtils.willRepaint = true;
                Handles.DrawBezier(_clickLink.sourcePort.rect.center, e.mousePosition, _clickLink.sourcePort.rect.center, e.mousePosition, ColorUtils.orange1, Texture2D.whiteTexture, 3);
            }
            
            // ------绘制已经链接的线------

            foreach (var link in outLinks)
            {
                var sourcePort = link.GetSourceOutPort();
                var targetPort = link.GetTargetInPort();
                var height = GraphUtils.activeLink == link ? 5 : 3;
                Handles.DrawBezier(sourcePort.rect.center, targetPort.rect.center, sourcePort.rect.center, targetPort.rect.center, ColorUtils.orange1, Texture2D.whiteTexture, height);
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    if (CurveUtils.IsPosInCurve(e.mousePosition, sourcePort.rect.center, targetPort.rect.center, 3))
                    {
                        GraphUtils.activeLink = link;
                        e.Use();
                    }
                }
            }
        }

        private void DrawPortGUI(Port port, Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0 && port.rect.Contains(e.mousePosition))
            {
                if (port.canDragLink && port.linkCount < port.maxLinkCount)
                {
                    GraphUtils.ClearGraphMouseSelect();
                    _dragLinkMissNodeCount = 0;
                    _clickLink = new GUILink(this, port);
                    e.Use();
                }
                else if (port.linkCount == 1)
                {
                    GraphUtils.ClearGraphMouseSelect();
                    _dragLinkMissNodeCount = 0;
                    foreach (var link in port.node.inLinks)
                    {
                        if (link.targetInPort == port.fieldName)
                        {
                            var outPort = link.GetSourceOutPort();
                            var inPort = link.GetTargetInPort();
                            _clickLink = new GUILink(link.sourceNode, outPort, link.targetNode, inPort);
                            graph.DelLink(link);
                            e.Use();
                            break;
                        }
                    }
                }
            }
            
            if (port.rect.Contains(e.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(port.rect, MouseCursor.ArrowPlus);
            }
            
            if (_clickLink != null && _clickLink.sourcePort.fieldName == port.fieldName && _clickLink.sourceNode == this ||
                port.rect.Contains(e.mousePosition) ||
                port.isLink)
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
            DrawPorts(inPorts, outPorts);
        }
#endif
    }
}