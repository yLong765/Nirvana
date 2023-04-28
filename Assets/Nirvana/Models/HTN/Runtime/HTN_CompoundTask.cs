using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana.HTN
{
    public class HTN_CompoundTask : HTN_Task
    {
        private List<HTN_Task> tasks = new List<HTN_Task>();
        private int _curTaskId = 0;
        private HTN_PlanRunner planRunner;

        public void AddTask(HTN_Task task)
        {
            if (!tasks.Contains(task))
            {
                tasks.Add(task);
            }
        }

        public void RemoveTask(HTN_Task task)
        {
            if (tasks.Contains(task))
            {
                tasks.Remove(task);
            }
        }

        private bool MakePlan(HTN_Task task, ref List<HTN_Task> plan)
        {
            switch (task)
            {
                case HTN_PrimitiveTask pTask:
                {
                    var result = pTask.CheckPreCondition();
                    if (!result) return false;
                    plan.Add(pTask);
                    break;
                }
                case HTN_CompoundTask cTask:
                {
                    var result = cTask.CheckPreCondition();
                    if (!result) return false;

                    foreach (var childTask in cTask.tasks)
                    {
                        result = MakePlan(childTask, ref plan);
                        if (!result) return false;
                    }

                    break;
                }
            }

            return true;
        }
        
        private bool TryFindPlan(out List<HTN_Task> plan)
        {
            while (_curTaskId < tasks.Count)
            {
                var curTask = tasks[_curTaskId];
                if (curTask.CheckPreCondition() )
                {
                    plan = new List<HTN_Task>();
                    var result = MakePlan(curTask, ref plan);
                    if (result) return true;
                }

                _curTaskId++;
            }

            plan = null;
            return false;
        }

        public override HTNTaskStatus Execute()
        {
            if (status == HTNTaskStatus.Running)
            {
                status = planRunner.Execute();
                return status;
            }

            if (_curTaskId >= tasks.Count) _curTaskId = 0;
            if (!TryFindPlan(out var plan))
            {
                status = HTNTaskStatus.Failure;
                return status;
            }
            
            planRunner = new HTN_PlanRunner(plan);
            if (planRunner != null)
            {
                status = HTNTaskStatus.Failure;
                return status;
            }

            status = planRunner.Execute();
            
            return status;
        }
    }
}