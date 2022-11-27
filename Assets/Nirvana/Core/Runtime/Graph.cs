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
        private static JsonSerializerSettings _settings = new()
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

        public bool CheckCanLink(Node source, Node target, string sourceOutPortName, string targetInPortName)
        {
            if (source.TryGetOutPort(sourceOutPortName, out Port outPort))
            {
                if (outPort.linkCount == outPort.maxLinkCount)
                {
                    return false;
                }
            }

            if (target.TryGetInPort(targetInPortName, out Port inPort))
            {
                if (inPort.linkCount == inPort.maxLinkCount)
                {
                    return false;
                }
            }

            if (inPort != null && outPort != null)
            {
                if (!outPort.fieldType.IsAssignableFrom(inPort.fieldType))
                {
                    return false;
                }
            }

            return true;
        }
        
        public Link AddLink(Node source, Node target, string sourceOutPortName, string targetInPortName)
        {
            if (!CheckCanLink(source, target, sourceOutPortName, targetInPortName)) return null;
            
            var link = new Link();
            link.SetSourceNode(source, sourceOutPortName);
            link.SetTargetNode(target, targetInPortName);
            return link;
        }

        public void DelLink(Link link)
        {
            link.sourceNode.outLinks.Remove(link);
            link.targetNode.inLinks.Remove(link);

            if (link.sourceNode.TryGetOutPort(link.sourceOutPort, out Port outPort))
            {
                outPort.isLink = false;
            }
            
            if (link.targetNode.TryGetInPort(link.targetInPort, out Port inPort))
            {
                inPort.isLink = false;
            }
            
        }

        public void OnBeforeSerialize()
        {
            _serializedData = Serialize();
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
            return JsonConvert.SerializeObject(_graphSource, formatting, _settings);
        }

        public void Deserialize(string json)
        {
            _graphSource = JsonConvert.DeserializeObject<GraphSource>(json, _settings) ?? new GraphSource();
            for (int i = 0; i < _graphSource.nodes.Count; i++)
            {
                _graphSource.nodes[i].ID = i + 1;
                _graphSource.nodes[i].graph = this;
                _graphSource.nodes[i].OnRefresh();
            }
        }
    }
}