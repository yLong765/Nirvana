using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class BlackboardSource
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
        public Dictionary<string, Variable> variables
        {
            get => _variables;
            set => _variables = value;
        }
        
        public Variable AddVariable(Type type, string varName)
        {
            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable = (Variable) Activator.CreateInstance(variableType);
            newVariable.name = varName;
            variables[varName] = newVariable;
            return newVariable;
        }

        public void DelVariable(string varName)
        {
            if (_variables.ContainsKey(varName))
            {
                _variables.Remove(varName);
            }
        }
    }
}