using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nirvana.Editor
{
    public class BlackboardInspector : EditorWindow
    {
        private static List<Variable> _tempVariablesList;
        private static readonly GUILayoutOption[] _options = {GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.MinHeight(18)};

        public static void DrawGUI(Rect rect, Blackboard bb)
        {
            EditorGUI.BeginChangeCheck();
            
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, rect.height), ColorUtils.gray21, Styles.normalBG);
            var titleHeight = Styles.CalcSize(Styles.panelTitle, "Blackboard").y;
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, Styles.normalBG);
            GUILayout.Label("Blackboard", Styles.panelTitle);
            GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 2, rect.xMax - 2, rect.yMax - 2));
            if (GUILayout.Button("Add Variable"))
            {
                var menu = new GenericMenuPopup("Fields");
                var types = TypeUtils.GetChildTypes(typeof(object));
                foreach (var t in types)
                {
                    menu.AddItem(t.Name, () =>
                    {
                        bb.AddVariable(t, t.Name);
                        GraphUtils.willSetDirty = true;
                    });
                }
                
                menu.Show();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(16);
            GUI.color = ColorUtils.orange1;
            GUILayout.Label("Name", GUILayout.MaxWidth(75), GUILayout.ExpandWidth(true), GUILayout.MinHeight(18));
            GUILayout.Label("Value", _options);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (_tempVariablesList == null || !_tempVariablesList.SequenceEqual(bb.variables.Values)) {
                _tempVariablesList = bb.variables.Values.ToList();
            }
            EditorUtils.ReorderableList(_tempVariablesList, i =>
            {
                DrawVariableItem(bb, _tempVariablesList[i]);
            });
            
            if ( GUI.changed || Event.current.rawType == EventType.MouseUp ) {
                try
                {
                    bb.variables = _tempVariablesList.ToDictionary(d => d.name, d => d);
                }
                catch
                {
                    Debug.LogError("To Dictionary Error!");
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                GraphUtils.willSetDirty = true;
            }
        }
        
        private static void DrawVariableItem(Blackboard bb, Variable variable)
        { 
            GUILayout.BeginHorizontal();
            variable.name = GUILayout.TextField(variable.name, _options);
            variable.value = EditorUtils.TypeField(GUIContent.none, variable.value, variable.type, _options);
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                bb.variables.Remove(variable.name);
                GraphUtils.willSetDirty = true;
            }
            GUILayout.EndHorizontal();
        }
    }
}