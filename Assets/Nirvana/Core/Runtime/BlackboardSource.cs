using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nirvana
{
    /// <summary>
    /// 黑板元数据。只有这里的数据会被序列化保存
    /// </summary>
    public class BlackboardSource
    {
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
        /// <summary>
        /// 变量字典
        /// </summary>
        public Dictionary<string, Variable> variables
        {
            get => _variables;
            set => _variables = value;
        }
        
        /// <summary>
        /// 新增变量
        /// </summary>
        public Variable AddVariable(Type type, string varName)
        {
            while (variables.ContainsKey(varName)) varName += ".";
            
            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable = (Variable) Activator.CreateInstance(variableType);
            newVariable.name = varName;
            variables[varName] = newVariable;
            return newVariable;
        }

        public Variable GetVariableByID(string ID)
        {
           return variables.Values.First(variable => variable.ID == ID);
        }
        
        /// <summary>
        /// 删除变量
        /// </summary>
        public void DelVariable(string varName)
        {
            if (_variables.ContainsKey(varName))
            {
                _variables.Remove(varName);
            }
        }
    }
}