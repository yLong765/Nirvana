using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEngine;

[Name("Condition Int")]
public class ConditionNode : FlowNode
{
    private InPort<int> v1;
    private InPort<int> v2;
    private FlowOutPort trueFlow;
    private FlowOutPort falseFlow;
    
    protected override void RegisterPorts()
    {
        v1 = AddInPort<int>("value1");
        v2 = AddInPort<int>("value2");
        trueFlow = AddFlowOutPort("True");
        falseFlow = AddFlowOutPort("False");
        
        AddFlowInPort("In", () =>
        {
            if (v1.value > v2.value)
            {
                trueFlow.Call();
            }
            else
            {
                falseFlow.Call();
            }
        });

    }
}
