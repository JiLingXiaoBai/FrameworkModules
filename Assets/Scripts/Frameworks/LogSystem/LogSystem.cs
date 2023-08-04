using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        void Log(LogData data);
    }

    public enum AppenderType
    {
        File,          // 文件输出
        Console,       // 控制台输出
    }

    public class LogSystem : Singleton<LogSystem>
    {
        private readonly Dictionary<AppenderType, ILogAppender> _allAppenders = new();

        private readonly List<LogLevel> _ignoreLevel = new();

        public bool EnableLog { get; set; }

        public LogLevel LogLevel { get; set; }

        private bool isInitConfig = false;

        private LogSystem() { }

        private StackTrace StackTrace
        {
            get { return new StackTrace(3, true); }
        }

        public void LoadAppenders(AppenderType appenderType, ILogAppender appender)
        {
            if (_allAppenders.ContainsKey(appenderType)) return;

            _allAppenders.Add(appenderType, appender);
        }

        public void UnLoadAppenders(AppenderType appenderType)
        {
            if (!_allAppenders.ContainsKey(appenderType)) return;

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

        private bool IsOutputLog(LogLevel level)
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
            path = path[path.IndexOf("Assets")..];
            path = path.Replace('\\', '/');
            return path;
        }

        private string GetBriefnessTrack(StackFrame frame)
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
                builder.Append(string.Format(GetBriefnessTrack(item))).Append("\r\n");
            }
            return builder.ToString();
        }

        private string GetExceptionTrack(Exception e)
        {
            StringBuilder builder = new(120);
            builder.Append("Error:" + e.Message).Append("\r\n");
            if (!string.IsNullOrEmpty(e.StackTrace))
            {
                builder.Append(e.StackTrace);
            }
            return builder.ToString();
        }

        private void LogRecord(LogLevel level, object message, string track = "", bool receivedHandle = false)
        {
            if (!isInitConfig)
            {
                LogConfig.Instance.Init();
                isInitConfig = true;
            }
            if (!IsOutputLog(level)) return;
            LogData data = new()
            {
                logTime = DateTime.Now,
                logLevel = level,
                logMessage = message,
                logBasicData = GetBriefnessTrack(StackTrace.GetFrame(1)),
                logTrack = track
            };

            foreach (var appender in _allAppenders)
            {
                if (receivedHandle == (appender.Key == AppenderType.Console))
                    continue;
                appender.Value.Log(data);
            }
        }

        public void HandleLog(LogLevel level, object message, string track)
        {
            LogRecord(level, message, track, true);
        }

        public void Debug(object message)
        {
            LogRecord(LogLevel.DEBUG, message, TrackFormatting(StackTrace));
        }
        public void Debug(string format, params object[] args)
        {
            LogRecord(LogLevel.DEBUG, string.Format(format, args), TrackFormatting(StackTrace));
        }
        public void Debug(object message, string track)
        {
            LogRecord(LogLevel.DEBUG, message, track);
        }
        public void Debug(object message, Exception e)
        {
            LogRecord(LogLevel.DEBUG, message, GetExceptionTrack(e));
        }

        public void Info(object message)
        {
            LogRecord(LogLevel.INFO, message, TrackFormatting(StackTrace));
        }
        public void Info(string format, params object[] args)
        {
            LogRecord(LogLevel.INFO, string.Format(format, args), TrackFormatting(StackTrace));
        }
        public void Info(object message, string track)
        {
            LogRecord(LogLevel.INFO, message, track);
        }
        public void Info(object message, Exception e)
        {
            LogRecord(LogLevel.INFO, message, GetExceptionTrack(e));
        }

        public void Warn(object message)
        {
            LogRecord(LogLevel.WARN, message, TrackFormatting(StackTrace));
        }
        public void Warn(string format, params object[] args)
        {
            LogRecord(LogLevel.WARN, string.Format(format, args), TrackFormatting(StackTrace));
        }
        public void Warn(object message, string track)
        {
            LogRecord(LogLevel.WARN, message, track);
        }
        public void Warn(object message, Exception e)
        {
            LogRecord(LogLevel.WARN, message, GetExceptionTrack(e));
        }

        public void Error(object message)
        {
            LogRecord(LogLevel.ERROR, message, TrackFormatting(StackTrace));
        }
        public void Error(string format, params object[] args)
        {
            LogRecord(LogLevel.ERROR, string.Format(format, args), TrackFormatting(StackTrace));
        }
        public void Error(object message, string track)
        {
            LogRecord(LogLevel.ERROR, message, track);
        }
        public void Error(object message, Exception e)
        {
            LogRecord(LogLevel.ERROR, message, GetExceptionTrack(e));
        }

        public void Fatal(object message)
        {
            LogRecord(LogLevel.FATAL, message, TrackFormatting(StackTrace));
        }
        public void Fatal(string format, params object[] args)
        {
            LogRecord(LogLevel.FATAL, string.Format(format, args), TrackFormatting(StackTrace));
        }
        public void Fatal(object message, string track)
        {
            LogRecord(LogLevel.FATAL, message, track);
        }
        public void Fatal(object message, Exception e)
        {
            LogRecord(LogLevel.FATAL, message, GetExceptionTrack(e));
        }
    }




}
