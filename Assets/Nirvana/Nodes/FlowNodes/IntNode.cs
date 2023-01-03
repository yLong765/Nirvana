using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class IntNode : FlowNode
{
    public int value;
    
    protected override void RegisterPorts()
    {
        AddOutPort("value", () =>
        {
            return value;
        });
    }
}
