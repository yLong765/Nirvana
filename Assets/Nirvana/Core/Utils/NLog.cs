using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nirvana
{
    public enum LogTag
    {
        Error,
        Warning,
        Normal,
    }

    public class Log
    {
        public string value;
        public LogTag tag;
        private DateTime endTime;

        public Log(string value, LogTag tag, float duration)
        {
            this.value = value;
            this.tag = tag;
            this.endTime = DateTime.Now.AddSeconds(duration);
        }

        public bool IsDone()
        {
            return endTime <= DateTime.Now;
        }
    }

    public static class NLog
    {
        public static List<Log> allLogs => _logs;
        private static List<Log> _logs = new List<Log>();

        public static string GetLogIconName(LogTag logTag)
        {
            return logTag switch
            {
                LogTag.Normal => "console.infoicon",
                LogTag.Warning => "console.warnicon",
                _ => "console.erroricon"
            };
        }

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
            _logs.Add(new Log(value, LogTag.Error, duration));
            Debug.LogError(value);
        }

        public static void Warning(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogTag.Warning, duration));
            Debug.LogWarning(value);
        }

        public static void Normal(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogTag.Normal, duration));
            Debug.Log(value);
        }
    }
}