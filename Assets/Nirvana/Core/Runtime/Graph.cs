using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Graph : ISerializationCallbackReceiver
    {
        [SerializeField] private string _serializedData;
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private string _name;
        
        private List<Node> _nodes = new List<Node>();
        
        public Blackboard blackboard
        {
            get => _blackboard;
            set
            {
                _blackboard = value;
            }
        }

        [JsonIgnore] public List<Node> allNodes => _nodes;

        public Vector2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public Node AddNode(Type type, Vector2 pos)
        {
            var newNode = Node.Create(this, type, pos);
            allNodes.Add(newNode);
            newNode.ID = allNodes.Count;
            return newNode;
        }

        public void RemoveNode(Node node)
        {
            if (_nodes.Contains(node))
            {
                _nodes.Remove(node);
            }

            for (int i = 0; i < allNodes.Count; i++)
            {
                allNodes[i].ID = i;
            }
        }
        
        public Variable AddVariable(Type type, string varName)
        {
            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable = (Variable)Activator.CreateInstance(variableType);
            newVariable.name = varName;
            blackboard.variables[varName] = newVariable;
            return newVariable;
        }

        public void OnBeforeSerialize()
        {
            var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
            _serializedData = JsonConvert.SerializeObject(_nodes, Formatting.None, settings);
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(_serializedData))
            {
                var settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
                _nodes = JsonConvert.DeserializeObject<List<Node>>(_serializedData, settings);

                if (_nodes == null) return;

                for (int i = 0; i < _nodes.Count; i++)
                {
                    _nodes[i].ID = i;
                    _nodes[i].graph = this;
                }
            }
        }
    }
}