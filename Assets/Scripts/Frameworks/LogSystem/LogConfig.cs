using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLXB.Framework.LogSystem
{
    public class LogConfig : Singleton<LogConfig>
    {
        private bool isInit = false;
        private LogConfig() { }
        public void Init()
        {
            if (isInit)
                return;
            isInit = true;

            Log.EnableLog(true);
            Log.LogLevel(LogLevel.ALL);
            Log.LoadAppenders(AppenderType.Console);
            Log.RegisterLogMessage();
            MonoMgr.Instance.AddDestroyListener(Log.UnRegisterLogMessage);
        }
    }
}
