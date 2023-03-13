#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public class DefaultObjectDrawer : ObjectDrawer<object>
    {
        protected override object OnGUI(GUIContent content, object variable)
        {
            return EditorUtils.TypeField(info.Name, variable, info.FieldType);
        }
    }
}
#endif