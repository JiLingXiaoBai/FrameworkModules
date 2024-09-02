using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using XBToolKit;
using Object = UnityEngine.Object;

public class GameLogger
{
    // 普通调试日志开关
    public static bool DebugLogEnable = true;

    // 警告日志开关
    public static bool WarningLogEnable = true;

    // 错误日志开关
    public static bool ErrorLogEnable = true;

    // 将日志写入文件
    public static bool LogToFile = true;

    public static int LogFileCount = 3;

    private static readonly string LogFileDir = Path.Combine(Application.persistentDataPath, "GameLog");

    private static GameLogger _gameLogger;

    private StreamWriter _streamWriter;
    private ManualResetEvent _manualResetEvent;
    private ConcurrentQueue<LogData> _concurrentQueue;
    private bool _threadRunning;

    private class LogData : IReference
    {
        public string LogMsg;
        public string TraceMsg;
        public LogType Level;

        public void Clear()
        {
        }
    }

    private GameLogger()
    {
    }

    public static void Open()
    {
        var sb = new StringBuilder("GameLogger Init: \r\n");
        sb.AppendLine($"DebugLogEnable: {DebugLogEnable}");
        sb.AppendLine($"WarningLogEnable: {WarningLogEnable}");
        sb.AppendLine($"ErrorLogEnable: {ErrorLogEnable}");
        sb.AppendLine($"LogToFile: {LogToFile}");
        sb.AppendLine($"LogFileCount: {LogFileCount}");
        UnityEngine.Debug.Log(sb.ToString());
        if (!LogToFile || LogFileCount <= 0) return;
        _gameLogger ??= new GameLogger();
        _gameLogger.Awake();
    }

    public static void Close()
    {
        if (!LogToFile || LogFileCount <= 0) return;
        if (_gameLogger == null) return;
        _gameLogger.Dispose();
        _gameLogger = null;
    }

