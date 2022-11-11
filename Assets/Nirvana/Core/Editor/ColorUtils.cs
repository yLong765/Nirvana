using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static Color WithAlpha(this Color color, float a)
    {
        return new(color.r, color.g, color.b, a);
    }

    public static Color mediumPurple => new Color(0.576f, 0.439f, 0.859f, 1f);
    public static Color gray13 => new Color(0.13f, 0.13f, 0.13f, 1f);
    public static Color gray19 => new Color(0.19f, 0.19f, 0.19f, 1f);
    public static Color gray21 => new Color(0.21f, 0.21f, 0.21f, 1f);
    public static Color deepSkyBlue => new Color(0f, 0.749f, 1f, 1f);
    public static Color orange1 => new Color(1f, 0.647f, 0f, 1f);
    public static Color darkOrang2 => new Color(0.933f, 0.463f, 0f, 1f);
}
