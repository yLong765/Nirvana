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
        
        public Blackboard blackboard => _blackboard;
        public List<Node> allNodes => _nodes;

        public void Serialize()
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize()
        {
            throw new System.NotImplementedException();
        }
        
        public Node AddNode(Vector2 pos)
        {
            var newNode = Node.Create(this, pos);
            allNodes.Add(newNode);
            newNode.ID = allNodes.Count;
            newNode.title = "Is A Title";
            return newNode;
        }

        public void RemoveNode(Node node)
        {
            if (_nodes.Contains(node))
            {
                _nodes.Remove(node);
            }
        }
    }
}