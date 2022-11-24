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
                    _inPorts.Add(port);
                }
                else if (info.TryGetAttribute<OutPortAttribute>(out var outAtt))
                {
                    var portName = string.IsNullOrEmpty(outAtt.name) ? info.Name : outAtt.name;
                    var port = new Port {name = portName, type = info.FieldType, order = outAtt.order};
                    _ports.Add(port);
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
        public override void DrawLinkGUI()
        {
            var e = Event.current;
            
            float singleLineHeight = UnityEditor.EditorGUIUtility.singleLineHeight;
            float yStart = rect.y + StyleUtils.windowTitle.CalcSize(title).y;
            float portWidth = StyleUtils.portSymbol.CalcSize("●").x;
            
            for (int i = 0; i < _inPorts.Count; i++)
            {
                var portRect = new Rect(rect.x - portWidth - 2, yStart + i * singleLineHeight, portWidth, singleLineHeight);
                if (portRect.Contains(e.mousePosition))
                {
                    UnityEditor.EditorGUIUtility.AddCursorRect(portRect, UnityEditor.MouseCursor.ArrowPlus);
                    GUI.color = Color.cyan;
                    UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
                    GUI.color = Color.white;
                }
                else
                {
                    UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
                }
            }
            
            for (int i = 0; i < _outPorts.Count; i++)
            {
                var portRect = new Rect(rect.x + rect.width + 4, yStart + i * singleLineHeight, portWidth, singleLineHeight);
                if (portRect.Contains(e.mousePosition))
                {
                    UnityEditor.EditorGUIUtility.AddCursorRect(portRect, UnityEditor.MouseCursor.ArrowPlus);
                    GUI.color = Color.cyan;
                    UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
                    GUI.color = Color.white;
                }
                else
                {
                    UnityEditor.EditorGUI.LabelField(portRect, "●", StyleUtils.portSymbol);
                }
            }
        }

        public override void DrawWindowGUI()
        {
            DrawPorts(inPorts, outPorts);
        }
#endif
    }
}