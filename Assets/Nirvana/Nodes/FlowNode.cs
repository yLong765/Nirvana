using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nirvana
{
    public abstract class FlowNode : Node
    {
        private Dictionary<string, Port> _inPorts = new Dictionary<string, Port>();
        private Dictionary<string, Port> _outPorts = new Dictionary<string, Port>();

        public Dictionary<string, Port> inPorts => _inPorts;
        public Dictionary<string, Port> outPorts => _outPorts;

        public override void OnCreate()
        {
            RegisterPorts();
        }

        protected abstract void RegisterPorts();

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
            _inPorts.Add(ID, new InPort<T>(this, ID, name));
        }

        public void AddOutPort<T>(string name, Func<T> getValue)
        {
            var ID = GetPortID(name, _outPorts);
            _outPorts.Add(ID, new OutPort<T>(this, ID, name, getValue));
        }

        public void AddFlowInPort(string name)
        {
            var ID = GetPortID(name, _inPorts);
            _inPorts.Add(ID, new FlowInPort(this, ID, name));
        }

        public void AddFlowOutPort(string name)
        {
            var ID = GetPortID(name, _outPorts);
            _outPorts.Add(ID, new FlowOutPort(this, ID, name));
        }
        
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

#if UNITY_EDITOR
        private class GUILink
        {
            public Port sourcePort;
            public Port targetPort;
            public Node sourceNode => sourcePort.node;
            public Node targetNode => targetPort?.node;

            public GUILink(Port sourcePort, Port targetPort = null)
            {
                this.sourcePort = sourcePort;
                this.targetPort = targetPort;
            }
        }

        private static GUILink _clickLink;
        private static int _dragLinkMissNodeCount = 0;

        public override void DrawLinkGUI()
        {
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
                        graph.AddLink(port, _clickLink.sourcePort);
                        mouseOnPort = true;
                        _clickLink = null;
                        e.Use();
                        break;
                    }
                }
                else if (_clickLink.sourcePort.IsOutPort())
                {
                    foreach (var port in _inPorts.Select(pair => pair.Value).Where(port => port.rect.Contains(e.mousePosition)))
                    {
                        graph.AddLink(_clickLink.sourcePort, port);
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
                            graph.AddLink(_clickLink.sourcePort, _clickLink.targetPort);
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
                if (link != null)
                {
                    var sourcePort = link.sourcePort;
                    var targetPort = link.targetPort;
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
                if (!port.isFullLink)
                {
                    GraphUtils.ClearSelect();
                    _dragLinkMissNodeCount = 0;
                    _clickLink = new GUILink(port);
                    e.Use();
                }
                else if (port.linkCount == 1)
                {
                    GraphUtils.ClearSelect();
                    _dragLinkMissNodeCount = 0;
                    foreach (var link in port.node.inLinks)
                    {
                        if (link.targetPortId == port.ID)
                        {
                            _clickLink = new GUILink(link.sourcePort, link.targetPort);
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
            
            if (_clickLink != null && _clickLink.sourcePort == port && _clickLink.sourceNode == this ||
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
            DrawPorts(_inPorts.Values.ToList(), _outPorts.Values.ToList());
        }
#endif
    }
}