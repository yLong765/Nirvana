#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Nirvana.Attributes;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public partial class Node
    {
        [JsonIgnore] public bool isSelected => GraphUtils.activeNodes.Contains(this);

        public virtual void DrawInspectorGUI()
        {
            tag = EditorGUILayout.TextField(tag);
            EditorUtils.DefaultTextField(tag, "Tag...");

            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in fields)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorUtils.TypeField(info.Name, info.GetValue(this), info.FieldType);
                if (EditorGUI.EndChangeCheck())
                {
                    info.SetValue(this, value);
                }
            }
        }

        public virtual void DrawNodeGUI()
        {
            List<FieldInfo> inPorts = new List<FieldInfo>();
            List<FieldInfo> outPorts = new List<FieldInfo>();
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in fields)
            {
                var inPortAtt = info.GetAttribute<InPortAttribute>();
                if (inPortAtt != null)
                {
                    inPorts.Add(info);
                }
                else
                {
                    var outPortAtt = info.GetAttribute<OutPortAttribute>();
                    if (outPortAtt != null)
                    {
                        outPorts.Add(info);
                    }
                }
            }
            
            DrawPorts(inPorts, outPorts);
        }

        private void DrawPorts(List<FieldInfo> inPorts, List<FieldInfo> outPorts)
        {

        }

        private void DrawInPort(string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(name, StyleUtils.inPortLabel);
            GUILayout.EndHorizontal();
            var lastRect = GUILayoutUtility.GetLastRect();
            var width = StyleUtils.portSymbol.CalcSize("●").x;
            lastRect.width = width;
            EditorGUI.LabelField(lastRect, "●", StyleUtils.portSymbol);
        }

        private void DrawOutPort(string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, StyleUtils.outPortLabel);
            GUILayout.Space(13);
            GUILayout.EndHorizontal();
            var lastRect = GUILayoutUtility.GetLastRect();
            var width = StyleUtils.portSymbol.CalcSize("●").x;
            lastRect.x = lastRect.width - width;
            lastRect.width = width;
            EditorGUI.LabelField(lastRect, "●", StyleUtils.portSymbol);
        }
    }
}
#endif