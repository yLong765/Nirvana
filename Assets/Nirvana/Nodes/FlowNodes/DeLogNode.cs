using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

public class DeLogNode : FlowNode
{
    public string content;
    
    protected override void RegisterPorts()
    {
        AddFlowInPort("In", () =>
        {
            Debug.Log(content);
        });
    }
}
