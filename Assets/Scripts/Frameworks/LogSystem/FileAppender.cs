using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
namespace JLXB.Framework.LogSystem
{
    public class FileAppender : Singleton<FileAppender>, ILogAppender
    {
        private const string TIME_FORMATER = "yyyy.MM.dd HH:mm:ss,fff";
        // 10M
        private const int MAX_FILE_SIZE = 10 * 1024 * 1024;

        private StreamWriter _streamWriter;

        private FileStream _fileStream;

        private string _logFilePath;
        private string _logRootPath;

        private List<LogData> _writeList;

        private List<LogData> _waitList;

        private object _lockObj;

        private bool _stopFlag;

        private int _fileCount;

        private FileAppender()
        {
            Init();
            Run();
            MonoMgr.Instance.AddDestroyListener(() =>
            {
                _stopFlag = true;
                _fileStream?.Close();
            });
        }

        private void Init()
        {
#if UNITY_EDITOR
            //使用项目和Assets目录同级的Logs文件夹
            _logRootPath = Application.dataPath.Remove(Application.dataPath.Length - 7) + "/Logs";
#elif UNITY_STANDALONE_WIN
            _logRootPath = Application.dataPath + "/Logs";
#elif UNITY_STANDALONE_OSX
            _logRootPath = Application.dataPath + "/Logs";
#else
            _logRootPath = Application.persistentDataPath + "/Logs";
#endif
            _fileCount = 0;

            _logFilePath = Path.Combine(_logRootPath, string.Format("{0}_{1}.log", DateTime.Now.ToString("yyyyMM"), _fileCount));

            if (File.Exists(_logFilePath))
            {
                _fileStream = new FileStream(_logFilePath, FileMode.Append);
            }
            else
            {
                if (!Directory.Exists(_logRootPath))
                    Directory.CreateDirectory(_logRootPath);
                _fileStream = new FileStream(_logFilePath, FileMode.Create);
            }
            _streamWriter = new StreamWriter(_fileStream)
            {
                AutoFlush = true
            };

            _writeList = new List<LogData>();
            _waitList = new List<LogData>();
            _lockObj = new object();
            _stopFlag = false;
        }

        public void Log(LogData logData)
        {
            lock (_lockObj)
            {
                _waitList.Add(logData);
                Monitor.PulseAll(_lockObj);
            }
        }

        public void Run()
        {
            Loom.RunAsync(() =>
            {
                while (!_stopFlag)
                {
                    lock (_lockObj)
                    {
                        if (_waitList.Count == 0)
                        {
                            Monitor.Wait(_lockObj);
                        }
                        _writeList.AddRange(_waitList);
                        _waitList.Clear();
                    }
                    for (int i = 0; i < _writeList.Count; i++)
                    {
                        LogData logData = _writeList[i];
                        Loom.QueueOnMainThread((object sd) =>
                        {
                            _streamWriter.WriteLine(string.Format("[{0}] {1,-5}:{2}\r\n{3}", logData.logTime.ToString(TIME_FORMATER), logData.logLevel, logData.logMessage, logData.logBasicData));

                            if (logData.logTrack != null)
                            {
                                _streamWriter.WriteLine(logData.logTrack + "\n");
                            }
                        }, null);

                        HandleTriggerEvent();
                    }
                    _writeList.Clear();
                }
            });
        }

        private void HandleTriggerEvent()
        {
            if (_fileStream.Length >= MAX_FILE_SIZE)
            {
                _fileCount += 1;
                _logFilePath = Path.Combine(_logRootPath, string.Format("{0}_{1}.log", DateTime.Now.ToString("yyyyMMdd"), _fileCount));

                _fileStream = new FileStream(_logFilePath, FileMode.Create);
                _streamWriter = new StreamWriter(_fileStream)
                {
                    AutoFlush = true
                };
            }
        }
    }
}
