#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public class VariableDrawer : ObjectDrawer<BBVariable>
    {
        protected override BBVariable OnGUI(GUIContent content, BBVariable variable)
        {
            variable ??= Activator.CreateInstance(info.FieldType) as BBVariable;

            if (!variable.linkBlackboard)
            {
                GUILayout.BeginHorizontal();
                variable.value = EditorUtils.TypeField(content, variable.value, variable.type);
                if (GUILayout.Button(StyleUtils.fixIconTexture, StyleUtils.fixIcon))
                {
                    variable.LinkToBlackboard(true);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (content != GUIContent.none)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(content, GUI.skin.button);
                }
                
                var selectVariableName = variable.isNone ? "[NONE]" : variable.name;
                var rect = EditorGUILayout.GetControlRect(false);
                if (GUI.Button(rect, selectVariableName, EditorStyles.popup))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("[NONE]"), false, () => { variable.LinkToBlackboard(); });

                    Type genericType = null;
                    if (info.FieldType.IsGenericType)
                    {
                        genericType = info.FieldType.GetGenericArguments()[0];
                    }

                    if (genericType != null)
                    {
                        foreach (var pair in GraphUtils.currentGraph.variables.Where(pair => pair.Value.type == genericType))
                        {
                            menu.AddItem(new GUIContent($"Graph/{pair.Key}"), false, () =>
                            {
                                variable.LinkToBlackboard(pair.Value);
                            });
                        }
                    }
                    
                    menu.AddItem(new GUIContent("(Custom Value)"), false, () =>
                    {
                        variable.LinkToBlackboard();
                    });

                    menu.DropDown(rect);
                }
                
                if (content != GUIContent.none)
                {
                    GUILayout.EndHorizontal();
                }
            }

            return variable;
        }
    }
}
#endif