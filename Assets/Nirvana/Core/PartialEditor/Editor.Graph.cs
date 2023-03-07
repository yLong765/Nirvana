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
        [SerializeField] private float _zoom = 1;
        [SerializeField] private string _title;

        public string serializedData
        {
            get => _serializedData;
            private set => _serializedData = value;
        }
        
        /// <summary>
        /// Editor.便宜
        /// </summary>
        public Vector2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        /// <summary>
        /// Editor.缩放
        /// </summary>
        public float zoom
        {
            get => _zoom;
            set => _zoom = value;
        }
        
        /// <summary>
        /// Editor.标题
        /// </summary>
        public string title
        {
            get => _title;
            set => _title = value;
        }
    }
}
#endif