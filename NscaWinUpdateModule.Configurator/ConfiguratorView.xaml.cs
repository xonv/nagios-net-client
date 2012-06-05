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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Nagios.Net.Client.Common;
using Nagios.Net.Client.Common.Schedule;
using System.IO;
using System.Configuration;

namespace NscaWinUpdateModule.Configurator
{
    /// <summary>
    /// Interaction logic for ConfiguratorView.xaml
    /// </summary>
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {
        Configuration _cfg;
        RecurrencePattern _criticalSchedule = new RecurrencePattern();
        RecurrencePattern _warningSchedule = new RecurrencePattern();


        public ConfiguratorView()
        {
            InitializeComponent();

            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NscaWinUpdateModule.Configurator.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
        }

        #region IConfigurator

        public string Name
        {
            get { return "Nsca Windows Update Module"; }
        }
        public string Description
        {
            get { return "NscaWinUpdateModule module check available Windows Updates and send reporting messages by schedule"; }
        }
        public UserControl ConfigForm
        {
            get { return this; }
        }

        public void SaveChanges()
        {
            this.Save();
        }

        #endregion

        #region IConfig

        public void LoadConfig()
        {
            bool scr;

            sendCritical.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckCriticalUpdates) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckCriticalUpdates].Value, out scr))
                    sendCritical.IsChecked = scr;
            sendSecurity.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckSecurityUpdates) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckSecurityUpdates].Value, out scr))
                    sendSecurity.IsChecked = scr;
            sendDefinition.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckDefinitionUpdates) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckDefinitionUpdates].Value, out scr))
                    sendDefinition.IsChecked = scr;
            sendUpdate.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckUpdates) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckUpdates].Value, out scr))
                    sendUpdate.IsChecked = scr;
            sendFuture.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckFuturePacks) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckFuturePacks].Value, out scr))
                    sendFuture.IsChecked = scr;
            sendOther.IsChecked = false;
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckOtherUpdates) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.CheckOtherUpdates].Value, out scr))
                    sendOther.IsChecked = scr;

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendCriticalUpdatesAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendCriticalUpdatesAsCritical].Value, out scr))
                {
                    criticalUpdatesAsCritical.IsChecked = scr;
                    criticalUpdatesAsWarning.IsChecked = !scr;
                }
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendSecurityUpdatesAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendSecurityUpdatesAsCritical].Value, out scr))
                {
                    securityUpdatesAsCritical.IsChecked = scr;
                    securityUpdatesAsWarning.IsChecked = !scr;
                }
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendDefinitionUpdatesAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendDefinitionUpdatesAsCritical].Value, out scr))
                {
                    definitionUpdatesAsCritical.IsChecked = scr;
                    definitionUpdatesAsWarning.IsChecked = !scr;
                }
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendUpdatesAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendUpdatesAsCritical].Value, out scr))
                {
                    updatesAsCritical.IsChecked = scr;
                    updatesAsWarning.IsChecked = !scr;
                }
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendFuturePacksAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendFuturePacksAsCritical].Value, out scr))
                {
                    futurePackAsCritical.IsChecked = scr;
                    futurePackAsWarning.IsChecked = !scr;
                }
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendOtherUpdatesAsCritical) == true)
                if (bool.TryParse(_cfg.AppSettings.Settings[ConfigConstants.SendOtherUpdatesAsCritical].Value, out scr))
                {
                    otherUpdatesAsCritical.IsChecked = scr;
                    otherUpdatesAsWarning.IsChecked = !scr;
                }

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CriticalServiceName) == true)
                criticalServiceName.Text = _cfg.AppSettings.Settings[ConfigConstants.CriticalServiceName].Value;

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.WarningServiceName) == true)
                warningServiceName.Text = _cfg.AppSettings.Settings[ConfigConstants.WarningServiceName].Value;

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CriticalSchedule) == true)
            {
                try
                {
                    _criticalSchedule = RecurrencePattern.Load(_cfg.AppSettings.Settings[ConfigConstants.CriticalSchedule].Value);
                }
                catch { _criticalSchedule = new RecurrencePattern(); }
            }

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.WarningSchedule) == true)
            {
                try
                {
                    _warningSchedule = RecurrencePattern.Load(_cfg.AppSettings.Settings[ConfigConstants.WarningSchedule].Value);
                }
                catch { _warningSchedule = new RecurrencePattern(); }
            }

        }

        #endregion

        #region Data Processing

        private void Save()
        {

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckCriticalUpdates) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckCriticalUpdates].Value = sendCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckCriticalUpdates, sendCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckSecurityUpdates) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckSecurityUpdates].Value = sendSecurity.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckSecurityUpdates, sendSecurity.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckDefinitionUpdates) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckDefinitionUpdates].Value = sendDefinition.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckDefinitionUpdates, sendDefinition.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckUpdates) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckUpdates].Value = sendUpdate.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckUpdates, sendUpdate.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckFuturePacks) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckFuturePacks].Value = sendFuture.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckFuturePacks, sendFuture.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CheckOtherUpdates) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CheckOtherUpdates].Value = sendOther.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CheckOtherUpdates, sendOther.IsChecked.ToString());

            // ===============================================

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendCriticalUpdatesAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendCriticalUpdatesAsCritical].Value = criticalUpdatesAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendCriticalUpdatesAsCritical, criticalUpdatesAsCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendSecurityUpdatesAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendSecurityUpdatesAsCritical].Value = securityUpdatesAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendSecurityUpdatesAsCritical, securityUpdatesAsCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendDefinitionUpdatesAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendDefinitionUpdatesAsCritical].Value = definitionUpdatesAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendDefinitionUpdatesAsCritical, definitionUpdatesAsCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendUpdatesAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendUpdatesAsCritical].Value = updatesAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendUpdatesAsCritical, updatesAsCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendFuturePacksAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendFuturePacksAsCritical].Value = futurePackAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendFuturePacksAsCritical, futurePackAsCritical.IsChecked.ToString());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.SendOtherUpdatesAsCritical) == true)
                _cfg.AppSettings.Settings[ConfigConstants.SendOtherUpdatesAsCritical].Value = otherUpdatesAsCritical.IsChecked.ToString();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.SendOtherUpdatesAsCritical, otherUpdatesAsCritical.IsChecked.ToString());

            // ================================================

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CriticalServiceName) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CriticalServiceName].Value = criticalServiceName.Text;
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CriticalServiceName, criticalServiceName.Text);

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.WarningServiceName) == true)
                _cfg.AppSettings.Settings[ConfigConstants.WarningServiceName].Value = warningServiceName.Text;
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.WarningServiceName, warningServiceName.Text);

            // ===============================================

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.CriticalSchedule) == true)
                _cfg.AppSettings.Settings[ConfigConstants.CriticalSchedule].Value = _criticalSchedule.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.CriticalSchedule, _criticalSchedule.GetSerialized());

            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.WarningSchedule) == true)
                _cfg.AppSettings.Settings[ConfigConstants.WarningSchedule].Value = _warningSchedule.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.WarningSchedule, _warningSchedule.GetSerialized());

            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        #endregion

        private static Window InitScheduleWindow(RecurrencePattern rPattern)
        {
            Window w = new Window();
            w.SizeToContent = SizeToContent.WidthAndHeight;
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            NetClient.Common.Schedule.ScheduleView v = new NetClient.Common.Schedule.ScheduleView();
            v.Pattern = rPattern;
            w.Content = v;
            w.Title = "Schedule settings";
            w.ShowInTaskbar = false;
            w.WindowStyle = WindowStyle.SingleBorderWindow;
            w.ResizeMode = ResizeMode.NoResize;
            return w;
        }

        private void warningSched_Click(object sender, RoutedEventArgs e)
        {

            Window w = InitScheduleWindow(_warningSchedule);

            w.Closed += (sx, ex) =>
            {
                if (w.DialogResult == true)
                {
                    _warningSchedule = ((NetClient.Common.Schedule.ScheduleView)w.Content).Pattern;
                }
            };
            w.ShowDialog();
        }

        private void crtclSched_Click(object sender, RoutedEventArgs e)
        {

            Window w = InitScheduleWindow(_criticalSchedule);

            w.Closed += (sx, ex) =>
            {
                if (w.DialogResult == true)
                {
                    _criticalSchedule = ((NetClient.Common.Schedule.ScheduleView)w.Content).Pattern;
                }
            };
            w.ShowDialog();
        }
    }
}
