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
using System.ComponentModel.Composition;
using Nagios.Net.Client.Common;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace NscaWinServicesModule.Configurator
{
    /// <summary>
    /// Interaction logic for ConfiguratorView.xaml
    /// </summary>
    ///
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {
        System.Configuration.Configuration _cfg;
        ObservableCollection<ServiceDescription> _services;

        public ConfiguratorView()
        {
            _services = new ObservableCollection<ServiceDescription>();
            InitializeComponent();

            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NscaWinServicesModule.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
            LoadConfig();
        }

        public ObservableCollection<ServiceDescription> Services
        {
            get { return _services; }
            set { _services = value; }
        }

        #region IConfigurator

        public string Name
        {
            get { return "Nsca WinServices Module"; }
        }
        public string Description
        {
            get { return "NscaWinServicesModule module check the availability and status of Windows Services"; }
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
            try
            {
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.Services) == true)
                {
                    _services.Clear();
                    WinServices cc = WinServices.Load(_cfg.AppSettings.Settings[ConfigConstants.Services].Value);
                    if (cc != null)
                        cc.Services.ForEach(x => _services.Add(x));
                }

                this.servicesGrid.ItemsSource = this.Services;
                this.servicesGrid.SelectedIndex = 0;

                this.nagiosServiceStartPause.Text = "60";
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.NagiosServiceStartPause) == true)
                {
                    this.nagiosServiceStartPause.Text = _cfg.AppSettings.Settings[ConfigConstants.NagiosServiceStartPause].Value;
                }
            }
            catch (Exception ex)
            {
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
        }

        #endregion

        #region Data Processing

        private void Save()
        {
            WinServices pc = new WinServices();
            pc.Services = Services.ToList();
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.Services) == true)
                _cfg.AppSettings.Settings[ConfigConstants.Services].Value = pc.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.Services, pc.GetSerialized());

            int duration;
            if (int.TryParse(this.nagiosServiceStartPause.Text, out duration) == true)
            {
                _cfg.AppSettings.Settings[ConfigConstants.NagiosServiceStartPause].Value = this.nagiosServiceStartPause.Text;
            }
            else
                _cfg.AppSettings.Settings[ConfigConstants.NagiosServiceStartPause].Value = "60";

            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceEditor w = new ServiceEditor();
            ServiceDescription c = new ServiceDescription();
            w.ViewModel.SetService(c);
            if (w.ShowDialog() == true)
            {
                ServiceDescription nc = w.ViewModel.GetService();
                this.Services.Add(nc);
                this.servicesGrid.SelectedItem = nc;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.servicesGrid.SelectedItem == null)
                return;

            ServiceEditor w = new ServiceEditor();
            ServiceDescription c = (ServiceDescription)this.servicesGrid.SelectedItem;
            w.ViewModel.SetService(c);
            if (w.ShowDialog() == true)
            {
                ServiceDescription nc = w.ViewModel.GetService();
                this.Services.Remove(c);
                this.Services.Add(nc);
                this.servicesGrid.SelectedItem = nc;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.servicesGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you want to delete selected service?", "Delete service", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                ServiceDescription c = (ServiceDescription)this.servicesGrid.SelectedItem;
                this.Services.Remove(c);
                this.servicesGrid.SelectedItem = this.Services.FirstOrDefault();
            }
        }
    }
}
