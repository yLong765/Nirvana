using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [CreateAssetMenu(menuName = "Nirvana Tools/Flow Graph", order = 1)]
    public class FlowGraph : Graph
    {
        public override Type baseNodeType => typeof(FlowNode);
    }
}
