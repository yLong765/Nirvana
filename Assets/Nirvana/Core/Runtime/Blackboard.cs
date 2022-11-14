using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Blackboard : ISerializer
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
        
        public void Serialize()
        {
            
        }

        public void Deserialize()
        {
            throw new System.NotImplementedException();
        }
    }
}