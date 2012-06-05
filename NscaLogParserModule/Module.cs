using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Nagios.Net.Client.Common;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace NscaLogParserModule
{
    [Export(typeof(IModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Module : IModule, INsca
    {
        FileSystemWatcher watcherConfig;
        List<FileWatcher> _watchers;

        public Module()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filter = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
            watcherConfig = new FileSystemWatcher(path, filter);
            watcherConfig.Changed += new FileSystemEventHandler(OnConfigChanghed);
            _watchers = new List<FileWatcher>();
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
                return "NscaLogParserModule module watch text log file on changes and send message";
            }
        }

        #endregion

        #region INsca

        public event NscaCheckEventHandler NscaCheck;
        private void RaiseNscaCheck(string service, Nagios.Net.Client.Nsca.Level level, string msg)
        {
            if (NscaCheck != null)
                NscaCheck.Invoke(this, new NscaCheckEventArgs(string.IsNullOrWhiteSpace(service) ? this.ModuleName : service, level, msg));
        }

        public void Run()
        {
            LoadConfig();
            watcherConfig.EnableRaisingEvents = true;
        }

        private void LoadConfig()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);
            if (cfg.AppSettings.Settings.AllKeys.Contains(NscaLogParserModule.Configurator.ConfigConstants.FileLogs) == true)
                SetWatchers(cfg.AppSettings.Settings[NscaLogParserModule.Configurator.ConfigConstants.FileLogs].Value);
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

        private void SetWatchers(string settings)
        {
            LogFiles lf = LogFiles.Load(settings);
            if (lf == null)
                return;
            _watchers.ForEach(x => { x.LogChanged -= new TextLogHandler(fw_LogChanged); x.Dispose(); });
            _watchers.Clear();
            foreach (LogFile f in lf.Files)
            {
                FileWatcher fw = new FileWatcher();
                fw.Config(f.Folder, f.FileTemplate, f.MessageTemplate, f.NagiosServiceName, f.MessageLevel);
                fw.LogChanged += new TextLogHandler(fw_LogChanged);
                fw.Start();
                _watchers.Add(fw);
            }
        }

        void fw_LogChanged(object sender, EventTextLogArgs e)
        {
            RaiseNscaCheck(e.ServiceName, e.Level, e.Message);
        }
    }
}
