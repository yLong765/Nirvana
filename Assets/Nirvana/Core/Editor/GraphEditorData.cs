using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.Editor
{
    [Serializable]
    [CreateAssetMenu(menuName = "Nirvana Tools/Graph Canvas")]
    public class GraphEditorData : ScriptableObject
    {
        [SerializeField] private Graph _graph;
        public Graph graph
        {
            get
            {
                if (_graph == null)
                {
                    _graph = new Graph();
                    _graph.name = "Graph Canvas";
                }

                return _graph;
            }
        }
        
    }
}