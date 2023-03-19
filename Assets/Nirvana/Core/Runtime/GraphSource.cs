using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    /// <summary>
    /// Graph元数据。只有这里的数据会被序列化保存
    /// </summary>
    public class GraphSource
    {
        private List<Node> _nodes;
        private BlackboardSource _bbSource;
        private List<BBVar> _bbVars;
        private List<Task> _tasks;

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

        public List<BBVar> bbVars
        {
            get => _bbVars;
            set => _bbVars = value;
        }

        public List<Task> tasks
        {
            get => _tasks;
            set => _tasks = value;
        }

        public GraphSource()
        {
            _nodes = new List<Node>();
            _bbSource = new BlackboardSource();
            _bbVars = new List<BBVar>();
        }
    }
}