#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public partial class Node
    {
        private static bool _nodeInspectorHeaderGroup = true;
        
        private List<FieldInfo> _fieldInfos = new List<FieldInfo>();

        private List<FieldInfo> GetAllFieldInfos()
        {
            if (_fieldInfos.Count == 0)
            {
                _fieldInfos = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            }

            return _fieldInfos;
        }

        [JsonIgnore] public bool isSelected => GraphUtils.activeNodes.Contains(this);

        public virtual void DrawInspectorGUI()
        {
            tag = EditorGUILayout.TextField(tag);
            EditorUtils.DefaultTextField(tag, "Tag...");
            
            _nodeInspectorHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(_nodeInspectorHeaderGroup, "Fields");
            if (_nodeInspectorHeaderGroup)
            {
                var fields = GetAllFieldInfos();
                foreach (var info in fields.Where(info => !info.HasAttribute<IgnoreInNodeInspectorAttribute>()))
                {
                    EditorGUI.BeginChangeCheck();
                    var value = EditorUtils.TypeField(info.Name, info.GetValue(this), info.FieldType);
                    if (EditorGUI.EndChangeCheck())
                    {
                        info.SetValue(this, value);
                    }
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public virtual void DrawWindowGUI() { }

        public virtual void DrawLinkGUI() { }

        protected static void DrawPorts(List<Port> inPorts, List<Port> outPorts)
        {
            int minL = Mathf.Min(inPorts.Count, outPorts.Count);
            for (int i = 0; i < minL; i++)
            {
                DrawInOutPort(inPorts[i].name, outPorts[i].name);
            }

            for (int i = minL; i < inPorts.Count; i++)
            {
                DrawInPort(inPorts[i].name);
            }
            
            for (int i = minL; i < outPorts.Count; i++)
            {
                DrawOutPort(outPorts[i].name);
            }
        }

        protected static void DrawInOutPort(string inName, string outName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label(inName, StyleUtils.inPortLabel);
            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            GUILayout.FlexibleSpace();
            GUILayout.Label(outName, StyleUtils.outPortLabel);
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
        }
        
        protected static void DrawInPort(string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(2);
            GUILayout.Label(name, StyleUtils.inPortLabel);
            GUILayout.EndHorizontal();
        }

        protected static void DrawOutPort(string name)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, StyleUtils.outPortLabel);
            GUILayout.Space(2);
            GUILayout.EndHorizontal();
        }
    }
}
#endif