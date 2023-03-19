using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class FlowTask : Task
    {
        private FlowStatus _status = FlowStatus.Ready;

        private bool Initialize()
        {
            OnInit();
            return true;
        }
        
        public FlowStatus Execute()
        {
            if (_status == FlowStatus.Running)
            {
                OnUpdate();
                return _status;
            }

            if (!Initialize())
            {
                _status = FlowStatus.Failure;
                return _status;
            }
            
            _status = FlowStatus.Running;
            OnExecute();
            if (_status == FlowStatus.Running)
            {
                OnUpdate();
            }

            return _status;
        }

        protected void Stop(bool success = true)
        {
            _status = success ? FlowStatus.Success : FlowStatus.Failure;
            Onstop();
        }

        protected void Error(string message)
        {
            Stop(false);
            NLog.Error(message);
        }

        protected virtual void OnInit() { }
        protected virtual void OnExecute() { }
        protected virtual void OnUpdate() { }
        protected virtual void Onstop() { }
    }

    public class FlowTask<T> : FlowTask
    {
         
    }
}