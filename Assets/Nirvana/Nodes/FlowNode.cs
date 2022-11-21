using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class FlowNode : Node
    {
        [InPort] public Port InPort;
        [OutPort] public Port OutPort;
    }
}