using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class StartNode : FlowNode
{
    private FlowOutPort start;
    
    protected override void RegisterPorts()
    {
        start = AddFlowOutPort("start");
    }

    public override void OnGraphStart()
    {
        start.Call();
    }
}
