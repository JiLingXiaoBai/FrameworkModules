using System;
namespace JLXB.Framework.LogSystem
{
    public class ConsoleAppender : Singleton<ConsoleAppender>, ILogAppender
    {
        private ConsoleAppender() { }
        public void Log(LogData data)
        {
            string str = String.Format("{0}\n[{1,-5}] {2}\n", data.logMessage, data.logLevel, data.logBasicData);
            switch (data.logLevel)
            {
                case LogLevel.DEBUG:
                case LogLevel.INFO:
                    UnityEngine.Debug.Log(str);
                    break;
                case LogLevel.WARN:
                    UnityEngine.Debug.LogWarning(str);
                    break;
                case LogLevel.ERROR:
                    UnityEngine.Debug.LogError(str);
                    break;
                case LogLevel.FATAL:
                    UnityEngine.Debug.LogError(str);
                    break;
            }
        }
    }
}
