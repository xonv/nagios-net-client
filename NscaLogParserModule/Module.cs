// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
