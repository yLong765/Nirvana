using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public static class StyleUtils
    {
        #region Extend

        public static Vector2 CalcSize(this GUIStyle style, string value)
        {
            var size = style.CalcSize(new GUIContent(value));
            size.y += 3f;
            return size;
        }

        public static float CalcHeight(this GUIStyle style, string value, float width)
        {
            return style.CalcHeight(new GUIContent(value), width);
        }

        public static float CalcWidth(this GUIStyle style, string value)
        {
            return style.CalcSize(new GUIContent(value)).x;
        }

        #endregion

        #region Texture

        public static Texture2D bezierTexture => IconResources.Instance.icon.bezierTexture;
        public static Texture flowIconTexture => IconResources.Instance.icon.flowIconTexture;
        public static Texture settingIconTexture => IconResources.Instance.icon.settingIconTexture;
        public static Texture fixIconTexture => IconResources.Instance.icon.fixIconTexture;

        #endregion

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

        #region NodeWindow

        private static GUIStyle _windowBG;
        public static GUIStyle windowBG
        {
            get
            {
                if (_windowBG == null)
                {
                    _windowBG = new GUIStyle();
                    _windowBG.normal.background = Texture2D.whiteTexture;
                }

                return _windowBG;
            }
        }

        private static GUIStyle _normalPortBG;
        public static GUIStyle normalPortBG
        {
            get
            {
                if (_normalPortBG == null)
                {
                    _normalPortBG = new GUIStyle();
                    _normalPortBG.normal.background = Texture2D.whiteTexture;
                    _normalPortBG.margin = new RectOffset(1, 1, 0, 1);
                }

                return _normalPortBG;
            }
        }
        
        private static GUIStyle _roundedBox;
        public static GUIStyle roundedBox
        {
            get
            {
                if (_roundedBox == null)
                {
                    _roundedBox = new GUIStyle("ShurikenEffectBg");
                    _roundedBox.margin = new RectOffset(1, 1, 0, 1);
                    _roundedBox.padding = new RectOffset(0, 0, 0, 0);
                }

                return _roundedBox;
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
                    _windowTitle.alignment = TextAnchor.MiddleCenter;
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

        #endregion

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

        private static GUIStyle _panelTitleBox;
        public static GUIStyle panelTitleBox
        {
            get
            {
                if (_panelTitleBox == null)
                {
                    _panelTitleBox = new GUIStyle(EditorStyles.label);
                    _panelTitleBox.fontSize = 16;
                    _panelTitleBox.fontStyle = FontStyle.Bold;
                    _panelTitleBox.alignment = TextAnchor.MiddleCenter;
                    _panelTitleBox.normal.textColor = ColorUtils.orange1;
                    _panelTitleBox.hover.textColor = ColorUtils.orange1;
                    _panelTitleBox.normal.background = Texture2D.whiteTexture;
                }

                return _panelTitleBox;
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
        
        private static GUIStyle _variableSettingIcon;
        public static GUIStyle variableSettingIcon
        {
            get
            {
                if (_variableSettingIcon == null)
                {
                    _variableSettingIcon = new GUIStyle();
                    _variableSettingIcon.alignment = TextAnchor.MiddleCenter;
                    _variableSettingIcon.padding = new RectOffset(1, 1, 1, 1);
                }

                return _variableSettingIcon;
            }
        }
        
        private static GUIStyle _fixIcon;
        public static GUIStyle fixIcon
        {
            get
            {
                if (_fixIcon == null)
                {
                    _fixIcon = new GUIStyle();
                    _fixIcon.fixedWidth = 18;
                    _fixIcon.fixedHeight = 18;
                    _fixIcon.alignment = TextAnchor.MiddleCenter;
                    _fixIcon.padding = new RectOffset(1, 1, 3, 1);
                }

                return _fixIcon;
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
                    _graphTitle.fontSize = 12;
                    _graphTitle.fontStyle = FontStyle.Bold;
                    _graphTitle.alignment = TextAnchor.MiddleCenter;
                    _graphTitle.padding = new RectOffset(0, 6, 0, 0);
                }

                return _graphTitle;
            }
        }

        private static GUIStyle _inPortLabel;
        public static GUIStyle inPortLabel
        {
            get
            {
                if (_inPortLabel == null)
                {
                    _inPortLabel = new GUIStyle(EditorStyles.label);
                    _inPortLabel.margin = new RectOffset(0, 0, 0, 0);
                    _inPortLabel.padding = new RectOffset(0, 0, 1, 1);
                    _inPortLabel.alignment = TextAnchor.MiddleLeft;
                }

                return _inPortLabel;
            }
        }

        private static GUIStyle _outPortLabel;
        public static GUIStyle outPortLabel
        {
            get
            {
                if (_outPortLabel == null)
                {
                    _outPortLabel = new GUIStyle(EditorStyles.label);
                    _outPortLabel.margin = new RectOffset(0, 0, 0, 0);
                    _outPortLabel.padding = new RectOffset(0, 0, 1, 1);
                    _outPortLabel.fontSize = 12;
                    _outPortLabel.alignment = TextAnchor.MiddleRight;
                }

                return _outPortLabel;
            }
        }

        private static GUIStyle _portSymbol;
        public static GUIStyle portSymbol
        {
            get
            {
                if (_portSymbol == null)
                {
                    _portSymbol = new GUIStyle(EditorStyles.label);
                    _portSymbol.margin = new RectOffset(0, 0, 0, 0);
                    _portSymbol.padding = new RectOffset(2, 0, 0, 0);
                    _portSymbol.fontSize = 8;
                    _portSymbol.alignment = TextAnchor.MiddleCenter;
                }

                return _portSymbol;
            }
        }

        private static GUIStyle _portTipBox;
        public static GUIStyle portTipBox
        {
            get
            {
                if (_portTipBox == null)
                {
                    _portTipBox = new GUIStyle(EditorStyles.label);
                    _portTipBox.normal.background = Texture2D.whiteTexture;
                    _portTipBox.margin = new RectOffset(0, 0, 0, 0);
                    _portTipBox.padding = new RectOffset(1, 1, 1, 1);
                    _portTipBox.fontSize = 12;
                    _portTipBox.alignment = TextAnchor.MiddleLeft;
                }

                return _portTipBox;
            }
        }

        private static GUIStyle _loggerBox;
        public static GUIStyle loggerBox
        {
            get
            {
                if (_loggerBox == null)
                {
                    _loggerBox = new GUIStyle(EditorStyles.label);
                    _loggerBox.margin = new RectOffset(0, 0, 0, 0);
                    _loggerBox.padding = new RectOffset(0, 0, 0, 0);
                    _loggerBox.fontSize = 12;
                    _loggerBox.alignment = TextAnchor.MiddleLeft;
                    _loggerBox.wordWrap = true;
                }

                return _loggerBox;
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