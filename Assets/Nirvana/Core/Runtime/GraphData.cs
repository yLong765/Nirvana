using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    [CreateAssetMenu(menuName = "Nirvana Tools/Graph Canvas")]
    public class GraphData : ScriptableObject
    {
        [SerializeField] private Graph _graph;
        public Graph graph
        {
            get
            {
                _graph ??= new Graph();
                _graph.title = name;
                return _graph;
            }
        }

        private void InitGraph()
        {
            foreach (var link in graph.allNodes.SelectMany(node => node.inLinks))
            {
                link.BindPorts();
            }
        }
        
        public void StartGraph()
        {
            InitGraph();
            
            foreach (var node in graph.allNodes)
            {
                node.OnGraphStart();
            }
        }
    }
}