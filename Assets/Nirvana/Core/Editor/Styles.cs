using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Styles
{
    private static GUIStyle _nodeWindow;
    public static GUIStyle nodeWindow
    {
        get
        {
            if (_nodeWindow == null)
            {
                _nodeWindow = new GUIStyle();
                _nodeWindow.normal.background = Texture2D.whiteTexture;
                _nodeWindow.padding = new RectOffset(0, 0, 0, 0);
            }

            return _nodeWindow;
        }
    }

    private static GUIStyle _nodeWindowTitle;
    public static GUIStyle nodeWindowTitle
    {
        get
        {
            if (_nodeWindowTitle == null)
            {
                _nodeWindowTitle = new GUIStyle(EditorStyles.boldLabel);
                _nodeWindowTitle.fontSize = 14;
                _nodeWindowTitle.alignment = TextAnchor.UpperCenter;
            }

            return _nodeWindowTitle;
        }
    }

    private static GUIStyle _nodeWindowTitleBG;
    public static GUIStyle nodeWindowTitleBg
    {
        get
        {
            if (_nodeWindowTitleBG == null)
            {
                _nodeWindowTitleBG = new GUIStyle();
                _nodeWindowTitleBG.normal.background = Texture2D.normalTexture;
                _nodeWindowTitleBG.margin = new RectOffset(1, 1, 1, 0);
            }

            return _nodeWindowTitleBG;
        }
    }

    private static GUIStyle _nodeWindowHeightLine;
    public static GUIStyle nodeWindowHeightLine
    {
        get
        {
            if (_nodeWindowHeightLine == null)
            {
                _nodeWindowHeightLine = new GUIStyle();
                _nodeWindowHeightLine.normal.background = Texture2D.whiteTexture;
                _nodeWindowHeightLine.border = new RectOffset(10, 10, 10, 10);
                _nodeWindowHeightLine.overflow = new RectOffset(2, 2, 2, 2);
            }

            return _nodeWindowHeightLine;
        }
    }

}
