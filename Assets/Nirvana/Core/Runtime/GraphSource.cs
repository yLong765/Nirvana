using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class GraphSource
    {
        private List<Node> _nodes;
        private BlackboardSource _bbSource;

        /// <summary>
        /// Graph里全部的Node
        /// </summary>
        public List<Node> nodes
        {
            get => _nodes;
            set => _nodes = value;
        }

        /// <summary>
        /// Graph使用的blackboard元数据
        /// </summary>
        public BlackboardSource bbSource
        {
            get => _bbSource;
            set => _bbSource = value;
        }

        public GraphSource()
        {
            _nodes = new List<Node>();
            _bbSource = new BlackboardSource();
        }
    }
}