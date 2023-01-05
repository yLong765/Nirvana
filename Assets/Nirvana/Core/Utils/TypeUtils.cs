using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Nirvana
{
    public static class TypeUtils
    {
        public static Type[] defaultTypes = new[]
        {
            typeof(int),
            typeof(float),
            typeof(double),
            typeof(string),

            typeof(Vector2),
            typeof(Vector2Int),
            typeof(Vector3),
            typeof(Vector3Int),
            typeof(Vector4),
            typeof(Quaternion),
        };

        public static List<Type> GetChildTypes(Type type)
        {
            return defaultTypes.Where(type.IsAssignableFrom).ToList();
        }

        public static List<Type> GetSubClassTypes(Type type)
        {
            var result = new List<Type>();
            var allTypes = type.Assembly.GetTypes();
            foreach (var t in allTypes)
            {
                if (type.IsAssignableFrom(t) && t != type)
                {
                    result.Add(t);
                }
            }

            return result;
        }

        // public static List<FieldInfo> GetAllFields(Type type)
        // {
        //     return type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        // }

        #region Extend

        public static List<FieldInfo> GetAllFields(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        }
        
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return (T) type.GetAttribute(typeof(T));
        }

        public static Attribute GetAttribute(this Type type, Type attributeType)
        {
            var attributes = type.GetCustomAttributes(true);
            foreach (Attribute attribute in attributes)
            {
                var attType = attribute.GetType();
                if (attributeType.IsAssignableFrom(attType))
                {
                    return attribute;
                }
            }

            return null;
        }

        public static bool TryGetAttribute<T>(this Type type, out T attribute) where T : Attribute
        {
            attribute = GetAttribute<T>(type);
            return attribute != null;
        }
        
        public static T GetAttribute<T>(this FieldInfo info) where T : Attribute
        {
            var attributes = info.GetCustomAttributes();
            var targetType = typeof(T);
            foreach (Attribute attribute in attributes)
            {
                if (targetType.IsInstanceOfType(attribute))
                {
                    return attribute as T;
                }
            }
            
            return null;
        }

        public static bool HasAttribute<T>(this FieldInfo info) where T : Attribute
        {
            var att = info.GetAttribute<T>();
            return att != null;
        }

        public static bool TryGetAttribute<T>(this FieldInfo info, out T attribute) where T : Attribute
        {
            attribute = info.GetAttribute<T>();
            return attribute != null;
        }

        #endregion
    }
}