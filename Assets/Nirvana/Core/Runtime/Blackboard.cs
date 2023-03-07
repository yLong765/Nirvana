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
        private static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
        };
        
        [SerializeField] private string _serializedData = string.Empty;
        private BlackboardSource _bbSource;

        /// <summary>
        /// variable字典
        /// </summary>
        public Dictionary<string, Variable> variables
        {
            get => _bbSource.variables;
            set => _bbSource.variables = value;
        }

        public void OnBeforeSerialize()
        {
            _serializedData = JsonConvert.SerializeObject(_bbSource, Formatting.None, Settings);
        }

        public void OnAfterDeserialize()
        {
            _bbSource = !string.IsNullOrEmpty(_serializedData)
                ? JsonConvert.DeserializeObject<BlackboardSource>(_serializedData, Settings)
                : new BlackboardSource();
        }
    }
}