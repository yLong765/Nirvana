using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
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
                _graph ??= new Graph();
                _graph.title = name;
                return _graph;
            }
        }


        public void Execute()
        {
            Debug.Log("Execute");

            foreach (var node in graph.allNodes)
            {
                if (node is FlowNode)
                {
                    foreach (var link in node.inLinks)
                    {
                        if (link.targetPort is InPort inPort)
                        {
                            inPort.BindTo(link.sourcePort as OutPort);
                        }
                        else if (link.sourcePort is FlowOutPort flowOutPort)
                        {
                            flowOutPort.BindTo(link.targetPort as FlowInPort);
                        }
                    }
                }
            }
            
            var findNode = graph.allNodes.Find(node => node.title == "Test1Node");
            if (findNode is FlowNode fnode)
            {
                Debug.Log("fnode Execute");
                fnode.Execute();
            }
        }
    }
}