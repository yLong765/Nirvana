using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public abstract partial class Graph : ScriptableObject, ISerializationCallbackReceiver, ISerialize
    {
        #region abstract

        public abstract Type baseNodeType { get; }

        #endregion
        
        private GraphSource _graphSource = new GraphSource();
        [JsonIgnore] public List<Node> allNodes => _graphSource.nodes;
        
        public BlackboardSource bbSource
        {
            get => _graphSource.bbSource;
            set => _graphSource.bbSource = value;
        }

        private void InitGraph()
        {
            foreach (var link in allNodes.SelectMany(node => node.inLinks))
            {
                link.BindPorts();
            }
        }
        
        public void StartGraph()
        {
            InitGraph();
            
            foreach (var node in allNodes)
            {
                node.OnGraphStart();
            }
        }

        public Node AddNode(Type type, Vector2 pos)
        {
            var newNode = Node.Create(this, type, pos);
            newNode.ID = allNodes.Count;
            
            Undo.RecordObject(this, "New Node");
            
            allNodes.Add(newNode);
            
            EditorUtility.SetDirty(this);
            return newNode;
        }

        public void RemoveNode(Node node)
        {
            Undo.RecordObject(this, "Remove Node");
            
            if (allNodes.Contains(node))
            {
                node.OnDestroy();
                allNodes.Remove(node);
            }

            UpdateNodeIDs();
            EditorUtility.SetDirty(this);
        }

        public Link AddLink(Port sourcePort, Port targetPort)
        {
            if (!Node.IsNewLinkAllowed(sourcePort, targetPort)) return null;
            
            Undo.RecordObject(this, "New Link");
            
            var newLink = Link.Create(sourcePort, targetPort);
            
            EditorUtility.SetDirty(this);
            return newLink;
        }

        public void RemoveLink(Link link)
        {
            Undo.RecordObject(this, "Remove Link");
            link.sourceNode.outLinks.Remove(link);
            link.targetNode.inLinks.Remove(link);
            link.sourcePort.linkCount--;
            link.targetPort.linkCount--;
            EditorUtility.SetDirty(this);
        }

        public void UpdateNodeIDs()
        {
            for (int i = 0; i < allNodes.Count; i++)
            {
                allNodes[i].ID = i;
            }
        }

        #region JsonSerialize

        private static readonly JsonSerializerSettings Settings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (sender, args) =>
            {
                Debug.LogError(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            } 
        };
        
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
                _graphSource.nodes[i].ID = i;
                _graphSource.nodes[i].graph = this;
                _graphSource.nodes[i].Refresh();
            }
        }

        #endregion
    }
}