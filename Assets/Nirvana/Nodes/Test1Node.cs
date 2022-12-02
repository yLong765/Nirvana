using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class Test1Node : FlowNode
{
    protected override void RegisterPorts()
    {
        AddFlowOutPort("Out");
        AddOutPort<string>("ceshi", () => "ceshi");
    }
}
