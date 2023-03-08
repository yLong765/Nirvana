#if UNITY_EDITOR
using System;
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
        /// <summary>
        /// Node最小Size
        /// </summary>
        public static Vector2 MIN_SIZE = new(80, 38);
        private static bool _nodeInspectorHeaderGroup = true;
        
        private int _id;
        private string _title;
        private string _tag;
        private Vector2 _position;
        private Vector2 _size = MIN_SIZE;
        private bool _isOverwriteOnGraphStartMethod = false;
        
        /// <summary>
        /// Editor.Id
        /// </summary>
        [JsonIgnore] public int ID
        {
            get => _id;
            set => _id = value;
        }
        
        /// <summary>
        /// Editor.标题
        /// </summary>
        public string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    
                    if (GetType().TryGetAttribute<TitleAttribute>(out var titleAttribute))
                    {
                        _title = titleAttribute.title;
                    }
                    else
                    {
                        _title = GetType().Name;
                        if (_title.EndsWith("Node"))
                        {
                            _title = _title[.._title.LastIndexOf("Node", StringComparison.Ordinal)];
                        }

                        if (_isOverwriteOnGraphStartMethod)
                        {
                            _title = $"➦ {_title}";
                        }
                    }
                }

                return _title;
            }
        }

        /// <summary>
        /// Editor.标签
        /// </summary>
        public string tag
        {
            get => _tag;
            set => _tag = value;
        }
        
        /// <summary>
        /// Graph中的位置
        /// </summary>
        [JsonIgnore] public Vector2 position
        {
            get => _position;
            set => _position = value;
        }
        
        /// <summary>
        /// Graph中的Rect
        /// </summary>
        public Rect rect
        {
            get => new(_position.x, _position.y, _size.x, _size.y);
            set
            {
                _position.x = value.x;
                _position.y = value.y;
                _size.x = Mathf.Max(value.width, MIN_SIZE.x);
                _size.y = Mathf.Max(value.height, MIN_SIZE.y);
            }
        }

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
        
        /// <summary>
        /// 加载刷新后调用
        /// </summary>
        public void EditorRefresh()
        {
            _isOverwriteOnGraphStartMethod = !(GetType().GetMethod("OnGraphStart")?.DeclaringType == typeof(Node));
            OnEditorRefresh();
        }

        /// <summary>
        /// 绘制Node Inspector面版
        /// </summary>
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

        /// <summary>
        /// 绘制Node Window面版
        /// </summary>
        public virtual void DrawWindowGUI() { }

        /// <summary>
        /// 绘制Link线
        /// </summary>
        public virtual void DrawLinkGUI() { }
        
        protected virtual void OnEditorRefresh() { }
    }
}
#endif