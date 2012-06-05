using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Nagios.Net.Client;

namespace NscaLogParserModule
{
    public delegate void TextLogHandler(object sender, EventTextLogArgs e);

    public class FileWatcher : IDisposable
    {
        FileSystemWatcher _watcher;
        string _serviceName;
        Nagios.Net.Client.Nsca.Level _level;
        string _eventFilter;
        List<LogFileWatch> _files;
        Regex _regExp;

        public FileWatcher()
        {
            _files = new List<LogFileWatch>();
            _watcher = new FileSystemWatcher();
            _watcher.Created += new FileSystemEventHandler(_watcher_Created);
            _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Deleted)
            {
                ParseChanges(e);
            }
            else
            {
                var f = _files.SingleOrDefault(x => x.FileName == e.FullPath);
                if (f != null)
                    _files.Remove(f);
            }
        }

        private void ParseChanges(FileSystemEventArgs e)
        {
            FileInfo fi = new FileInfo(e.FullPath);
            LogFileWatch fw = _files.SingleOrDefault(x => x.FileName == fi.FullName);
            if (fw == null)
            {
                _files.Add(new LogFileWatch(e.FullPath, 0));
                return;
            }

            try
            {
                using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long fLength = fs.Length;

                    if (fLength > 0)
                    {
                        if (fw.Offset == 0)
                            fw.Offset = fLength;
                        else
                        {
                            //read file up to the end
                            using (StreamReader r = new StreamReader(fs))
                            {
                                try
                                {
                                    r.BaseStream.Seek(fw.Offset, SeekOrigin.Begin);

                                    string s = string.Empty;

                                    while ((s = r.ReadLine()) != null)
                                    {
                                        if (string.IsNullOrEmpty(s) == false && _regExp != null)
                                        {
                                            if (_regExp.IsMatch(s))
                                            {
                                                RaiseLogChanged(string.Format("{0}: {1}", fi.Name, s));
                                            }
                                        }

                                    }
                                    fw.Offset = r.BaseStream.Position;
                                }
                                catch (Exception ee)
                                {
                                    Log.WriteLog(ee.Message + System.Environment.NewLine + ee.StackTrace, true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message + System.Environment.NewLine + ex.StackTrace, true);
            }

        }

        void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            ParseChanges(e);
        }

        public event TextLogHandler LogChanged;
        private void RaiseLogChanged(string msg)
        {
            if (LogChanged != null)
                LogChanged.Invoke(this, new EventTextLogArgs(_serviceName, _level, msg));
        }

        public void Config(string folder, string fileFilter, string eventFilter, string service, Nagios.Net.Client.Nsca.Level level)
        {
            // find files
            DirectoryInfo di = new DirectoryInfo(folder);
            if (di.Exists == false)
            {
                Log.WriteLog(string.Format("Nsca LogParser Module: folder {0} is not exist, set full folder name by Configurator", folder), true);
                return;
            }
            try
            {
                FileInfo[] files = di.GetFiles(fileFilter);
                foreach (FileInfo f in files)
                {
                    _files.Add(new LogFileWatch(f.FullName, f.Length));
                }
                // setup watcher
                _watcher.EnableRaisingEvents = false;
                _watcher.Path = folder;
                _watcher.Filter = fileFilter;
                _watcher.IncludeSubdirectories = false;
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

                _serviceName = service;
                _eventFilter = eventFilter;
                _level = level;

                if (string.IsNullOrWhiteSpace(eventFilter) == false)
                {
                    _regExp = new Regex(eventFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                else
                    _regExp = null;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message + System.Environment.NewLine + ex.StackTrace, true);
            }
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false; ;
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
        }
    }

    public class EventTextLogArgs : EventArgs
    {
        string serviceName;
        Nagios.Net.Client.Nsca.Level level;
        string message;

        public EventTextLogArgs(string serviceName, Nagios.Net.Client.Nsca.Level level, string message)
        {
            this.serviceName = serviceName;
            this.level = level;
            this.message = message;
        }

        public string ServiceName
        {
            get { return serviceName; }
        }
        public Nagios.Net.Client.Nsca.Level Level
        {
            get { return level; }
        }
        public string Message
        {
            get { return message; }
        }
    }

    public class LogFileWatch
    {
        public LogFileWatch() { }
        public LogFileWatch(string FileName, long Offset)
        {
            this.FileName = FileName;
            this.Offset = Offset;
        }
        public string FileName { get; set; }
        public long Offset { get; set; }
    }
}
