using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.HTN
{
    public class HTN_PrimitiveTask : HTN_Task
    {
        private List<HTN_Effect> effects = new List<HTN_Effect>();

        public void AddEffect(HTN_Effect effect)
        {
            if (!effects.Contains(effect))
            {
                effects.Add(effect);
            }
            else
            {
                NLog.Warning("Add effect Failed: already add");
            }
        }

        public void RemoveEffect(HTN_Effect effect)
        {
            if (effects.Contains(effect))
            {
                effects.Remove(effect);
            }
            else
            {
                NLog.Warning("Remove effect Failed: don't find effect");
            }
        }

        public override HTNTaskStatus Execute()
        {
            if (status == HTNTaskStatus.Running)
            {
                OnUpdateAction();
                return status;
            }

            if (!CheckPreCondition())
            {
                status = HTNTaskStatus.Failure;
                return status;
            }

            status = HTNTaskStatus.Running;
            OnExecuteAction();
            if (status == HTNTaskStatus.Running)
            {
                OnUpdateAction();
            }

            return status;
        }

        protected void Stop(bool success = true)
        {
            status = success ? HTNTaskStatus.Success : HTNTaskStatus.Failure;
            OnStopAction();
        }

        protected void Error(string message)
        {
            Stop(false);
            NLog.Error(message);
        }

        #region Virtual

        protected virtual void OnExecuteAction()
        {
        }

        protected virtual void OnUpdateAction()
        {
        }

        protected virtual void OnStopAction()
        {
        }

        #endregion
    }
}
