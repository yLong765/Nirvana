using System.Collections;
using System.Collections.Generic;
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
                    var port = new Port {name = portName, type = info.FieldType, order = inAtt.order};
                    _ports.Add(port);
                    port.ID = _ports.Count;
                    _inPorts.Add(port);
                }
                else if (info.TryGetAttribute<OutPortAttribute>(out var outAtt))
                {
                    var portName = string.IsNullOrEmpty(outAtt.name) ? info.Name : outAtt.name;
                    var port = new Port {name = portName, type = info.FieldType, order = outAtt.order};
                    _ports.Add(port);
                    port.ID = _ports.Count;
                    _outPorts.Add(port);
                }
            }
            
            _ports.Sort(ComparerPort);
            _inPorts.Sort(ComparerPort);
            _outPorts.Sort(ComparerPort);
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

        private static bool _clickPort = false;
        private static bool _mouseInPort = false;
        private static Vector3 _clickPortPos = default;
        private int _clickPortId = 0;
        
        public override void DrawLinkGUI()
        {
            var e = Event.current;

            float yStart = rect.y + StyleUtils.windowTitle.CalcSize(title).y + 1;
            float portWidth = StyleUtils.portSymbol.CalcSize("●").x;
            
            for (int i = 0; i < _inPorts.Count; i++)
            {
                float portHeight = StyleUtils.inPortLabel.CalcSize(_inPorts[i].name).y - 3;
                var portRect = new Rect(rect.x - portWidth, yStart + i * portHeight, portWidth, portHeight);
                DrawPortGUI(portRect, _inPorts[i], e);
            }
            
            for (int i = 0; i < _outPorts.Count; i++)
            {
                float portHeight = StyleUtils.outPortLabel.CalcSize(_outPorts[i].name).y - 3;
                var portRect = new Rect(rect.x + rect.width, yStart + i * portHeight, portWidth, portHeight);
                DrawPortGUI(portRect, _outPorts[i], e);
            }
            
            if (_clickPort)
            {
                GraphUtils.willRepaint = true;
                UnityEditor.Handles.DrawBezier(_clickPortPos, e.mousePosition, _clickPortPos, e.mousePosition, ColorUtils.orange1, Texture2D.whiteTexture, 3);
            }

            // if (_clickPort && e.type == EventType.MouseUp && e.button == 0)
            // {
            //     _clickPort = false;
            //     _clickPortPos = default;
            //     _clickPortId = 0;
            //     e.Use();
            // }
        }

        private void DrawPortGUI(Rect portRect, Port port, Event e)
        {
            if (portRect.Contains(e.mousePosition))
            {
                if (_clickPort && e.type == EventType.MouseUp && e.button == 0)
                {
                    Debug.Log(port.name);
                    _clickPort = false;
                    _clickPortPos = default;
                    _clickPortId = 0;
                    e.Use();
                }
            }
            else
            {
                if (_clickPort && e.type == EventType.MouseUp && e.button == 0)
                {
                    Debug.Log(port.name + " 11");
                    _clickPort = false;
                    _clickPortPos = default;
                    _clickPortId = 0;
                }
            }
            
            if (e.type == EventType.MouseDown && e.button == 0 && portRect.Contains(e.mousePosition))
            {
                Debug.Log(portRect);
                Debug.Log(e.mousePosition);
                _clickPort = true;
                _clickPortPos = portRect.center;
                _clickPortId = port.ID;
                e.Use();
            }
            
            if (portRect.Contains(e.mousePosition))
            {
                _mouseInPort = true;
                UnityEditor.EditorGUIUtility.AddCursorRect(portRect, UnityEditor.MouseCursor.ArrowPlus);
            }
            else
            {
                _mouseInPort |= false;
            }
            
            if (_clickPort && _clickPortId == port.ID || portRect.Contains(e.mousePosition))
            {
                GUI.color = ColorUtils.orange1;
                UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
                GUI.color = Color.white;
            }
            else
            {
                UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
            }
        }

        public override void DrawWindowGUI()
        {
            DrawPorts(inPorts, outPorts);
        }
#endif
    }
}