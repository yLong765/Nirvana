using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class HTN_Graph : Graph
    {
        public override Type baseNodeType => typeof(HTN_Node);
    }
}
