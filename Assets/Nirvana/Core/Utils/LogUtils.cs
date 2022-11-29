using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public enum LogType
    {
        Error,
        Warning,
        Normal,
    }
    
    public class Log
    {
        public string value;
        public LogType type;
        private DateTime endTime;

        public Log(string value, LogType type, float duration)
        {
            this.value = value;
            this.type = type;
            this.endTime = DateTime.Now.AddSeconds(duration);
        }

        public bool IsDone()
        {
            return endTime <= DateTime.Now;
        }
    }
    
    public static class LogUtils
    {
        private static List<Log> _logs = new List<Log>();

        public static List<Log> allLogs => _logs;

        public static void CheckAllLog()
        {
            for (int i = _logs.Count - 1; i >= 0; i--)
            {
                if (_logs[i].IsDone())
                {
                    _logs.RemoveAt(i);
                }
            }
        }
        
        public static void Error(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogType.Error, duration));
        }

        public static void Warning(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogType.Warning, duration));
        }

        public static void Normal(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogType.Normal, duration));
        }
    }
}