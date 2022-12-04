using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class Test1Node : FlowNode
{
    public FlowOutPort outPort;
    
    protected override void RegisterPorts()
    {
        outPort = AddFlowOutPort("Out");
        AddOutPort<string>("ceshi", () => "ceshi");
    }

    public override void Execute()
    {
        outPort.Call();
    }
}
