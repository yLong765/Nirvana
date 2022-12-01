using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public abstract partial class Port
    {
        public int order { get; set; }
        public Rect rect { get; set; }
        public PortType portType { get; set; }

        public bool IsInPort()
        {
            return this is InPort;
        }

        public bool IsOutPort()
        {
            return this is OutPort;
        }
    }
}