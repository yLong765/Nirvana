using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.HTN
{
    public class HTN_PlanRunner
    {
        private List<HTN_Task> _tasks;
        private int _curTask;

        public HTN_PlanRunner(List<HTN_Task> tasks)
        {
            _tasks = tasks;
            _curTask = 0;
        }

        public HTNTaskStatus Execute()
        {
            while (_curTask < _tasks.Count)
            {
                var task = _tasks[_curTask];
                var taskStatus = task.Execute();
                switch (taskStatus)
                {
                    case HTNTaskStatus.Success:
                        _curTask++;
                        break;
                    case HTNTaskStatus.Ready:
                    case HTNTaskStatus.Running:
                    case HTNTaskStatus.Failure:
                        return taskStatus;
                }
            }

            return HTNTaskStatus.Success;
        }
    }
}