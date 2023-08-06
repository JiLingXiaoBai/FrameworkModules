using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
namespace JLXB.Framework.LogSystem
{
    public enum LogLevel
    {
        ALL,   // 最低等级的，用于打开所有日志记录；
        DEBUG, // 主要用于开发过程中打印一些运行信息；
        INFO,  // 打印一些你感兴趣的或者重要的信息，这个可以用于生产环境中输出程序运行的一些重要信息；
        WARN,  // 表明会出现潜在错误的情形，给开发人员一些提示；
        ERROR, // 虽然发生错误事件，但仍然不影响系统的继续运行，打印错误和异常信息；
        FATAL, // 指出每个严重的错误事件将会导致应用程序的退出，重大错误，可以直接停止程序；
        OFF,   // 最高等级的，用于关闭所有日志记录；
    }

    public struct LogData
    {
        public string logID;        // 日志ID
        public LogLevel logLevel;   // 日志级别
        public DateTime logTime;    // 日志时间
        public object logMessage;   // 日志消息
        public string logBasicData; // 日志基础数据
        public string logTrack;     // 日志堆栈信息
    }

    public interface ILogAppender
    {
        void OnLoad();
        void OnUnload();
        void Log(LogData data);
    }

    public enum AppenderType
    {
        File,          // 文件输出
    }

    public class LogSystem : Singleton<LogSystem>
    {
        private readonly Dictionary<AppenderType, ILogAppender> _allAppenders = new();

        private readonly List<LogLevel> _ignoreLevel = new();

        public bool EnableLog { get; set; }

        public LogLevel LogLevel { get; set; }

        private LogSystem() { }

        private bool _isInited = false;
        public void Init()
        {
            if (_isInited) return;
            _isInited = true;
            EnableLog = true;
            LogLevel = LogLevel.ALL;
            LoadAppender(AppenderType.File);
            RegisterLogMessage();
            MonoMgr.Instance.AddDestroyListener(OnDestroy);
        }

        public void OnDestroy()
        {
            foreach (var appender in _allAppenders)
            {
                appender.Value.OnUnload();
            }
            _allAppenders.Clear();
            UnRegisterLogMessage();
        }

        private StackTrace StackTrace
        {
            get { return new StackTrace(4, true); }
        }

        public void RegisterLogMessage()
        {
            Application.logMessageReceived += HandleLog;
        }

        public void UnRegisterLogMessage()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    LogRecord(LogLevel.ERROR, condition, stackTrace);
                    break;
                case LogType.Log:
                    LogRecord(LogLevel.DEBUG, condition, stackTrace);
                    break;
                case LogType.Exception:
                    LogRecord(LogLevel.ERROR, condition, stackTrace);
                    break;
                case LogType.Warning:
                    LogRecord(LogLevel.WARN, condition, stackTrace);
                    break;
                case LogType.Assert:
                    LogRecord(LogLevel.ALL, condition, stackTrace);
                    break;
            }
        }

        public void LoadAppender(AppenderType appenderType)
        {
            if (_allAppenders.ContainsKey(appenderType)) return;
            switch (appenderType)
            {
                case AppenderType.File:
                    _allAppenders.Add(appenderType, FileAppender.Instance);
                    FileAppender.Instance.OnLoad();
                    break;
            }

        }

        public void UnLoadAppender(AppenderType appenderType)
        {
            if (!_allAppenders.ContainsKey(appenderType)) return;

            switch (appenderType)
            {
                case AppenderType.File:
                    FileAppender.Instance.OnUnload();
                    break;
            }
            _allAppenders.Remove(appenderType);
        }

        /// <summary>
        /// For these added level, the log does not do output.(Except for error output)
        /// </summary>
        /// <param name="level"></param>
        public void IgnoreLevel(LogLevel level)
        {
            if (!_ignoreLevel.Contains(level))
                _ignoreLevel.Add(level);
        }
        public void UnIgnoreLevel(LogLevel level)
        {
            if (_ignoreLevel.Contains(level))
                _ignoreLevel.Remove(level);
        }

        public bool IsOutputLog(LogLevel level)
        {
            if (!EnableLog || (int)LogLevel > (int)level || _ignoreLevel.Contains(level)) return false;
            return true;
        }

        private string GetParameters(System.Reflection.MethodBase methodBase)
        {
            StringBuilder builder = new(1);
            foreach (var item in methodBase.GetParameters())
            {
                builder.Append(item.ParameterType.Name);
            }
            return builder.ToString();
        }

        private string GetRelativePath(string path)
        {
            if (path == null) return null;
            path = path[path.IndexOf("Assets")..];
            path = path.Replace('\\', '/');
            return path;
        }

        private string TrackBriefness(StackFrame frame)
        {
            return string.Format("{0}:{1}({2}) (at {3}:{4,3})",
                frame.GetMethod().DeclaringType.Name,
                frame.GetMethod().Name,
                GetParameters(frame.GetMethod()),
                GetRelativePath(frame.GetFileName()),
                frame.GetFileLineNumber());
        }

        private string TrackFormatting(StackTrace stackTrace)
        {
            StringBuilder builder = new(120);
            foreach (var item in stackTrace.GetFrames())
            {
                builder.Append(TrackBriefness(item)).Append("\r\n");
            }
            return builder.ToString();
        }

        public string GetFormatTrack()
        {
            return TrackFormatting(StackTrace);
        }

        public string GetBriefnessTrack()
        {
            return TrackBriefness(StackTrace.GetFrame(1));
        }

        public string GetExceptionTrack(Exception e)
        {
            StringBuilder builder = new(120);
            builder.Append("Error:" + e.Message).Append("\r\n");
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                builder.Append(e.StackTrace);
            }
            return builder.ToString();
        }

        public void LogRecord(LogLevel level, object message, string track = "")
        {
            LogData data = new()
            {
                logTime = DateTime.Now,
                logLevel = level,
                logMessage = message,
                logBasicData = TrackBriefness(StackTrace.GetFrame(1)),
                logTrack = track
            };

            foreach (var appender in _allAppenders)
            {
                appender.Value.Log(data);
            }
        }
    }
}
