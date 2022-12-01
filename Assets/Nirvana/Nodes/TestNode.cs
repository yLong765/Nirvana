using System;
using System.Collections;
using System.Collections.Generic;
using Nirvana;

public class TestNode : FlowNode
{
    protected override void RegisterPorts()
    {
        AddInPort<string>("String Param");
        AddInPort<string>("String Param");
        AddOutPort<string>("String Param", () => "NB");
        AddFlowInPort("In");
        AddFlowOutPort("Out");
    }
}
