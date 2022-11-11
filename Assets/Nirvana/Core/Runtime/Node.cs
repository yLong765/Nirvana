using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Node
    {
        [SerializeField] private int _id;
        [SerializeField] private string _title;
        [SerializeField] private string _introduction;
        [SerializeField] private Graph _graph;
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _size;

        public int ID
        {
            get => _id;
            set => _id = value;
        }

        public string title
        {
            get => _title;
            set => _title = value;
        }
        public string Introduction => _introduction;
        public Graph graph => _graph;

        public Vector2 position
        {
            get => _position;
            set => _position = value;
        }

        public Rect rect
        {
            get => new(_position.x, _position.y, _size.x, _size.y);
            set
            {
                _position.x = value.x;
                _position.y = value.y;
                _size.x = Mathf.Max(value.width, MIN_SIZE.x);
                _size.y = Mathf.Max(value.height, MIN_SIZE.y);
            }
        }

        public static Vector2 MIN_SIZE = new(80, 50);

        public Node(Graph graph, Vector2 pos)
        {
            _graph = graph;
            _position = pos;
            _size = MIN_SIZE;
        }

        public static Node Create(Graph graph, Vector2 pos)
        {
            return new Node(graph, pos);
        }

#if UNITY_EDITOR
        
        public bool isSelected => Editor.GraphUtils.activeNodes.Contains(this);
#endif
    }
}