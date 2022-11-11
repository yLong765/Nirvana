using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public static class Styles
    {
        private static GUIStyle _normalBG;
        public static GUIStyle normalBG
        {
            get
            {
                if (_normalBG == null)
                {
                    _normalBG = new GUIStyle();
                    _normalBG.normal.background = Texture2D.whiteTexture;
                }

                return _normalBG;
            }
        }

        private static GUIStyle _windowTitle;
        public static GUIStyle windowTitle
        {
            get
            {
                if (_windowTitle == null)
                {
                    _windowTitle = new GUIStyle(EditorStyles.boldLabel);
                    _windowTitle.fontSize = 14;
                    _windowTitle.alignment = TextAnchor.UpperCenter;
                    _windowTitle.normal.textColor = Color.black;
                }

                return _windowTitle;
            }
        }

        private static GUIStyle _windowHeightLine;
        public static GUIStyle windowHeightLine
        {
            get
            {
                if (_windowHeightLine == null)
                {
                    _windowHeightLine = new GUIStyle();
                    _windowHeightLine.normal.background = Texture2D.whiteTexture;
                    _windowHeightLine.overflow = new RectOffset(2, 2, 2, 1);
                }

                return _windowHeightLine;
            }
        }

        private static GUIStyle _menuTitle;
        public static GUIStyle menuTitle
        {
            get
            {
                if (_menuTitle == null)
                {
                    _menuTitle = new GUIStyle(EditorStyles.label);
                    _menuTitle.fontSize = 14;
                    _menuTitle.fontStyle = FontStyle.Bold;
                    _menuTitle.alignment = TextAnchor.MiddleCenter;
                }

                return _menuTitle;
            }
        }

        private static GUIStyle _toolbarLeftButton;
        public static GUIStyle toolbarLeftButton
        {
            get
            {
                if (_toolbarLeftButton == null)
                {
                    _toolbarLeftButton = new GUIStyle(EditorStyles.toolbarButton);
                    _toolbarLeftButton.alignment = TextAnchor.MiddleLeft;
                }

                return _toolbarLeftButton;
            }
        }
        
        private static GUIStyle _toolbarSearchField;
        public static GUIStyle toolbarSearchTextField
        {
            get
            {
                if (_toolbarSearchField == null)
                {
                    _toolbarSearchField ??= new GUIStyle("ToolbarSeachTextField");
                    _toolbarSearchField.margin.left = 5;
                }

                return _toolbarSearchField;
            }
        }
        private static GUIStyle _toolbarSearchButton;
        public static GUIStyle toolbarSearchCancelButton => _toolbarSearchButton ??= new GUIStyle("ToolbarSeachCancelButton");
    }
}