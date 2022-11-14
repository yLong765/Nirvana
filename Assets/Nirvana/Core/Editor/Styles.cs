using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public static class Styles
    {
        public static Vector2 CalcSize(GUIStyle style, string value)
        {
            var size = style.CalcSize(new GUIContent(value));
            size.y += 3f;
            return size;
        }

        public static float CalcHeight(GUIStyle style, string value, float width)
        {
            return style.CalcHeight(new GUIContent(value), width);
        }
        
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
                    _windowTitle.normal.textColor = ColorUtils.orange1;
                    _windowTitle.hover.textColor = ColorUtils.orange1;
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

        private static GUIStyle _tagText;
        public static GUIStyle tagText
        {
            get
            {
                if (_tagText == null)
                {
                    _tagText = new GUIStyle(EditorStyles.label);
                    _tagText.normal.textColor = ColorUtils.gray51;
                    _tagText.hover.textColor = ColorUtils.gray51;
                    _tagText.fontSize = 10;
                    _tagText.wordWrap = true;
                }

                return _tagText;
            }
        }

        private static GUIStyle _panelTitle;
        public static GUIStyle panelTitle
        {
            get
            {
                if (_panelTitle == null)
                {
                    _panelTitle = new GUIStyle(EditorStyles.label);
                    _panelTitle.fontSize = 16;
                    _panelTitle.fontStyle = FontStyle.Bold;
                    _panelTitle.normal.textColor = ColorUtils.orange1;
                    _panelTitle.hover.textColor = ColorUtils.orange1;
                    _panelTitle.alignment = TextAnchor.MiddleCenter;
                }

                return _panelTitle;
            }
        }

        private static GUIStyle _symbolText;
        public static GUIStyle symbolText
        {
            get
            {
                if (_symbolText == null)
                {
                    _symbolText = new GUIStyle(EditorStyles.label);
                    _symbolText.fontSize = 20;
                    _symbolText.fontStyle = FontStyle.Bold;
                    _symbolText.alignment = TextAnchor.MiddleLeft;
                    _symbolText.padding.left = 4;
                    _symbolText.padding.top = 3;
                }

                return _symbolText;
            }
        }

        private static GUIStyle _graphTitle;
        public static GUIStyle graphTitle
        {
            get
            {
                if (_graphTitle == null)
                {
                    _graphTitle = new GUIStyle(EditorStyles.label);
                    _graphTitle.fontSize = 14;
                    _graphTitle.alignment = TextAnchor.MiddleCenter;
                }

                return _graphTitle;
            }
        }
        
        private static GUIStyle _defaultLabel;
        public static GUIStyle defaultLabel {
            get
            {
                if ( _defaultLabel == null ) {
                    _defaultLabel = new GUIStyle(EditorStyles.label);
                    _defaultLabel.normal.textColor = ColorUtils.gray51;
                    _defaultLabel.fontSize = 11;
                    _defaultLabel.alignment = TextAnchor.MiddleLeft;
                    _defaultLabel.padding.left = 6;
                }
                return _defaultLabel;
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