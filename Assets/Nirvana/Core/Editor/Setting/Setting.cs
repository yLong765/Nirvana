using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    [CreateAssetMenu(menuName = "Nirvana Tools/Nirvana Setting")]
    public class Setting : ScriptableObject
    {
        private static Setting _instance;
        public static Setting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Setting>("NirvanaSetting");
                }

                return _instance;
            }
        }

        public bool debugMode = false;
    }
}