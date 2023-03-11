using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ParadoxNotion;
using UnityEngine;

namespace Nirvana
{
    public static class ReflectionUtils
    {
        private static Assembly[] _loadedAssemblies;
        private static Type[] _allTypes;
        private static Dictionary<Type, Type[]> _subTypesMap = new Dictionary<Type, Type[]>();
        private static Dictionary<Type, Type[]> _genericArgsTypeCache = new Dictionary<Type, Type[]>();

        private static Assembly[] loadedAssemblies => _loadedAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        public static Type[] GetAllTypes(bool includeObsolete)
        {
            if (_allTypes != null)
            {
                return _allTypes;
            }

            var result = new List<Type>();
            foreach (var asm in loadedAssemblies)
            {
                try
                {
                    result.AddRange(asm.GetExportedTypes().Where(t => includeObsolete || !t.RTIsDefined<ObsoleteAttribute>(false)));
                }
                catch
                {
                    continue;
                }
            }

            return _allTypes = result.OrderBy(t => t.Namespace).ThenBy(t => t.FriendlyName()).ToArray();
        }

        public static Type[] GetImplementationsOf(Type baseType)
        {
            if (_subTypesMap.TryGetValue(baseType, out var result))
            {
                return result;
            }

            var allTypes = GetAllTypes(false);
            return _subTypesMap[baseType] = allTypes.Where(type => baseType.RTIsAssignableFrom(type) && !type.RTIsAbstract()).ToArray();
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            if (_genericArgsTypeCache.TryGetValue(type, out var result))
            {
                return result;
            }

            return _genericArgsTypeCache[type] = type.GetGenericArguments();
        }
    }
}