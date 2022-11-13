using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectUtils
{
    public static Rect ModifyWitch(this Rect rect, float value)
    {
        return new Rect(rect.x, rect.y, value, rect.height);
    }
}
