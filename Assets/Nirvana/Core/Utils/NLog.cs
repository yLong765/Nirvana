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

    public static class NLog
    {
        public static List<Log> allLogs => _logs;
        private static List<Log> _logs = new List<Log>();

        public static string GetLogIconName(LogType logType)
        {
            return logType switch
            {
                LogType.Normal => "console.infoicon",
                LogType.Warning => "console.warnicon",
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
            _logs.Add(new Log(value, LogType.Error, duration));
            Debug.LogError(value);
        }

        public static void Warning(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogType.Warning, duration));
            Debug.LogWarning(value);
        }

        public static void Normal(string value, float duration = 5f)
        {
            CheckAllLog();
            _logs.Add(new Log(value, LogType.Normal, duration));
            Debug.Log(value);
        }
    }
}