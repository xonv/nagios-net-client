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
