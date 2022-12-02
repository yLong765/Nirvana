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
    public abstract partial class Node
    {
        private int _id;
        
        [JsonIgnore] public int ID
        {
            get => _id;
            set => _id = value;
        }
        
        private static bool _nodeInspectorHeaderGroup = true;
        
        private List<FieldInfo> _fieldInfos = new List<FieldInfo>();

        protected List<FieldInfo> GetAllFieldInfos()
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
    }
}
#endif