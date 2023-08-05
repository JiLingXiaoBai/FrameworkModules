namespace JLXB.Framework.LogSystem
{
    public class LogConfig : Singleton<LogConfig>
    {
        private LogConfig() { }
        public void Init()
        {
            Log.EnableLog(true);
            Log.SetLogLevel(LogLevel.ALL);
            Log.LoadAppenders(AppenderType.File);
            Log.RegisterLogMessage();
            MonoMgr.Instance.AddDestroyListener(Log.UnRegisterLogMessage);
        }
    }
}
