using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Nirvana
{
    public abstract class FlowNode : Node
    {
        private Port[] _orderInPorts;
        private Port[] _orderOutPorts;
        
        public override void OnCreate()
        {
            RegisterPorts();
            RefreshEditorPort();
        }

        public override void OnRefresh()
        {
            RegisterPorts();
            RefreshLinks();
            RefreshEditorPort();
        }

        protected abstract void RegisterPorts();

        public abstract void Execute();

        private string GetPortID(string name, IDictionary dict)
        {
            var id = name;
            while (dict.Contains(id))
            {
                id += " ";
            }

            return id;
        }

        public InPort<T> AddInPort<T>(string name)
        {
            var ID = GetPortID(name, inPorts);
            var newPort = new InPort<T>(this, ID, name);
            inPorts.Add(ID, newPort);
            return newPort;
        }

        public OutPort<T> AddOutPort<T>(string name, Func<T> getValue)
        {
            var ID = GetPortID(name, outPorts);
            var newPort = new OutPort<T>(this, ID, name, getValue);
            outPorts.Add(ID, newPort);
            return newPort;
        }

        public FlowInPort AddFlowInPort(string name ,Action flowFunc)
        {
            var ID = GetPortID(name, inPorts);
            var newPort = new FlowInPort(this, ID, name, flowFunc);
            inPorts.Add(ID, newPort);
            return newPort;
        }

        public FlowOutPort AddFlowOutPort(string name)
        {
            var ID = GetPortID(name, outPorts);
            var newPort = new FlowOutPort(this, ID, name);
            outPorts.Add(ID, newPort);
            return newPort;
        }

        private void RefreshLinks()
        {
            foreach (var link in outLinks)
            {
                link.RefreshSourcePort();
            }
            
            foreach (var link in inLinks)
            {
                link.RefreshTargetPort();
            }
        }
        
        private void RefreshEditorPort()
        {
#if UNITY_EDITOR
            _orderInPorts = inPorts.Values.OrderBy(p => p is FlowInPort ? 0 : 1).ToArray();
            _orderOutPorts = outPorts.Values.OrderBy(p => p is FlowOutPort ? 0 : 1).ToArray();
#endif
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
            foreach (var port in _orderInPorts)
            {
                var portHeight = StyleUtils.inPortLabel.CalcSize(port.ID).y - 3;
                port.rect = new Rect(rect.x - portWidth, yStart + id * portHeight, portWidth, portHeight);
                ++id;
            }

            id = 0;
            foreach (var port in _orderOutPorts)
            {
                var portHeight = StyleUtils.inPortLabel.CalcSize(port.ID).y - 3;
                port.rect = new Rect(rect.x + rect.width, yStart + id * portHeight, portWidth, portHeight);
                ++id;
            }

            // ------处理鼠标抬起事件------

            if (_clickLink != null && e.type == EventType.MouseUp && e.button == 0)
            {
                bool mouseOnPort = false;
                if (_clickLink.sourcePort.IsInPort())
                {
                    foreach (var port in _orderOutPorts)
                    {
                        if (port.rect.Contains(e.mousePosition))
                        {
                            graph.AddLink(port, _clickLink.sourcePort);
                            mouseOnPort = true;
                            _clickLink = null;
                            e.Use();
                            break;
                        }
                    }
                }
                else if (_clickLink.sourcePort.IsOutPort())
                {
                    foreach (var port in _orderInPorts)
                    {
                        if (port.rect.Contains(e.mousePosition))
                        {
                            graph.AddLink(_clickLink.sourcePort, port);
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
                        if (_clickLink.targetPort != null)
                        {
                            graph.AddLink(_clickLink.sourcePort, _clickLink.targetPort);
                        }

                        _clickLink = null;
                        e.Use();
                    }
                }
            }

            // ------绘制拖动的链接线------

            if (_clickLink != null)
            {
                GraphUtils.willRepaint = true;
                var texture = StyleUtils.LoadTexture2D("Textures/Bezier");
                Handles.DrawBezier(_clickLink.sourcePort.rect.center, e.mousePosition, _clickLink.sourcePort.rect.center, e.mousePosition,
                    ColorUtils.orange1, texture, 3);
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
                    Handles.DrawBezier(sourcePort.rect.center, targetPort.rect.center, sourcePort.rect.center, targetPort.rect.center, ColorUtils.orange1, texture, height);
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
            
            // ------绘制Port圆点------

            foreach (var port in _orderInPorts)
            {
                DrawPortGUI(port, e);
            }

            foreach (var port in _orderOutPorts)
            {
                DrawPortGUI(port, e);
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
                    var link = port.node.GetInLink(port);
                    if (link != null)
                    {
                        _clickLink = new GUILink(link.sourcePort, link.targetPort);
                        graph.DelLink(link);
                        e.Use();
                    }
                }
            }

            if (port.rect.Contains(e.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(port.rect, MouseCursor.ArrowPlus);
                var content = port.type.ToString();
                var width = StyleUtils.portTipBox.CalcWidth(content);
                var tipPort = port.rect;
                tipPort.x -= port.IsInPort() ? width : -port.rect.width;
                tipPort.width = width;
                EditorUtils.DrawBox(tipPort, ColorUtils.orange1, StyleUtils.portTipBox);
                GUI.color = Color.black;
                EditorGUI.LabelField(tipPort, content);
                GUI.color = Color.white;
            }

            if (port.isLink || port.rect.Contains(e.mousePosition))
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
            int minL = Mathf.Min(_orderInPorts.Length, _orderOutPorts.Length);
            for (int i = 0; i < minL; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(2);
                GUILayout.Label(_orderInPorts[i].ID, StyleUtils.inPortLabel);
                GUILayout.FlexibleSpace();
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                GUILayout.Label(_orderOutPorts[i].ID, StyleUtils.outPortLabel);
                GUILayout.Space(2);
                GUILayout.EndHorizontal();
            }

            for (int i = minL; i < _orderInPorts.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(2);
                GUILayout.Label(_orderInPorts[i].ID, StyleUtils.inPortLabel);
                GUILayout.EndHorizontal();
            }
            
            for (int i = minL; i < _orderOutPorts.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_orderOutPorts[i].ID, StyleUtils.outPortLabel);
                GUILayout.Space(2);
                GUILayout.EndHorizontal();
            }
        }

        private static bool _flowInPortHeaderGroup = true;
        
        public override void DrawInspectorGUI()
        {
            base.DrawInspectorGUI();
            
            _flowInPortHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_flowInPortHeaderGroup, "InPorts");
            if (_flowInPortHeaderGroup)
            {
                foreach (var port in _orderInPorts)
                {
                    if (port is not FlowInPort)
                    {
                        if (port is InPort inPort)
                        {
                            if (inPort.isLink)
                            {
                                var link = inPort.node.GetInLink(inPort);
                                if (link != null)
                                {
                                    EditorGUILayout.LabelField(inPort.name + $" Link To [{link.sourceNode.title}.{link.sourcePortId}]");
                                }
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                var value = EditorUtils.TypeField(inPort.name, inPort.GetObjectValue(), inPort.type);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    inPort.SetObjectValue(value);
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
#endif
    }
}