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
using Nagios.Net.Client.Common.Schedule;
using NscaWinUpdateModule.Configurator;
using System.Text;


namespace NscaWinUpdateModule
{
    [Export(typeof(IModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Module : IModule, INsca
    {
        System.Timers.Timer _timer;
        TimeSpan _timerInterval;
        FileSystemWatcher watcherConfig;
        Dictionary<string, string> _configValues;
        RecurrencePattern _criticalSchedule;
        RecurrencePattern _warningSchedule;

        public Module()
        {
            _timerInterval = new TimeSpan(0, 1, 0);
            _configValues = new Dictionary<string, string>();
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
                return "NscaWinUpdateModule module check available Windows Updates and send reporting messages by schedule";
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
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = _timerInterval.TotalMilliseconds;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            }
            _timer.Enabled = true;
            watcherConfig.EnableRaisingEvents = true;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckUpdates(e.SignalTime);
        }

        private void LoadConfig()
        {
            _configValues.Clear();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(path);

            foreach (string key in cfg.AppSettings.Settings.AllKeys)
                _configValues.Add(key, cfg.AppSettings.Settings[key].Value);

            // schedule parameters
            try
            {
                if (_configValues.ContainsKey(ConfigConstants.CriticalSchedule))
                    _criticalSchedule = RecurrencePattern.Load(_configValues[ConfigConstants.CriticalSchedule]);
            }
            catch { }
            try
            {
                if (_configValues.ContainsKey(ConfigConstants.WarningSchedule))
                    _warningSchedule = RecurrencePattern.Load(_configValues[ConfigConstants.WarningSchedule]);
            }
            catch { }
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
                _timer.Enabled = false;
            }
        }

        #endregion

        public void CheckUpdates(DateTime time)
        {
            try
            {
                // check scheduler
                bool needToCheckCritical = false;
                bool needToCheckWarning = false;

                if (_criticalSchedule != null)
                {
                    DateTime? dt = _criticalSchedule.GetFirstOccurrence(time, _timerInterval);
                    if (dt.HasValue)
                        needToCheckCritical = true;
                }
                if (_warningSchedule != null)
                {
                    DateTime? dt = _warningSchedule.GetFirstOccurrence(time, _timerInterval);
                    if (dt.HasValue)
                        needToCheckWarning = true;
                }


                if (needToCheckCritical == false && needToCheckWarning == false)
                    return;

                // if schedule ocurred then check updates
                List<UpdateInfo> updates = WindowsUpdate.CheckUpdatesAvailable();


                // send message for critical updates
                if (needToCheckCritical)
                {
                    StringBuilder criticalMessage = new StringBuilder();

                    if (_configValues[ConfigConstants.CheckCriticalUpdates] == true.ToString() && _configValues[ConfigConstants.SendCriticalUpdatesAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Critical).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckSecurityUpdates] == true.ToString() && _configValues[ConfigConstants.SendSecurityUpdatesAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Security).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckDefinitionUpdates] == true.ToString() && _configValues[ConfigConstants.SendDefinitionUpdatesAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Definition).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckUpdates] == true.ToString() && _configValues[ConfigConstants.SendUpdatesAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Updates).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckFuturePacks] == true.ToString() && _configValues[ConfigConstants.SendFuturePacksAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Feature).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckOtherUpdates] == true.ToString() && _configValues[ConfigConstants.SendOtherUpdatesAsCritical] == true.ToString())
                        updates.Where(x => x.UpdateType != CheckingConstants.Critical &&
                        x.UpdateType != CheckingConstants.Security &&
                        x.UpdateType != CheckingConstants.Definition &&
                        x.UpdateType != CheckingConstants.Updates &&
                        x.UpdateType != CheckingConstants.Feature).OrderBy(x => x.Priority).ToList().ForEach(x => criticalMessage.AppendLine(x.GetMessage()));

                    if (criticalMessage.Length == 0)
                    {
                        RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.OK, _configValues.ContainsKey(ConfigConstants.CriticalServiceName) ? _configValues[ConfigConstants.CriticalServiceName] : null, "No critical updates found");
                    }
                    else
                    {
                        RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.Critical, _configValues.ContainsKey(ConfigConstants.CriticalServiceName) ? _configValues[ConfigConstants.CriticalServiceName] : null, criticalMessage.ToString());
                    }
                }

                // send message for warning updates
                if (needToCheckWarning)
                {
                    StringBuilder warningMessage = new StringBuilder();

                    if (_configValues[ConfigConstants.CheckCriticalUpdates] == true.ToString() && _configValues[ConfigConstants.SendCriticalUpdatesAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Critical).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckSecurityUpdates] == true.ToString() && _configValues[ConfigConstants.SendSecurityUpdatesAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Security).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckDefinitionUpdates] == true.ToString() && _configValues[ConfigConstants.SendDefinitionUpdatesAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Definition).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckUpdates] == true.ToString() && _configValues[ConfigConstants.SendUpdatesAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Updates).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckFuturePacks] == true.ToString() && _configValues[ConfigConstants.SendFuturePacksAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType == CheckingConstants.Feature).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));
                    if (_configValues[ConfigConstants.CheckOtherUpdates] == true.ToString() && _configValues[ConfigConstants.SendOtherUpdatesAsCritical] == false.ToString())
                        updates.Where(x => x.UpdateType != CheckingConstants.Critical &&
                        x.UpdateType != CheckingConstants.Security &&
                        x.UpdateType != CheckingConstants.Definition &&
                        x.UpdateType != CheckingConstants.Updates &&
                        x.UpdateType != CheckingConstants.Feature).OrderBy(x => x.Priority).ToList().ForEach(x => warningMessage.AppendLine(x.GetMessage()));

                    if (warningMessage.Length == 0)
                    {
                        RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.OK, _configValues.ContainsKey(ConfigConstants.WarningServiceName) ? _configValues[ConfigConstants.WarningServiceName] : null, "No warning updates found");
                    }
                    else
                    {
                        RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.Warning, _configValues.ContainsKey(ConfigConstants.WarningServiceName) ? _configValues[ConfigConstants.WarningServiceName] : null, warningMessage.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseNscaCheck(Nagios.Net.Client.Nsca.Level.Unknown, null, string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
        }
    }
}
