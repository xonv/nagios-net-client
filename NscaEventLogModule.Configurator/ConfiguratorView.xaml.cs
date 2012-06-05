using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Nagios.Net.Client.Common;

namespace NscaEventLogModule.Configurator
{
    /// <summary>
    /// Interaction logic for ConfiguratorView.xaml
    /// </summary>
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {
        System.Configuration.Configuration _cfg;
        ObservableCollection<EventLogDescription> _eventLogs;

        public ObservableCollection<EventLogDescription> Logs
        {
            get { return _eventLogs; }
            set { _eventLogs = value; }
        }

        public ConfiguratorView()
        {
            _eventLogs = new ObservableCollection<EventLogDescription>();
            InitializeComponent();
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                NscaEventLogModule.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
            LoadConfig();
        }

        public string Name
        {
            get { return "Nsca EventLog Module"; }
        }

        public string Description
        {
            get { return "NscaEventLogModule module check system eventlogs and send appropriate messages "; }
        }

        public UserControl ConfigForm
        {
            get { return this; }
        }

        public void SaveChanges()
        {
            this.Save();
        }

        #region Data processing

        private void Save()
        {

            EventLogs pc = new EventLogs();
            pc.Logs = Logs.ToList();
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.EventLogs) == true)
                _cfg.AppSettings.Settings[ConfigConstants.EventLogs].Value = pc.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.EventLogs, pc.GetSerialized());
            
            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        #endregion Data processing

        #region IConfig

        public void LoadConfig()
        {
            try
            {
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.EventLogs) == true)
                {
                    _eventLogs.Clear();
                    EventLogs cc = EventLogs.Load(_cfg.AppSettings.Settings[ConfigConstants.EventLogs].Value);
                    if (cc != null)
                        cc.Logs.ForEach(x => _eventLogs.Add(x));
                }
                this.eventLogsGrid.ItemsSource = this.Logs;
                this.eventLogsGrid.SelectedIndex = 0;
            }
            catch (System.Exception ex)
            {
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
        }

        #endregion IConfig

        #region Interface event handlers

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogEditor w = new EventLogEditor();
            EventLogEditorModel m = new EventLogEditorModel();
            w.ViewModel = m;

            if (w.ShowDialog() == true)
            {
                EventLogDescription nel = w.ViewModel.GetEventLog();
                this.Logs.Add(nel);
                this.eventLogsGrid.SelectedItem = nel;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.eventLogsGrid.SelectedItem == null)
                return;

            EventLogEditor w = new EventLogEditor();
            EventLogEditorModel m = new EventLogEditorModel();
            w.ViewModel = m;

            EventLogDescription c = (EventLogDescription)this.eventLogsGrid.SelectedItem;
            w.ViewModel.SetEventLog(c);

            if (w.ShowDialog() == true)
            {
                EventLogDescription nc = w.ViewModel.GetEventLog();
                this.Logs.Remove(c);
                this.Logs.Add(nc);
                this.eventLogsGrid.SelectedItem = nc;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.eventLogsGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you want to delete selected filter?", "Delete filter", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                EventLogDescription c = (EventLogDescription)this.eventLogsGrid.SelectedItem;
                this.Logs.Remove(c);
                this.eventLogsGrid.SelectedItem = this.Logs.FirstOrDefault();
            }
        }

        #endregion Interface event handlers
    }
}