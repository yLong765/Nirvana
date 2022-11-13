using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Graph : ISerializer
    {
        [SerializeField] private List<Node> _nodes = new List<Node>();
        [SerializeField] private Blackboard _blackboard;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private string _name;
        
        public Blackboard blackboard => _blackboard;
        public List<Node> allNodes => _nodes;

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

        public void Serialize()
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize()
        {
            throw new System.NotImplementedException();
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
    }
}