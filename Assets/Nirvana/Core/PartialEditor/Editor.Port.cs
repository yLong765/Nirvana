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

        [JsonIgnore]
        public int order
        {
            get => _order;
            set => _order = value;
        }

        [JsonIgnore]
        public Rect rect
        {
            get => _rect;
            set => _rect = value;
        }

        public bool IsInPort()
        {
            return this is InPort || this is FlowInPort;
        }

        public bool IsOutPort()
        {
            return this is OutPort || this is FlowOutPort;
        }
    }
}