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

namespace NscaHelloModule
{
    [Export(typeof(IModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Module : IModule, INsca
    {
        System.Threading.Timer _timer;
        int tInterval = 3600;
        FileSystemWatcher watcherConfig;

        public Module()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filter = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
            watcherConfig = new FileSystemWatcher(path, filter);
            watcherConfig.Changed += new FileSystemEventHandler(OnConfigChanghed);
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
                return "NscaHelloModule module send \"Hello\" message every N seconds";
            }
        }

        #endregion

        #region INsca

        public event NscaCheckEventHandler NscaCheck;
        private void RaiseNscaCheck(Nagios.Net.Client.Nsca.Level level, string msg)
        {
            if (NscaCheck != null)
                NscaCheck.Invoke(this, new NscaCheckEventArgs(this.ModuleName, level, msg));
        }

        public void Run()
        {
            LoadConfig();
            if (_timer == null)
                _timer = new System.Threading.Timer(new TimerCallback(SendHello), this, new TimeSpan(0, 0, 7), new TimeSpan(0, 0, tInterval));
            watcherConfig.EnableRaisingEvents = true;
        }

        private void LoadConfig()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);
            if (cfg.AppSettings.Settings.AllKeys.Contains("throwInterval") == true)
                int.TryParse(cfg.AppSettings.Settings["throwInterval"].Value, out tInterval);
        }

        public bool CanStop
        {
            get { return true; }
        }

        public void Stop()
        {
            watcherConfig.EnableRaisingEvents = false;
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion

        public void SendHello(Object stateInfo)
        {
            RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.OK, "Hello! " + DateTime.Now.ToString());
        }

    }
}
