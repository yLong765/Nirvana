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
        [SerializeField] private string _tag;
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _size;
        
        [NonSerialized] private Graph _graph;

        public int ID
        {
            get => _id;
            set => _id = value;
        }

        private string FriendlyName(Type t)
        {
            var s = string.Empty;
            if (t.IsGenericType) {
                s = !string.IsNullOrEmpty(t.Namespace) ? t.Namespace + "." + t.Name : t.Name;
                var args = t.GetGenericArguments();
                if ( args.Length != 0 ) {

                    s = s.Replace("`" + args.Length, "");

                    s += "<";
                    for ( var i = 0; i < args.Length; i++ ) {
                        s += ( i == 0 ? "" : ", " ) + FriendlyName(args[i]);
                    }
                    s += ">";
                }
            }

            return s;
        }
        
        public string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = FriendlyName(GetType());
                }

                return _title;
            }
        }

        public string tag
        {
            get => _tag;
            set => _tag = value;
        }
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

        public Node() { }
        
        public Node(Graph graph, Type type, Vector2 pos)
        {
            _graph = graph;
            _position = pos;
            _size = MIN_SIZE;
        }

        public static Node Create(Graph graph, Type type, Vector2 pos)
        {
            return new Node(graph, type, pos);
        }

#if UNITY_EDITOR
        public bool isSelected => GraphUtils.activeNodes.Contains(this);
#endif
    }
}