using System;
using System.Collections;
using System.Collections.Generic;
using Nirvana;

public class TestNode : FlowNode
{
    [InPort] public string testName;
    [OutPort] public string GGName;
    [OutPort] public string GG2Name;
    [OutPort] public string GG1Name;
}
