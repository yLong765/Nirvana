using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Blackboard : ISerializationCallbackReceiver
    {
        [SerializeField] private string _serializedData = string.Empty;
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public Dictionary<string, Variable> variables
        {
            get => _variables;
            set => _variables = value;
        }

        private static JsonSerializerSettings _settings = new() {TypeNameHandling = TypeNameHandling.All};

        public void OnBeforeSerialize()
        {
            _serializedData = JsonConvert.SerializeObject(_variables, Formatting.Indented, _settings);
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(_serializedData))
            {
                _variables = JsonConvert.DeserializeObject<Dictionary<string, Variable>>(_serializedData, _settings);
            }
        }

        public Variable AddVariable(Type type, string varName)
        {
            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable = (Variable)Activator.CreateInstance(variableType);
            newVariable.name = varName;
            _variables[varName] = newVariable;
            return newVariable;
        }
    }
}