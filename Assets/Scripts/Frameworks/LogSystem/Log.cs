using System;
using UnityEditor;

namespace JLXB.Framework.LogSystem
{
    public class Log
    {
#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID)
        {
            var stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("Log.cs"))
            {
                var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                while (matches.Success)
                {
                    var pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("JLXB.Framework.LogSystem"))
                    {
                        var splitIndex = pathLine.LastIndexOf(":", StringComparison.Ordinal);
                        var path = pathLine[..splitIndex];
                        var line = Convert.ToInt32(pathLine[(splitIndex + 1)..]);
                        var fullPath = UnityEngine.Application.dataPath[
                            ..UnityEngine.Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal)];
                        fullPath += path;
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'),
                            line);
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
            var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (fieldInfo == null) return null;
            var consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                if (EditorWindow.focusedWindow == (EditorWindow)consoleInstance)
                {
                    fieldInfo = consoleWindowType.GetField("m_ActiveText",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (fieldInfo == null) return null;
                    var activeText = fieldInfo.GetValue(consoleInstance).ToString();
                    return activeText;
                }
            }

            return null;
        }
#endif

        private static string FormatTrack => LogSystem.Instance.GetFormatTrack();
        private static string ExceptionTrack { get; set; }
        private static string BriefnessTrack => LogSystem.Instance.GetBriefnessTrack();
        private static string LogContent { get; set; }

        private static string FormatString(object message, LogLevel level, string track)
        {
            return $"{message}\n[{level,-5}] {track}\n";
        }

        public static void Debug(object message)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.DEBUG)) return;
            LogContent = FormatString(message, LogLevel.DEBUG, BriefnessTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Debug(object message, string track)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.DEBUG)) return;
            LogContent = FormatString(message, LogLevel.DEBUG, track);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Debug(object message, Exception e)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.DEBUG)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.DEBUG, ExceptionTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Debug(string format, params object[] args)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.DEBUG)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.DEBUG, FormatTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Info(object message)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.INFO)) return;
            LogContent = FormatString(message, LogLevel.INFO, BriefnessTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Info(object message, string track)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.INFO)) return;
            LogContent = FormatString(message, LogLevel.INFO, track);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Info(object message, Exception e)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.INFO)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.INFO, ExceptionTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Info(string format, params object[] args)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.INFO)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.INFO, FormatTrack);
            UnityEngine.Debug.Log(LogContent);
        }

        public static void Warn(object message)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.WARN)) return;
            LogContent = FormatString(message, LogLevel.WARN, BriefnessTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }

        public static void Warn(object message, string track)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.WARN)) return;
            LogContent = FormatString(message, LogLevel.WARN, track);
            UnityEngine.Debug.LogWarning(LogContent);
        }

        public static void Warn(object message, Exception e)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.WARN)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.WARN, ExceptionTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }

        public static void Warn(string format, params object[] args)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.WARN)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.WARN, FormatTrack);
            UnityEngine.Debug.LogWarning(LogContent);
        }

        public static void Error(object message)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.ERROR)) return;
            LogContent = FormatString(message, LogLevel.ERROR, BriefnessTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Error(object message, string track)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.ERROR)) return;
            LogContent = FormatString(message, LogLevel.ERROR, track);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Error(object message, Exception e)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.ERROR)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.ERROR, ExceptionTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Error(string format, params object[] args)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.ERROR)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.ERROR, FormatTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Fatal(object message)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.FATAL)) return;
            LogContent = FormatString(message, LogLevel.FATAL, BriefnessTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Fatal(object message, string track)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.FATAL)) return;
            LogContent = FormatString(message, LogLevel.FATAL, track);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Fatal(object message, Exception e)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.FATAL)) return;
            ExceptionTrack = LogSystem.Instance.GetExceptionTrack(e);
            LogContent = FormatString(message, LogLevel.FATAL, ExceptionTrack);
            UnityEngine.Debug.LogError(LogContent);
        }

        public static void Fatal(string format, params object[] args)
        {
            if (!LogSystem.Instance.IsOutputLog(LogLevel.FATAL)) return;
            LogContent = FormatString(string.Format(format, args), LogLevel.FATAL, FormatTrack);
            UnityEngine.Debug.LogError(LogContent);
        }
    }
}