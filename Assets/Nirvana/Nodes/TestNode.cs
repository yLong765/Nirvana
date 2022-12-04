using System;
using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class TestNode : FlowNode
{
    public InPort<string> s1;
    public InPort<string> s2;
    
    protected override void RegisterPorts()
    {
        s1 = AddInPort<string>("String Param");
        s2 = AddInPort<string>("String Param");
        AddOutPort<string>("String Param", () => "NB");
        AddFlowInPort("In", Execute);
        AddFlowOutPort("Out");
    }

    public override void Execute()
    {
        Debug.Log(s1.value);
        Debug.Log(s2.value);
    }
}
