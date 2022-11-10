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
            return newNode;
        }
    }
}