#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.PartialEditor
{
    public class DefaultObjectDrawer : ObjectDrawer<object>
    {
        protected override object OnGUI(GUIContent content, object value)
        {
            return EditorUtils.TypeField(info.Name, value, info.FieldType);
        }
    }
}
#endif