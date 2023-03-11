#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public interface IEditorDrawer
    {
        public object DrawGUI(GUIContent content, object value, FieldInfo fieldInfo);
    }

    public abstract class EditorDrawer : IEditorDrawer
    {
        private static Dictionary<Type, IEditorDrawer> _cacheDrawer = new Dictionary<Type, IEditorDrawer>();

        public static IEditorDrawer Get(Type type)
        {
            if (_cacheDrawer.ContainsKey(type))
            {
                return _cacheDrawer[type];
            }

            Type fallbackDrawerType = null;
            foreach (var drawerType in ReflectionUtils.GetImplementationsOf(typeof(IEditorDrawer)))
            {
                if (drawerType != typeof(DefaultObjectDrawer))
                {
                    if (drawerType.BaseType != null)
                    {
                        var args = drawerType.BaseType.GetGenericArguments();
                        if (args.Length == 1)
                        {
                            if (args[0].IsEquivalentTo(type))
                            {
                                return _cacheDrawer[type] = Activator.CreateInstance(drawerType) as IEditorDrawer;
                            }

                            if (args[0].IsAssignableFrom(type))
                            {
                                fallbackDrawerType = drawerType;
                            }
                        }
                    }
                }
            }

            if (fallbackDrawerType != null)
            {
                return _cacheDrawer[type] = Activator.CreateInstance(fallbackDrawerType) as IEditorDrawer;
            }

            return null;
        }

        public static IEditorDrawer Get<T>() where T : IEditorDrawer
        {
            return Get(typeof(T));
        }

        public abstract object DrawGUI(GUIContent content, object value, FieldInfo fieldInfo);
    }

    public abstract class EditorDrawer<T> : EditorDrawer
    {
        protected GUIContent content { get; private set; }
        protected T value { get; private set; }
        protected FieldInfo fieldInfo { get; private set; }

        public override object DrawGUI(GUIContent content, object value, FieldInfo fieldInfo)
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

        protected abstract T OnGUI(GUIContent content, T value);
    }
}
#endif