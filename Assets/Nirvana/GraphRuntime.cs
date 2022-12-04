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
        var test = Resources.Load<GraphEditorData>("NB");
        Debug.Log("GGG");
        test.Execute();
    }
}
