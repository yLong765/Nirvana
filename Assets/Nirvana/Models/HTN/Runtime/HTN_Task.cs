using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.HTN
{
    public abstract class HTN_Task
    {
        private List<HTN_PreCondition> preConditions = new List<HTN_PreCondition>();
        protected HTNTaskStatus status = HTNTaskStatus.Ready;

        public void AddPreCondition(HTN_PreCondition preCondition)
        {
            if (!preConditions.Contains(preCondition) || preCondition != null)
            {
                preConditions.Add(preCondition);
            }
        }

        public void RemovePreCondition(HTN_PreCondition preCondition)
        {
            if (preConditions.Contains(preCondition))
            {
                preConditions.Remove(preCondition);
            }
        }
        
        public bool CheckPreCondition()
        {
            var result = true;
            foreach (var preCondition in preConditions)
            {
                result &= preCondition.Invoke();
                if (!result)
                {
                    return false;
                }
            }

            return true;
        }

        public abstract HTNTaskStatus Execute();
    }
}