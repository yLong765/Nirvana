#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public class VariableDrawer : ObjectDrawer<Variable>
    {
        private static Variable _selectVariable;

        protected override Variable OnGUI(GUIContent content, Variable variable)
        {
            if (_selectVariable != null)
            {
                variable = _selectVariable;
                _selectVariable = null;
                return variable;
            }

            if (variable != null && !variable.useBlackboard)
            {
                GUILayout.BeginHorizontal();
                variable.value = EditorUtils.TypeField(content, variable.value, variable.type);
                if (GUILayout.Button(StyleUtils.fixIconTexture, StyleUtils.fixIcon))
                {
                    variable = null;
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
                
                var selectVariableName = variable == null ? "[NONE]" : variable.name;
                var rect = EditorGUILayout.GetControlRect(false);
                if (GUI.Button(rect, selectVariableName, EditorStyles.popup))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("[NONE]"), false, () => { _selectVariable = null; });

                    Type genericType = null;
                    if (info.FieldType.IsGenericType)
                    {
                        genericType = info.FieldType.GetGenericArguments()[0];
                    }

                    if (genericType != null)
                    {
                        foreach (var pair in GraphUtils.currentGraph.variables.Where(pair => pair.Value.type == genericType))
                        {
                            menu.AddItem(new GUIContent($"Graph/{pair.Key}"), false, () => { _selectVariable = pair.Value; });
                        }
                    }
                    
                    menu.AddItem(new GUIContent("(Custom Value)"), false, () =>
                    {
                        _selectVariable = Activator.CreateInstance(info.FieldType) as Variable;
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