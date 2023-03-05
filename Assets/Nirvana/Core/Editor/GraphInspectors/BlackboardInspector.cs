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
        private static Graph _context;
        private static BlackboardSource _bbSource;
        
        public static void DrawGUI(Rect rect, BlackboardSource bbSource, Graph graph)
        {
            _context = graph;
            _bbSource = bbSource;
            
            EditorGUI.BeginChangeCheck();
            
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, rect.height), ColorUtils.gray21, StyleUtils.normalBG);
            var titleHeight = StyleUtils.panelTitle.CalcSize("Blackboard").y;
            EditorUtils.DrawBox(new Rect(0, 0, rect.width, titleHeight), ColorUtils.gray17, StyleUtils.normalBG);
            GUILayout.Label("Blackboard", StyleUtils.panelTitle);
            GUILayout.BeginArea(Rect.MinMaxRect(2, titleHeight + 2, rect.xMax - 2, rect.yMax - 2));
            
            if (GUILayout.Button("Add Variable"))
            {
                var menu = new GenericMenuPopup("Fields");
                var types = TypeUtils.GetChildTypes(typeof(object));
                foreach (var t in types)
                {
                    menu.AddItem(t.Name, () =>
                    {
                        Undo.RecordObject(_context, "New Variable");
                        bbSource.AddVariable(t, t.Name);
                        EditorUtility.SetDirty(_context);
                        GraphUtils.willSetDirty = true;
                    });
                }
                
                menu.Show();
            }
            
            if (_tempVariablesList == null || !_tempVariablesList.SequenceEqual(bbSource.variables.Values))
            {
                _tempVariablesList = bbSource.variables.Values.ToList();
            }

            if (_tempVariablesList != null && _tempVariablesList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(14);
                GUI.color = ColorUtils.gold;
                GUILayout.Label("Name", GUILayout.MaxWidth(80), GUILayout.ExpandWidth(true), GUILayout.MinHeight(18));
                GUILayout.Label("Value", _options);
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
            
            var options = new EditorUtils.ReorderableListOptions {context = graph, customItemMenu = i => GetCustomMenuItem(_tempVariablesList[i], i)};
            EditorUtils.ReorderableList(_tempVariablesList, options, i =>
            {
                DrawVariableItem(_tempVariablesList[i], i);
            });
            
            if (GUI.changed || Event.current.rawType == EventType.MouseUp)
            {
                try
                {
                    Undo.RecordObject(_context, "change or drag sort Variable");
                    bbSource.variables = _tempVariablesList.ToDictionary(d => d.name, d => d);
                    EditorUtility.SetDirty(_context);
                }
                catch
                {
                    LogUtils.Error("Blackboard has duplicate names!");
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                GraphUtils.willSetDirty = true;
            }
        }
        
        private static void DrawVariableItem(Variable variable, int id)
        {
            if (_tempVariablesList.Where(v => v != variable).Select(v => v.name).Contains(variable.name))
            {
                GUI.color = Color.red;
            }
            
            GUILayout.BeginHorizontal();
            variable.name = GUILayout.TextField(variable.name, _options);
            variable.value = EditorUtils.TypeField(GUIContent.none, variable.value, variable.type, _options);
            GUILayout.EndHorizontal();
            
            GUI.color = Color.white;
        }

        private static GenericMenu GetCustomMenuItem(Variable variable, int id)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Copy New Variable"), false, () =>
            {
                Undo.RecordObject(_context, "Copy New Variable");
                _bbSource.AddVariable(variable.type, variable.name);
                EditorUtility.SetDirty(_context);
            });
            menu.AddItem(new GUIContent("Delete Variable"), false, () =>
            {
                Undo.RecordObject(_context, "Delete Variable");
                _bbSource.DelVariable(variable.name);
                EditorUtility.SetDirty(_context);
            });
            return menu;
        }
    }
}