using System;
using System.Collections;
using System.Collections.Generic;
using Nirvana;
using UnityEditor;
using UnityEngine;

public class GraphRuntime : MonoBehaviour
{
    private void Start()
    {
        Resources.Load<GraphData>("NB").StartGraph();
    }
}
