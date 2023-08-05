using System;
using UnityEngine;
using UnityEditor.Callbacks;

namespace JLXB.Framework.LogSystem
{
    public class Log
    {
#if UNITY_EDITOR
        [OnOpenAsset(0)]
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

        private static string FormatTrack { get { return LogSystem.Instance.GetFormatTrack(); } }
        private static string ExceptionTrack { get; set; }
        private static string BriefnessTrack { get { return LogSystem.Instance.GetBriefnessTrack(); } }
        private static string LogContent { get; set; }

        private static string FormatString(object message, LogLevel level, string track)
        {
            return string.Format("{0}\n[{1,-5}] {2}\n", message, level, track);
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
            LogSystem.Instance.LogRecord(level, condition, stackTrace);
        }

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
            Application.logMessageReceived += HandleLog;
        }

        public static void UnRegisterLogMessage()
        {
            Application.logMessageReceived -= HandleLog;
        }

        public static void LoadAppenders(AppenderType type)
        {
            switch (type)
            {
                case AppenderType.File:
                    LogSystem.Instance.LoadAppenders(type, FileAppender.Instance);
                    break;
            }
        }

        public static void Debug(object message)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.DEBUG)) return;
            LogContent = FormatString(message, LogLevel.DEBUG, BriefnessTrack);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Debug(object message, string track)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.DEBUG)) return;
            LogContent = FormatString(message, LogLevel.DEBUG, track);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Debug(object message, Exception e)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.DEBUG)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.DEBUG, ExceptionTrack);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Debug(string format, params object[] args)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.DEBUG)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.DEBUG, FormatTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Info(object message)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.INFO)) return;
            LogContent = FormatString(message, LogLevel.INFO, BriefnessTrack);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Info(object message, string track)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.INFO)) return;
            LogContent = FormatString(message, LogLevel.INFO, track);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Info(object message, Exception e)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.INFO)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.INFO, ExceptionTrack);
            UnityEngine.Debug.Log(LogContent);
        }
        public static void Info(string format, params object[] args)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.INFO)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.INFO, FormatTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Warn(object message)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.WARN)) return;
            LogContent = FormatString(message, LogLevel.WARN, BriefnessTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }
        public static void Warn(object message, string track)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.WARN)) return;
            LogContent = FormatString(message, LogLevel.WARN, track);
            UnityEngine.Debug.LogWarning(LogContent);
        }
        public static void Warn(object message, Exception e)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.WARN)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.WARN, ExceptionTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }
        public static void Warn(string format, params object[] args)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.WARN)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.WARN, FormatTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }

        public static void Error(object message)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.ERROR)) return;
            LogContent = FormatString(message, LogLevel.ERROR, BriefnessTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Error(object message, string track)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.ERROR)) return;
            LogContent = FormatString(message, LogLevel.ERROR, track);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Error(object message, Exception e)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.ERROR)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.ERROR, ExceptionTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Error(string format, params object[] args)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.ERROR)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.ERROR, FormatTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Fatal(object message)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.FATAL)) return;
            LogContent = FormatString(message, LogLevel.FATAL, BriefnessTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Fatal(object message, string track)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.FATAL)) return;
            LogContent = FormatString(message, LogLevel.FATAL, track);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Fatal(object message, Exception e)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.FATAL)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.FATAL, ExceptionTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
        public static void Fatal(string format, params object[] args)
        {
            if (!LogSystem.Instance.CheckOutPut(LogLevel.FATAL)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.FATAL, FormatTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
    }
}
