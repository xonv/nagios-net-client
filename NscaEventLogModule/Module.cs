using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nagios.Net.Client.Common;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace NscaEventLogModule
{

    [Export(typeof(IModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Module : IModule, INsca
    {
        FileSystemWatcher watcherConfig;
        EventLogs _logs;
        EventWatchers _eventWatchers;

        public Module()
        {
            _eventWatchers = new EventWatchers();
            _eventWatchers.SendLogEvent += new LogEventHandler(_eventWatchers_SendLogEvent);
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filter = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
            watcherConfig = new FileSystemWatcher(path, filter);
            watcherConfig.Changed += new FileSystemEventHandler(OnConfigChanghed);
        }

        void _eventWatchers_SendLogEvent(object sender, EventWatcherArgs e)
        {
            RaiseNscaCheck(e.Level, e.ServiceName, e.Message);
        }

        void OnConfigChanghed(object sender, FileSystemEventArgs e)
        {
            watcherConfig.EnableRaisingEvents = false;
            Stop();
            Run();
            watcherConfig.EnableRaisingEvents = true;
        }
        #region IModule

        public string ModuleName
        {
            get
            {
                return new AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Name;
            }
        }

        public string Version
        {
            get
            {
                return new AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Version.ToString();
            }
        }

        public string Description
        {
            get
            {
                return "NscaEventLogModule module watch the Event Log events and send message when events logged to the system logs";
            }
        }

        #endregion

        #region INsca

        public event NscaCheckEventHandler NscaCheck;
        private void RaiseNscaCheck(Nagios.Net.Client.Nsca.Level level, string serviceName, string msg)
        {
            if (NscaCheck != null)
                NscaCheck.Invoke(this, new NscaCheckEventArgs(string.IsNullOrWhiteSpace(serviceName) ? this.ModuleName : serviceName, level, msg));
        }

        public void Run()
        {
            LoadConfig();
            try
            {
                if (_logs != null)
                {
                    _eventWatchers.Init(_logs.Logs);
                    _eventWatchers.Start();
                }
            }
            catch (Exception ex)
            {
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
            watcherConfig.EnableRaisingEvents = true;
            }
        }

        private void LoadConfig()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);
            if (cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.EventLogs) == true)
                _logs = EventLogs.Load(cfg.AppSettings.Settings[ConfigConstants.EventLogs].Value);
        }

        public bool CanStop
        {
            get { return true; }
        }

        public void Stop()
        {
            watcherConfig.EnableRaisingEvents = false;
        }

        #endregion

    }
}
