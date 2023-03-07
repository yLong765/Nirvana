using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    public abstract partial class Port
    {
        private int _order;
        private Rect _rect;

        /// <summary>
        /// Editor.排序
        /// </summary>
        [JsonIgnore]
        public int order
        {
            get => _order;
            set => _order = value;
        }

        /// <summary>
        /// Editor.Port点位置
        /// </summary>
        [JsonIgnore]
        public Rect rect
        {
            get => _rect;
            set => _rect = value;
        }

        /// <summary>
        /// 是否是In Port
        /// </summary>
        public bool IsInPort()
        {
            return this is InPort || this is FlowInPort;
        }

        /// <summary>
        /// 是否是Out Port
        /// </summary>
        public bool IsOutPort()
        {
            return this is OutPort || this is FlowOutPort;
        }
    }
}