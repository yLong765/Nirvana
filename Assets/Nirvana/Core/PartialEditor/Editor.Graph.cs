#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public partial class Graph
    {
        [SerializeField] private string _serializedData;
        [SerializeField] private Vector2 _offset;
        [SerializeField] private float _zoom;

        public string serializedData
        {
            get => _serializedData;
            private set => _serializedData = value;
        }
        
        public Vector2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public float zoom
        {
            get => _zoom;
            set => _zoom = value;
        }
        
        public string title
        {
            get => _graphSource.title;
            set => _graphSource.title = value;
        }
    }
}
#endif