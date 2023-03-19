using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public class UndoUtils
    {
        public static void RecordObject(Object objectToUndo, string name)
        {
            Undo.RecordObject(objectToUndo, name);
        }

        public static void SetDirty(Object target)
        {
            EditorUtility.SetDirty(target);
        }
    }
}