using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public class FlowNode : Node
    {
        [InPort("▶", order = 0), IgnoreInNodeInspector] public Port InPort;
        [OutPort("▶", order = 0), IgnoreInNodeInspector] public Port OutPort;

        private List<Port> _ports = new List<Port>();
        private List<Port> _inPorts = new List<Port>();
        private List<Port> _outPorts = new List<Port>();
        
        [JsonIgnore] public List<Port> allPorts => _ports;
        [JsonIgnore] public List<Port> inPorts => _inPorts;
        [JsonIgnore] public List<Port> outPorts => _outPorts;

        private Port GetPort(string fieldName)
        {
            return _ports.FirstOrDefault(port => port.fieldName == fieldName);
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
                    var port = Port.Create(this, portName, info.Name, info.FieldType, inAtt.order);
                    _ports.Add(port);
                    _inPorts.Add(port);
                }
                else if (info.TryGetAttribute<OutPortAttribute>(out var outAtt))
                {
                    var portName = string.IsNullOrEmpty(outAtt.name) ? info.Name : outAtt.name;
                    var port = Port.Create(this, portName, info.Name, info.FieldType, outAtt.order);
                    _ports.Add(port);
                    _outPorts.Add(port);
                }
            }
            
            _ports.Sort(ComparerPort);
            _inPorts.Sort(ComparerPort);
            _outPorts.Sort(ComparerPort);

            //------更新PortLink------
            
            for (int i = inLinks.Count - 1; i >= 0; i--)
            {
                if (GetPort(inLinks[i].sourcePort) == null)
                {
                    DelOutLink(inLinks[i]);
                }
            }
        }
        
        public override void OnCreate()
        {
            RefreshPorts();
        }

        public override void OnRefresh()
        {
            RefreshPorts();
        }

#if UNITY_EDITOR
        private class GUIPort
        {
            public Node node;
            public Port port;

            public GUIPort(Node node, Port port)
            {
                this.node = node;
                this.port = port;
            }
        }
        private static GUIPort _clickPort;
        private static int _dragLinkMissNodeCount = 0;
        
        public override void DrawLinkGUI()
        {
            var e = Event.current;

            float yStart = rect.y + StyleUtils.windowTitle.CalcSize(title).y + 1;
            float portWidth = StyleUtils.portSymbol.CalcSize("●").x;
            
            // ------确定Port位置------
            
            for (int i = 0; i < _inPorts.Count; i++)
            {
                float portHeight = StyleUtils.inPortLabel.CalcSize(_inPorts[i].name).y - 3;
                _inPorts[i].rect = new Rect(rect.x - portWidth, yStart + i * portHeight, portWidth, portHeight);
            }
            
            for (int i = 0; i < _outPorts.Count; i++)
            {
                float portHeight = StyleUtils.outPortLabel.CalcSize(_outPorts[i].name).y - 3;
                _outPorts[i].rect = new Rect(rect.x + rect.width, yStart + i * portHeight, portWidth, portHeight);
            }

            // ------处理鼠标抬起事件------
            
            if (_clickPort != null && e.type == EventType.MouseUp && e.button == 0)
            {
                bool mouseOnPort = false;
                foreach (var port in _inPorts)
                {
                    if (port.rect.Contains(e.mousePosition))
                    {
                        Debug.Log("Add Link " + _clickPort.port.fieldName + " to " + port.fieldName);
                        graph.AddLink(_clickPort.node, this, _clickPort.port.fieldName, port.fieldName);
                        mouseOnPort = true;
                        _clickPort = null;
                        e.Use();
                    }
                }

                if (!mouseOnPort)
                {
                    _dragLinkMissNodeCount++;
                    if (_dragLinkMissNodeCount == graph.allNodes.Count)
                    {
                        _clickPort = null;
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
            
            if (_clickPort != null)
            {
                GraphUtils.willRepaint = true;
                UnityEditor.Handles.DrawBezier(_clickPort.port.rect.center, e.mousePosition, _clickPort.port.rect.center, e.mousePosition, ColorUtils.orange1, Texture2D.whiteTexture, 3);
            }
            
            // ------绘制已经链接的线------

            foreach (var link in inLinks)
            {
                Port sourcePort = null;
                Port targetPort = null;
                if (link.sourceNode is FlowNode snode)
                {
                    sourcePort = snode.GetPort(link.sourcePort);
                }

                if (link.targetNode is FlowNode tnode)
                {
                    targetPort = tnode.GetPort(link.targetPort);
                }

                if (sourcePort != null && targetPort != null)
                {
                    sourcePort.isLink = true;
                    targetPort.isLink = true;
                    UnityEditor.Handles.DrawBezier(sourcePort.rect.center, targetPort.rect.center, sourcePort.rect.center, targetPort.rect.center, ColorUtils.orange1, Texture2D.whiteTexture, 3);
                }
            }
        }

        private void DrawPortGUI(Port port, Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0 && port.rect.Contains(e.mousePosition))
            {
                GraphUtils.activeNodes = null;
                _dragLinkMissNodeCount = 0;
                _clickPort = new GUIPort(this, port);
                e.Use();
            }
            
            if (port.rect.Contains(e.mousePosition))
            {
                UnityEditor.EditorGUIUtility.AddCursorRect(port.rect, UnityEditor.MouseCursor.ArrowPlus);
            }
            
            if (_clickPort != null && _clickPort.port.fieldName == port.fieldName && _clickPort.node == this ||
                port.rect.Contains(e.mousePosition) ||
                port.isLink)
            {
                GUI.color = ColorUtils.orange1;
                UnityEditor.EditorGUI.LabelField(port.rect, "●", StyleUtils.portSymbol);
                GUI.color = Color.white;
            }
            else
            {
                UnityEditor.EditorGUI.LabelField(port.rect, "●", StyleUtils.portSymbol);
            }
        }

        public override void DrawWindowGUI()
        {
            DrawPorts(inPorts, outPorts);
        }
#endif
    }
}