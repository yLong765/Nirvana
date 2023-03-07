using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectUtils
{
    public static Rect ModifyWitch(this Rect rect, float value)
    {
        return new Rect(rect.x, rect.y, value, rect.height);
    }

    /// <summary>
    /// 根据两个点构建Rect
    /// </summary>
    public static Rect GetBoundRect(Vector2 p1, Vector2 p2)
    {
        var minX = Mathf.Min(p1.x, p2.x);
        var maxX = Mathf.Max(p1.x, p2.x);
        var minY = Mathf.Min(p1.y, p2.y);
        var maxY = Mathf.Max(p1.y, p2.y);
        return Rect.MinMaxRect(minX, minY, maxX, maxY);
    }
}
