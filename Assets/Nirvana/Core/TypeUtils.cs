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
    }
}