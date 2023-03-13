#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public interface IObjectDrawer
    {
        public object DrawGUI(GUIContent content, object value, FieldInfo info);
    }

    public abstract class ObjectDrawer : IObjectDrawer
    {
        private static Dictionary<Type, IObjectDrawer> _cacheDrawer = new Dictionary<Type, IObjectDrawer>();
        
        public static IObjectDrawer Get(Type type)
        {
            if (_cacheDrawer.ContainsKey(type))
            {
                return _cacheDrawer[type];
            }

            Type fallbackDrawerType = null;
            foreach (var drawerType in ReflectionUtils.GetImplementationsOf(typeof(IObjectDrawer)))
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
                                return _cacheDrawer[type] = Activator.CreateInstance(drawerType) as IObjectDrawer;
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
                return _cacheDrawer[type] = Activator.CreateInstance(fallbackDrawerType) as IObjectDrawer;
            }

            return _cacheDrawer[type] = Activator.CreateInstance<DefaultObjectDrawer>();
        }

        public static IObjectDrawer Get<T>() where T : IObjectDrawer
        {
            return Get(typeof(T));
        }
        
        public abstract object DrawGUI(GUIContent content, object value, FieldInfo info);
    }
    
    public abstract class ObjectDrawer<T> : ObjectDrawer
    {
        protected GUIContent content { get; private set; }
        protected T value { get; private set; }
        protected FieldInfo info { get; private set; }

        public override object DrawGUI(GUIContent content, object value, FieldInfo info)
        {
            this.content = content;
            this.value = (T) value;
            this.info = info;

            var result = OnGUI(content, this.value);

            this.content = null;
            this.value = default;
            this.info = default;

            return result;
        }

        protected abstract T OnGUI(GUIContent content, T variable);
    }
}
#endif