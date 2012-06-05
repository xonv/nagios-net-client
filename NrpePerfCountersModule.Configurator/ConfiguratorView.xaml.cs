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

namespace NrpePerfCountersModule.Configurator
{
    /// <summary>
    /// Interaction logic for ConfiguratorView.xaml
    /// </summary>
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {
        System.Configuration.Configuration _cfg;
        ObservableCollection<PerfCounter> _counters;

        public ConfiguratorView()
        {
            _counters = new ObservableCollection<PerfCounter>();

            InitializeComponent();

            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NrpePerfCountersModule.Configurator.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
            LoadConfig();
        }

        public ObservableCollection<PerfCounter> Counters
        {
            get { return _counters; }
            set { _counters = value; }
        }

        #region IConfigurator

        public string Name
        {
            get { return "Nrpe PerfCounters Module"; }
        }
        public string Description
        {
            get { return "NrpePerfCountersModule module check system performance counters and retrive values by Nagios server queries"; }
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
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.Counters) == true)
                {
                    _counters.Clear();
                    PerfCounters cc = PerfCounters.Load(_cfg.AppSettings.Settings[ConfigConstants.Counters].Value);
                    if (cc != null)
                        cc.Counters.ForEach(x => Counters.Add(x));
                }
                this.countersGrid.ItemsSource = this.Counters;
                this.countersGrid.SelectedIndex = 0;
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
            PerfCounters pc = new PerfCounters();
            pc.Counters = Counters.ToList();
            if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.Counters) == true)
                _cfg.AppSettings.Settings[ConfigConstants.Counters].Value = pc.GetSerialized();
            else
                _cfg.AppSettings.Settings.Add(ConfigConstants.Counters, pc.GetSerialized());


            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CounterEditor w = new CounterEditor();
            PerfCounter c = new PerfCounter { Duration = 5 };
            w.ViewModel.SetPerfCounter(c);
            if (w.ShowDialog() == true)
            {
                PerfCounter nc = w.ViewModel.GetPerfCounter();
                this.Counters.Add(nc);
                this.countersGrid.SelectedItem = nc;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.countersGrid.SelectedItem == null)
                return;

            CounterEditor w = new CounterEditor();
            PerfCounter c = (PerfCounter)this.countersGrid.SelectedItem;
            w.ViewModel.SetPerfCounter(c);
            if (w.ShowDialog() == true)
            {
                PerfCounter nc = w.ViewModel.GetPerfCounter();
                this.Counters.Remove(c);
                this.Counters.Add(nc);
                this.countersGrid.SelectedItem = nc;
            }
        }

        private void EnableButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.countersGrid.SelectedItem == null)
                return;
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.countersGrid.SelectedItem == null)
                return;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.countersGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you want to delete selected counter?", "Delete counter", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                PerfCounter c = (PerfCounter)this.countersGrid.SelectedItem;
                this.Counters.Remove(c);
                this.countersGrid.SelectedItem = this.Counters.FirstOrDefault();
            }
        }
    }
}
