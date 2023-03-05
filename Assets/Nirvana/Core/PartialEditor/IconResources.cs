using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [CreateAssetMenu(menuName = "Nirvana Tools/Nirvana Icon Resources", order = 81)]
    public class IconResources : ScriptableObject
    {
        private static IconResources _instance;
        public static IconResources Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<IconResources>("IconResources");
                }

                return _instance;
            }
        }

        [Serializable]
        public class Icon
        {
            public Texture2D bezierTexture;
            public Texture flowIconTexture;
            public Texture settingIconTexture;
        }

        [SerializeField] public Icon icon;
    }
}