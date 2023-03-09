using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public class VariableDrawer : EditorDrawer<Variable>
    {
        protected override object OnGUI(GUIContent content, Variable variable)
        {
            var hasHideInInspector = fieldInfo.HasAttribute<HideInInspector>();
            if (!hasHideInInspector)
            {
                return EditorUtils.TypeField(content, variable.value, variable.type);
            }

            return null;
        }
    }
}