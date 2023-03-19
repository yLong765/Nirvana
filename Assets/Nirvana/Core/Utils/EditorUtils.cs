using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nirvana.Editor;
using ParadoxNotion;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Nirvana
{
    public static class EditorUtils
    {
        /// <summary>
        /// 绘制Box
        /// </summary>
        public static void DrawBox(Rect rect, Color color, GUIStyle style)
        {
            GUI.color = color;
            GUI.Box(rect, string.Empty, style);
            GUI.color = Color.white;
        }

        public static void DrawBox(Rect rect, string title, Color backboardColor, GUIStyle style)
        {
            GUI.backgroundColor = backboardColor;
            GUI.Box(rect, title, style);
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// 绘制Window
        /// </summary>
        public static Rect Window(int id, Rect rect, GUI.WindowFunction func, Color color, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            GUI.color = color;
            var newRect = GUILayout.Window(id, rect, func, string.Empty, style, layoutOptions);
            GUI.color = Color.white;
            return newRect;
        }

        /// <summary>
        /// 绘制搜索窗口
        /// </summary>
        public static string SearchField(string search)
        {
            GUILayout.BeginHorizontal();
            search = EditorGUILayout.TextField(search, StyleUtils.toolbarSearchTextField);
            if (!string.IsNullOrEmpty(search) && GUILayout.Button(string.Empty, StyleUtils.toolbarSearchCancelButton))
            {
                search = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            GUILayout.EndHorizontal();
            return search;
        }

        /// <summary>
        /// 绘制有默认文字的文本框
        /// </summary>
        public static string DefaultTextField(string value, string defaultText)
        {
            var check = EditorGUILayout.TextField(value);
            if (string.IsNullOrEmpty(check))
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                GUI.Label(lastRect, defaultText, StyleUtils.defaultLabel);
            }

            return check;
        }

        private static IList pickedList;
        private static int pickedListIndex = -1;
        
        public struct ReorderableListOptions
        {
            public delegate GenericMenu GetCustomItemMenu(int i);
            public Graph context;
            public GetCustomItemMenu customItemMenu;
        }
        
        public static void ReorderableList(IList list, ReorderableListOptions options, System.Action<int> itemGUI)
        {
            if (list == null) return;

            var e = Event.current;

            for (int i = 0; i < list.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.BeginVertical();
                itemGUI(i);
                GUILayout.EndVertical();
                var lastRect = GUILayoutUtility.GetLastRect();
                var pickRect = Rect.MinMaxRect(lastRect.xMin - 16, lastRect.yMin - 1, lastRect.xMin, lastRect.yMax);
                GUI.Label(pickRect, "☰");
                if (options.customItemMenu != null)
                {
                    GUILayout.Space(18);
                    var buttonRect = Rect.MinMaxRect(lastRect.xMax + 1, lastRect.yMin - 1, lastRect.xMax + 19, lastRect.yMax);
                    if (GUI.Button(buttonRect, new GUIContent(StyleUtils.settingIconTexture), StyleUtils.variableSettingIcon))
                    {
                        UndoUtils.RecordObject(options.context, "Menu Item");
                        options.customItemMenu(i).ShowAsContext();
                        UndoUtils.SetDirty(options.context);
                    }
                }
                GUILayout.EndHorizontal();

                EditorGUIUtility.AddCursorRect(pickRect, MouseCursor.MoveArrow);
                var boundRect = GUILayoutUtility.GetLastRect();

                if (pickRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                {
                    pickedList = list;
                    pickedListIndex = i;
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                    e.Use();
                }

                if (pickedList == list)
                {
                    if (pickedListIndex == i)
                    {
                        GUI.Box(boundRect, string.Empty);
                    }

                    if (pickedListIndex != -1 && pickedListIndex != i && boundRect.Contains(e.mousePosition))
                    {
                        var markRect = new Rect(boundRect.x, boundRect.y - 2, boundRect.width, 2);
                        if (pickedListIndex < i)
                        {
                            markRect.y = boundRect.yMax - 2;
                        }

                        GUI.DrawTexture(markRect, Texture2D.whiteTexture);
                        if (e.type == EventType.MouseUp)
                        {
                            var pickObj = list[pickedListIndex];
                            list.RemoveAt(pickedListIndex);
                            list.Insert(i, pickObj);
                            GUI.changed = true;
                            pickedList = null;
                            pickedListIndex = -1;
                            e.Use();
                        }
                    }
                }
            }

            if (e.rawType == EventType.MouseUp)
            {
                if (list == pickedList)
                {
                    pickedList = null;
                    pickedListIndex = -1;
                }
            }
        }

        public static object TypeField(string title, object value, Type t, params GUILayoutOption[] options)
        {
            return TypeField(new GUIContent(title), value, t, options);
        }

        public static object TypeField(GUIContent content, object value, Type t, params GUILayoutOption[] options)
        {
            //Check scene object type for UnityObjects. Consider Interfaces as scene object type. Assume that user uses interfaces with UnityObjects
            // if ( typeof(UnityObject).IsAssignableFrom(t) || t.IsInterface ) {
            //     if ( value == null || value is UnityObject ) { //check this to avoid case of interface but no unityobject
            //         var isSceneObjectType = ( typeof(Component).IsAssignableFrom(t) || t == typeof(GameObject) || t.IsInterface );
            //         var newValue = EditorGUILayout.ObjectField(content, (UnityObject)value, t, isSceneObjectType, options);
            //         if ( unityObjectContext != null && newValue != null ) {
            //             if ( !Application.isPlaying && EditorUtility.IsPersistent(unityObjectContext) && !EditorUtility.IsPersistent(newValue as UnityEngine.Object) ) {
            //                 ParadoxNotion.Services.Logger.LogWarning("Assets can not have scene object references", "Editor", unityObjectContext);
            //                 newValue = value as UnityObject;
            //             }
            //         }
            //         return newValue;
            //     }
            // }

            //Check Type second
            // if ( t == typeof(Type) ) {
            //     return Popup<Type>(content, (Type)value, TypePrefs.GetPreferedTypesList(true), options);
            // }

            //get real current type
            t = value != null ? value.GetType() : t;

            //for these just show type information
            if (t.IsAbstract || t == typeof(object) || typeof(Delegate).IsAssignableFrom(t) ||
                typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(t))
            {
                EditorGUILayout.LabelField(content, new GUIContent(string.Format("({0})", t.FriendlyName())), options);
                return value;
            }

            //create instance for value types
            if (value == null && t.IsValueType)
            {
                value = Activator.CreateInstance(t);
            }

            //create new instance with button for non value types
            if (value == null && !t.IsAbstract && !t.IsInterface && (t.IsArray || t.GetConstructor(Type.EmptyTypes) != null))
            {
                if (content != GUIContent.none)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(content, GUI.skin.button);
                }

                if (GUILayout.Button("(null) Create", options))
                {
                    value = t.IsArray ? Array.CreateInstance(t.GetElementType(), 0) : Activator.CreateInstance(t);
                }

                if (content != GUIContent.none)
                {
                    GUILayout.EndHorizontal();
                }

                return value;
            }


            if (t == typeof(string))
            {
                return EditorGUILayout.TextField(content, (string) value, options);
            }

            if (t == typeof(char))
            {
                var c = (char) value;
                var s = c.ToString();
                s = EditorGUILayout.TextField(content, s, options);
                return string.IsNullOrEmpty(s) ? (char) c : (char) s[0];
            }

            if (t == typeof(bool))
            {
                return EditorGUILayout.Toggle(content, (bool) value, options);
            }

            if (t == typeof(int))
            {
                return EditorGUILayout.IntField(content, (int) value, options);
            }

            if (t == typeof(float))
            {
                return EditorGUILayout.FloatField(content, (float) value, options);
            }

            if (t == typeof(byte))
            {
                return Convert.ToByte(Mathf.Clamp(EditorGUILayout.IntField(content, (byte) value, options), 0, 255));
            }

            if (t == typeof(long))
            {
                return EditorGUILayout.LongField(content, (long) value, options);
            }

            if (t == typeof(double))
            {
                return EditorGUILayout.DoubleField(content, (double) value, options);
            }

            if (t == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(content, (Vector2) value, options);
            }

            if (t == typeof(Vector2Int))
            {
                return EditorGUILayout.Vector2IntField(content, (Vector2Int) value, options);
            }

            if (t == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(content, (Vector3) value, options);
            }

            if (t == typeof(Vector3Int))
            {
                return EditorGUILayout.Vector3IntField(content, (Vector3Int) value, options);
            }

            if (t == typeof(Vector4))
            {
                return EditorGUILayout.Vector4Field(content, (Vector4) value, options);
            }

            if (t == typeof(Quaternion))
            {
                var quat = (Quaternion) value;
                var vec4 = new Vector4(quat.x, quat.y, quat.z, quat.w);
                vec4 = EditorGUILayout.Vector4Field(content, vec4, options);
                return new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
            }

            if (t == typeof(Color))
            {
                // var att = attributes?.FirstOrDefault(a => a is ColorUsageAttribute) as ColorUsageAttribute;
                // var hdr = att != null ? att.hdr : false;
                // var showAlpha = att != null ? att.showAlpha : true;
                return EditorGUILayout.ColorField(content, (Color) value, true, true, false, options);
            }

            if (t == typeof(Gradient))
            {
                return EditorGUILayout.GradientField(content, (Gradient) value, options);
            }

            if (t == typeof(Rect))
            {
                return EditorGUILayout.RectField(content, (Rect) value, options);
            }

            if (t == typeof(AnimationCurve))
            {
                return EditorGUILayout.CurveField(content, (AnimationCurve) value, options);
            }

            if (t == typeof(Bounds))
            {
                return EditorGUILayout.BoundsField(content, (Bounds) value, options);
            }

            // if ( t == typeof(LayerMask) ) {
            //     return LayerMaskField(content, (LayerMask)value, options);
            // }

            if (t.IsSubclassOf(typeof(System.Enum)))
            {
                if (t.RTIsDefined(typeof(FlagsAttribute), true))
                {
                    return EditorGUILayout.EnumFlagsField(content, (System.Enum) value, options);
                }

                return EditorGUILayout.EnumPopup(content, (System.Enum) value, options);
            }

            return value;
        }

        public static void ShowChildTypeGenericMenu(string title, Type baseType, Action<Type> clickAction)
        {
            var menu = new GenericMenuPopup(title);
            var types = TypeUtils.GetSubClassTypes(baseType);
            foreach (var t in types)
            {
                menu.AddItem(t.Name, () =>
                {
                    clickAction(t);
                });
            }

            menu.Show();
        }

        public static void ShowChildTypeGenericMenu(Type baseType, Action<Type> clickAction)
        {
            ShowChildTypeGenericMenu(baseType.Name, baseType, clickAction);
        }
    }
}