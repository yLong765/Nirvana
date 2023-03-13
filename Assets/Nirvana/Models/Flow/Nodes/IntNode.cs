using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class IntNode : FlowNode
{
    public BBVar<int> value;
    
    protected override void RegisterPorts()
    {
        AddOutPort("value", () => value.value);
    }
}
