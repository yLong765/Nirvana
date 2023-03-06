#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public static class Prefs
    {
        private static readonly string PREFS_KEY_NAME = "Nirvana.Prefs";
        
        private class PreferenceData
        {
            public bool showDebugMode = false;
            public bool showBlackboardPanel = true;
            public float blackboardWidth = 300f;
            public float nodeInspectorWidth = 300f;
        }

        private static PreferenceData _data;
        private static PreferenceData data => _data ??= new PreferenceData();

        private static void Save()
        {
            EditorPrefs.SetString(PREFS_KEY_NAME, JsonConvert.SerializeObject(data));
        }
        
        [InitializeOnLoadMethod]
        private static void LoadData()
        {
            var pref = EditorPrefs.GetString(PREFS_KEY_NAME);
            if (!string.IsNullOrEmpty(pref))
            {
                _data = JsonConvert.DeserializeObject<PreferenceData>(pref);
            }
            _data ??= new PreferenceData();
        }

        public static bool showDebugModel
        {
            get => data.showDebugMode;
            set
            {
                data.showDebugMode = value;
                Save();
            }
        }
        
        public static bool showBlackboardPanel
        {
            get => data.showBlackboardPanel;
            set
            {
                data.showBlackboardPanel = value;
                Save();
            }
        }

        public static float blackboardWidth
        {
            get => data.blackboardWidth;
            set
            {
                data.blackboardWidth = Mathf.Clamp(value, 300, 500f);
                Save();
            }
        }

        public static float nodeInspectorPanelWidth
        {
            get => data.nodeInspectorWidth;
            set
            {
                data.nodeInspectorWidth = Mathf.Clamp(value, 300, 500f);
                Save();
            }
        }
    }
}
#endif