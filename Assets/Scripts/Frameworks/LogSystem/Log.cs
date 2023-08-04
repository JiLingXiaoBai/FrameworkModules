using System;
using UnityEngine;

namespace JLXB.Framework.LogSystem
{
    public class Log
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            string stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("Log.cs"))
            {
                System.Text.RegularExpressions.Match matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                while (matches.Success)
                {
                    string pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("JLXB.Framework.LogSystem"))
                    {
                        int splitIndex = pathLine.LastIndexOf(":");
                        string path = pathLine[..splitIndex];
                        line = Convert.ToInt32(pathLine[(splitIndex + 1)..]);
                        string fullPath = Application.dataPath[..Application.dataPath.LastIndexOf("Assets")];
                        fullPath += path;
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        private static string GetStackTrace()
        {
            var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                {
                    fieldInfo = ConsoleWindowType.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                    return activeText;
                }
            }
            return null;
        }
#endif
        public static void EnableLog(bool enable)
        {
            LogSystem.Instance.EnableLog = enable;
        }

        public static void SetLogLevel(LogLevel level)
        {
            LogSystem.Instance.LogLevel = level;
        }

        public static void RegisterLogMessage()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        public static void UnRegisterLogMessage()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        private static void HandleLog(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    HandleLog(LogLevel.ERROR, condition, stackTrace);
                    break;
                case LogType.Log:
                    HandleLog(LogLevel.DEBUG, condition, stackTrace);
                    break;
                case LogType.Exception:
                    HandleLog(LogLevel.ERROR, condition, stackTrace);
                    break;
                case LogType.Warning:
                    HandleLog(LogLevel.WARN, condition, stackTrace);
                    break;
                case LogType.Assert:
                    HandleLog(LogLevel.ALL, condition, stackTrace);
                    break;
            }
        }

        private static void HandleLog(LogLevel level, string condition, string stackTrace)
        {
            LogSystem.Instance.HandleLog(level, condition, stackTrace);
        }

        public static void LoadAppenders(AppenderType type)
        {
            switch (type)
            {
                case AppenderType.Console:
                    LogSystem.Instance.LoadAppenders(type, ConsoleAppender.Instance);
                    break;

                    // case AppenderType.File:
                    //     Logging.Instance.LoadAppenders(type, FileAppender.Instance);
                    //     break;
            }
        }


        public static void Debug(object message)
        {
            LogSystem.Instance.Debug(message);
        }
        public static void Debug(object message, string track)
        {
            LogSystem.Instance.Debug(message, track);
        }
        public static void Debug(object message, Exception e)
        {
            LogSystem.Instance.Debug(message, e);
        }
        public static void Debug(string format, params object[] args)
        {
            LogSystem.Instance.Debug(format, args);
        }

        public static void Info(object message)
        {
            LogSystem.Instance.Info(message);
        }
        public static void Info(object message, string track)
        {
            LogSystem.Instance.Info(message, track);
        }
        public static void Info(object message, Exception e)
        {
            LogSystem.Instance.Info(message, e);
        }
        public static void Info(string format, params object[] args)
        {
            LogSystem.Instance.Info(format, args);
        }

        public static void Warn(object message)
        {
            LogSystem.Instance.Warn(message);
        }
        public static void Warn(object message, string track)
        {
            LogSystem.Instance.Warn(message, track);
        }
        public static void Warn(object message, Exception e)
        {
            LogSystem.Instance.Warn(message, e);
        }
        public static void Warn(string format, params object[] args)
        {
            LogSystem.Instance.Warn(format, args);
        }

        public static void Error(object message)
        {
            LogSystem.Instance.Error(message);
        }
        public static void Error(object message, string track)
        {
            LogSystem.Instance.Error(message, track);
        }
        public static void Error(object message, Exception e)
        {
            LogSystem.Instance.Error(message, e);
        }
        public static void Error(string format, params object[] args)
        {
            LogSystem.Instance.Error(format, args);
        }

        public static void Fatal(object message)
        {
            LogSystem.Instance.Fatal(message);
        }
        public static void Fatal(object message, string track)
        {
            LogSystem.Instance.Fatal(message, track);
        }
        public static void Fatal(object message, Exception e)
        {
            LogSystem.Instance.Fatal(message, e);
        }
        public static void Fatal(string format, params object[] args)
        {
            LogSystem.Instance.Fatal(format, args);
        }
    }
}
