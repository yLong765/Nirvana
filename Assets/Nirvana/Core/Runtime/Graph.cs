using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Graph : ISerializationCallbackReceiver, ISerialize
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        };
        
        [SerializeField] private string _serializedData;
        [SerializeField] private Vector2 _offset;
        private GraphSource _graphSource = new GraphSource();
        
        [JsonIgnore] public List<Node> allNodes => _graphSource.nodes;
        
        public BlackboardSource bbSource
        {
            get => _graphSource.bbSource;
            set => _graphSource.bbSource = value;
        }

        public Vector2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public string title
        {
            get => _graphSource.title;
            set => _graphSource.title = value;
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
            if (allNodes.Contains(node))
            {
                node.OnDelete();
                allNodes.Remove(node);
            }

            for (int i = 0; i < allNodes.Count; i++)
            {
                allNodes[i].ID = i + 1;
            }
        }
        
        public Variable AddVariable(Type type, string varName)
        {
            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable = (Variable)Activator.CreateInstance(variableType);
            newVariable.name = varName;
            bbSource.variables[varName] = newVariable;
            return newVariable;
        }

        public Link AddLink(Node source, Node target, string sourceOutPortName, string targetInPortName)
        {
            if (!Node.IsNewLinkAllowed(source, target, sourceOutPortName, targetInPortName)) return null;
            
            var link = new Link();
            link.SetSourceNode(source);
            link.SetTargetNode(target);
            return link;
        }

        public void DelLink(Link link)
        {
            link.sourceNode.outLinks.Remove(link);
            link.targetNode.inLinks.Remove(link);
        }

        public void OnBeforeSerialize()
        {
            _serializedData = Serialize(Formatting.Indented);
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(_serializedData))
            {
                Deserialize(_serializedData);
            }
            else
            {
                _graphSource = new GraphSource();
            }
        }

        public string Serialize(Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(_graphSource, formatting, Settings);
        }

        public void Deserialize(string json)
        {
            _graphSource = JsonConvert.DeserializeObject<GraphSource>(json, Settings) ?? new GraphSource();
            for (int i = 0; i < _graphSource.nodes.Count; i++)
            {
                _graphSource.nodes[i].ID = i + 1;
                _graphSource.nodes[i].graph = this;
                _graphSource.nodes[i].OnRefresh();
            }
        }
    }
}