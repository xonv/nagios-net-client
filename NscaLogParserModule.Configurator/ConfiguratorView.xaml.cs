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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Nagios.Net.Client.Common;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Collections.ObjectModel;

namespace NscaLogParserModule.Configurator
{
    /// <summary>
    /// Interaction logic for ConfiguratorView.xaml
    /// </summary>
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {

        System.Configuration.Configuration _cfg;
        ObservableCollection<LogFile> _fileLogs;

        public ObservableCollection<LogFile> Logs
        {
            get { return _fileLogs; }
            set { _fileLogs = value; }
        }

        public ConfiguratorView()
        {
            _fileLogs = new ObservableCollection<LogFile>();

            InitializeComponent();

            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NscaLogParserModule.Configurator.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
            LoadConfig();
        }

        #region IConfigurator

        public string Name
        {
            get { return "Nsca LogParser Module"; }
        }
        public string Description
        {
            get { return "NscaLogParserModule module watch the text log changes"; }
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
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.FileLogs) == true)
                {
                    _fileLogs.Clear();
                    LogFiles cc = LogFiles.Load(_cfg.AppSettings.Settings[ConfigConstants.FileLogs].Value);
                    if (cc != null)
                        cc.Files.ForEach(x => _fileLogs.Add(x));
                }

                this.parserLogsGrid.ItemsSource = this.Logs;
                this.parserLogsGrid.SelectedIndex = 0;
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
            LogFiles pc = new LogFiles();
            pc.Files = Logs.ToList();
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.FileLogs) == true)
                _cfg.AppSettings.Settings[ConfigConstants.FileLogs].Value = pc.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.FileLogs, pc.GetSerialized());

            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        #region Interface event handlers

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TextLogEditor w = new TextLogEditor();
            TextLogEditorModel m = new TextLogEditorModel();
            w.ViewModel = m;

            if (w.ShowDialog() == true)
            {
                LogFile nel = w.ViewModel.GetValues();
                this.Logs.Add(nel);
                this.parserLogsGrid.SelectedItem = nel;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.parserLogsGrid.SelectedItem == null)
                return;

            TextLogEditor w = new TextLogEditor();
            TextLogEditorModel m = new TextLogEditorModel();
            w.ViewModel = m;

            LogFile c = (LogFile)this.parserLogsGrid.SelectedItem;
            w.ViewModel.SetValues(c);

            if (w.ShowDialog() == true)
            {
                LogFile nc = w.ViewModel.GetValues();
                this.Logs.Remove(c);
                this.Logs.Add(nc);
                this.parserLogsGrid.SelectedItem = nc;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.parserLogsGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you want to delete selected template?", "Delete template", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                LogFile c = (LogFile)this.parserLogsGrid.SelectedItem;
                this.Logs.Remove(c);
                this.parserLogsGrid.SelectedItem = this.Logs.FirstOrDefault();
            }
        }

        #endregion Interface event handlers
    }
}