    private static void CheckClearLog()
    {
        if (!Directory.Exists(LogFileDir))
        {
            Directory.CreateDirectory(LogFileDir);
        }
        else
        {
            var directionInfo = new DirectoryInfo(LogFileDir);
            var files = directionInfo.GetFiles("*");
            if (files.Length < LogFileCount) return;
            Array.Sort(files, (infoA, infoB) =>
            {
                if (infoA.CreationTime > infoB.CreationTime)
                {
                    return -1;
                }
                else if (infoA.CreationTime < infoB.CreationTime)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            for (var i = files.Length - 1; i >= LogFileCount - 1; i--)
            {
                files[i].Delete();
            }
        }
    }


    private void Awake()
    {
        CheckClearLog();
        var logFileName = $"GameLog-{DateTime.Now:yyyyMMddHHmmss}.txt";
        var logFilePath = Path.Combine(LogFileDir, logFileName);
        _streamWriter = new StreamWriter(logFilePath);
        _streamWriter.Write(GetSystemInfo());
        _manualResetEvent = new ManualResetEvent(false);
        _concurrentQueue = new ConcurrentQueue<LogData>();
        _threadRunning = true;

        Application.logMessageReceivedThreaded += OnLogMessageReceivedThread;

        var fileThread = new Thread(FileLogThread);
        fileThread.Start();
    }

    private void Dispose()
    {
        Application.logMessageReceivedThreaded -= OnLogMessageReceivedThread;

        _threadRunning = false;
        _manualResetEvent.Set();
        _streamWriter.Close();
        _streamWriter = null;
    }

    private void OnLogMessageReceivedThread(string logString, string stackTrace, LogType logType)
    {
        var logData = ReferencePool.Acquire<LogData>();
        logData.LogMsg = logString;
        logData.TraceMsg = stackTrace;
        logData.Level = logType;
        _concurrentQueue.Enqueue(logData);
        _manualResetEvent.Set();
    }


    private void FileLogThread()
    {
        if (_streamWriter == null) return;

        while (_threadRunning)
        {
            _manualResetEvent.WaitOne();
            if (_streamWriter == null) break;
            while (_concurrentQueue.TryDequeue(out var msg))
            {
                var res = msg.Level switch
                {
                    LogType.Error => $"[Error] {GetLogTime()} {msg.LogMsg}\r\n{GetStackTraceStr(msg.TraceMsg)}",
                    LogType.Assert => $"[Assert] {GetLogTime()} {msg.LogMsg}\r\n{GetStackTraceStr(msg.TraceMsg)}",
                    LogType.Warning => $"[Warning] {GetLogTime()} {msg.LogMsg}\r\n",
                    LogType.Log => $"[Debug] {GetLogTime()} {msg.LogMsg}\r\n",
                    LogType.Exception => $"[Exception] {GetLogTime()} {msg.LogMsg}\r\n{GetStackTraceStr(msg.TraceMsg)}",
                    _ => throw new ArgumentOutOfRangeException(nameof(msg.Level), msg.Level, null)
                };
                ReferencePool.Release(ref msg);
                _streamWriter.Write(res);
                _streamWriter.Write("\r\n");
            }
            _streamWriter.Flush();
            _manualResetEvent.Reset();
            Thread.Sleep(1);
        }
    }


    public static void Debug(object message, Object context = null)
    {
        if (!DebugLogEnable) return;
        UnityEngine.Debug.Log(message, context);
    }

    public static void Warning(object message, Object context = null)
    {
        if (!WarningLogEnable) return;
        UnityEngine.Debug.LogWarning(message, context);
    }

    public static void Error(object message, Object context = null)
    {
        if (!ErrorLogEnable) return;
        UnityEngine.Debug.LogError(message, context);
    }

    private static string GetLogTime()
    {
        return $"{DateTime.Now:HH:mm:ss.fff}";
    }

    private static string GetStackTraceStr(string traceMsg)
    {
        var count = 2;
        if (!traceMsg.Contains("GameLogger:"))
            count--;
#if !UNITY_EDITOR
        count--;
#endif
        for (var i = 0; i < count; i++)
        {
            traceMsg = traceMsg.Remove(0, traceMsg.IndexOf('\n') + 1);
        }
        return traceMsg;
    }

    private static string GetSystemInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "*********************************************************************************************************end");
        sb.AppendLine($"By {SystemInfo.deviceName}");
        var now = DateTime.Now;
        sb.AppendLine(string.Concat(new object[]
        {
            now.Year.ToString(), "年", now.Month.ToString(), "月", now.Day, "日  ", now.Hour.ToString(), ":",
            now.Minute.ToString(), ":", now.Second.ToString()
        }));
        sb.AppendLine();
        sb.AppendLine("操作系统:  " + SystemInfo.operatingSystem);
        sb.AppendLine("系统内存大小:  " + SystemInfo.systemMemorySize);
        sb.AppendLine("设备模型:  " + SystemInfo.deviceModel);
        sb.AppendLine("设备唯一标识符:  " + SystemInfo.deviceUniqueIdentifier);
        sb.AppendLine("处理器数量:  " + SystemInfo.processorCount);
        sb.AppendLine("处理器类型:  " + SystemInfo.processorType);
        sb.AppendLine("显卡标识符:  " + SystemInfo.graphicsDeviceID);
        sb.AppendLine("显卡名称:  " + SystemInfo.graphicsDeviceName);
        sb.AppendLine("显卡标识符:  " + SystemInfo.graphicsDeviceVendorID);
        sb.AppendLine("显卡厂商:  " + SystemInfo.graphicsDeviceVendor);
        sb.AppendLine("显卡版本:  " + SystemInfo.graphicsDeviceVersion);
        sb.AppendLine("显存大小:  " + SystemInfo.graphicsMemorySize);
        sb.AppendLine("显卡着色器级别:  " + SystemInfo.graphicsShaderLevel);
        sb.AppendLine("是否支持内置阴影:  " + SystemInfo.supportsShadows);
        sb.AppendLine(
            "*********************************************************************************************************end");
        sb.AppendLine("LogInfo:");
        sb.AppendLine();
        return sb.ToString();
    }


#if UNITY_EDITOR
    [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        var stackTrace = GetStackTrace();
        if (string.IsNullOrEmpty(stackTrace) || !stackTrace.Contains("GameLogger:")) return false;
        // 使用正则表达式匹配at的哪个脚本的哪一行
        var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        while (matches.Success)
        {
            var pathLine = matches.Groups[1].Value;

            if (!pathLine.Contains("GameLogger.cs"))
            {
                var splitIndex = pathLine.LastIndexOf(":", StringComparison.Ordinal);
                // 脚本路径
                var path = pathLine[..splitIndex];
                // 行号
                line = Convert.ToInt32(pathLine[(splitIndex + 1)..]);
                var fullPath =
                    Application.dataPath[..Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal)];
                fullPath += path;
                // 跳转到目标代码的特定行
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                break;
            }
            matches = matches.NextMatch();
        }
        return true;
    }

    /// <summary>
    /// 获取当前日志窗口选中的日志的堆栈信息
    /// </summary>
    /// <returns></returns>
    private static string GetStackTrace()
    {
        // 通过反射获取ConsoleWindow类
        var consoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        // 获取窗口实例
        var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.NonPublic);
        if (fieldInfo == null) return null;
        var consoleInstance = fieldInfo.GetValue(null);
        if (consoleInstance == null) return null;
        if (!ReferenceEquals(UnityEditor.EditorWindow.focusedWindow, consoleInstance)) return null;
        // 获取m_ActiveText成员
        fieldInfo = consoleWindowType.GetField("m_ActiveText",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic);
        // 获取m_ActiveText的值
        if (fieldInfo == null) return null;
        var activeText = fieldInfo.GetValue(consoleInstance).ToString();
        return activeText;
    }
#endif
}