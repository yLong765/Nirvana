using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    [Serializable]
    public class Blackboard : ISerializer
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public Dictionary<string, Variable> variables => _variables;

        public void Serialize()
        {
            
        }

        public void Deserialize()
        {
            throw new System.NotImplementedException();
        }
    }
}