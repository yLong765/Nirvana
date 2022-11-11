using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtend
{
    public static Color SetRGB(this Color color, float value)
    {
        return new(value, value, value, color.a);
    }
}
