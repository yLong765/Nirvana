using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public class FlowTask : Task
    {
        public void Execute()
        {
            if (OnInit())
            {
                OnExecute();
            }
        }

        protected void Error(string message)
        {
            NLog.Error(message);
        }

        protected virtual bool OnInit() { return true; }
        protected virtual void OnExecute() { }
    }

    public class FlowTaskFunc<RT> : FlowTask
    {
        public new RT Execute()
        {
            if (OnInit())
            {
                return OnExecute();
            }

            return default;
        }

        protected new virtual RT OnExecute() { return default; }
    }

    public class FlowTaskFunc<RT, T1> : FlowTask
    {
        public RT Execute(T1 t1)
        {
            if (OnInit())
            {
                return OnExecute(t1);
            }

            return default;
        }

        protected virtual RT OnExecute(T1 t1) { return default; }
    }
    
    public class FlowTaskAction<T> : FlowTask
    {
        public void Execute(T t1)
        {
            if (OnInit())
            {
                OnExecute(t1);
                FlowTaskFunc<bool, bool> ftf = new FlowTaskFunc<bool, bool>();
                ftf.Execute();
            }
        }

        protected virtual void OnExecute(T t1) { }
    }
}