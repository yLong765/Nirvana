using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class ConditionNode : FlowNode
{
    protected override void RegisterPorts()
    {
        AddFlowInPort("In", () =>
        {
            
        });
    }
}
