using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public interface IDrawer
    {
        public object DrawGUI(GUIContent content, object value, FieldInfo fieldInfo);
    }
    
    public abstract class EditorDrawer<T> : IDrawer where T : class
    {
        private static Dictionary<Type, IDrawer> _cacheDrawer = new Dictionary<Type, IDrawer>();

        protected GUIContent content { get; private set; }
        protected T value { get; private set; }
        protected FieldInfo fieldInfo { get; private set; }

        public static IDrawer Get()
        {
            var type = typeof(T);
            if (_cacheDrawer.ContainsKey(type))
            {
                return _cacheDrawer[type];
            }

            var drawer = Activator.CreateInstance(type) as IDrawer;
            _cacheDrawer.Add(type, drawer);
            return drawer;
        }

        public object DrawGUI(GUIContent content, object value, FieldInfo fieldInfo)
        {
            this.content = content;
            this.value = (T) value;
            this.fieldInfo = fieldInfo;

            var result = OnGUI(content, this.value);

            this.content = null;
            this.value = default;
            this.fieldInfo = default;

            return result;
        }

        protected abstract object OnGUI(GUIContent content, T value);
    }
}