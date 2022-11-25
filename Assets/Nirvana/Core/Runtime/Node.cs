using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public partial class Node 
    {
        private int _id;
        private string _title;
        private string _tag;
        private Vector2 _position;
        private Vector2 _size;
        
        private Graph _graph;

        private List<Link> _links = new List<Link>();

        public List<Link> links
        {
            get => _links;
            set => _links = value;
        }

        public void AddLink(Port port)
        {
            
        }

        public void DelLink(Port port)
        {
            
        }

        [JsonIgnore] public int ID
        {
            get => _id;
            set => _id = value;
        }

        public string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = GetType().Name;
                }

                return _title;
            }
        }

        public string tag
        {
            get => _tag;
            set => _tag = value;
        }

        [JsonIgnore] public Graph graph
        {
            get => _graph;
            set => _graph = value;
        }

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

        [JsonIgnore] public static Vector2 MIN_SIZE = new(80, 50);

        public virtual void OnCreate() { }
        
        public virtual void OnRefresh() { }

        public static Node Create(Graph graph, Type type, Vector2 pos)
        {
            var newNode = (Node) Activator.CreateInstance(type);
            newNode.graph = graph;
            newNode.position = pos;
            newNode.OnCreate();
            return newNode;
        }
    }
}