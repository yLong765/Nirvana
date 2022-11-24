using System;
using System.Collections;
using System.Collections.Generic;

namespace Nirvana
{
    public class GraphSource
    {
        private List<Node> _nodes;
        private BlackboardSource _bbSource;
        private string _title;

        public List<Node> nodes
        {
            get => _nodes;
            set => _nodes = value;
        }

        public BlackboardSource bbSource
        {
            get => _bbSource;
            set => _bbSource = value;
        }

        public string title
        {
            get => _title;
            set => _title = value;
        }

        public GraphSource()
        {
            _nodes = new List<Node>();
            _bbSource = new BlackboardSource();
            _title = "Graph Canvas";
        }
    }
}