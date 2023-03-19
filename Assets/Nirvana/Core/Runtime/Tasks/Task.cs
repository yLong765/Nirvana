using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nirvana
{
    public abstract class Task
    {
        private string _title;
        private string _description;

        public string title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                {
                    if (GetType().TryGetAttribute<TitleAttribute>(out var attribute))
                    {
                        _title = attribute.title;
                    }
                    else
                    {
                        _title = GetType().Name;
                        if (_title.EndsWith("Task"))
                        {
                            _title = _title[.._title.LastIndexOf("Task", StringComparison.Ordinal)];
                        }
                    }
                }
                
                return _title;
            }
        }

        public string description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    if (GetType().TryGetAttribute<DescriptionAttribute>(out var attribute))
                    {
                        _description = attribute.description;
                    }
                }
                
                return _description;
            }
        }

        public virtual string nodeInfo => title;

        protected virtual void OnCreate() { }

        public static Task Create(Type type)
        {
            var newTask = (Task) Activator.CreateInstance(type);
            newTask.OnCreate();
            return newTask;
        }
    }
}