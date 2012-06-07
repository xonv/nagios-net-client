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
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Nagios.Net.Client.Common;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Text;

namespace ClientConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _hasChanges; // for future use
        Configuration _appConfig;
        ObservableCollection<Nagios.Net.Client.Nrpe.FilteredHost> _nrpeHosts;

        private CompositionContainer _container;
        IEnumerable<Lazy<IConfigurator>> _lazyModules;
        List<IConfigurator> _modules;

        public MainWindow()
        {
            _modules = new List<IConfigurator>();
            InitializeComponent();
            _hasChanges = false;
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            ComposeModules();
            GeneralConfig();
            InitModules();
        }

        private void InitModules()
        {
            if (_lazyModules.Count() > 0)
            {
                foreach (Lazy<IConfigurator> m in _lazyModules)
                {
                    try
                    {
                        var module = m.Value;
                        if (module != null)
                        {
                            _modules.Add(module);
                            TabItem newTab = new TabItem();
                            newTab.Header = module.Name;
                            newTab.Content = module.ConfigForm;

                            this.mainTabs.Items.Add(newTab);

                            if (module is IConfig)
                            {
                                ((IConfig)module).LoadConfig();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error initialize of the Module ConfigForm", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void GeneralConfig()
        {
            try
            {
                string configFile = Path.Combine(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName, "nagiosnetclient.exe");
                _appConfig = ConfigurationManager.OpenExeConfiguration(configFile);

                SetGeneral();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error config loading", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine("Load config file: " + ex.Message);
            }
        }

        private void ComposeModules()
        {
            try
            {
                string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (_container == null)
                {
                    var catalog = new AggregateCatalog();
                    catalog.Catalogs.Add(new AssemblyCatalog(typeof(MainWindow).Assembly));
                    string modulesFolder = Path.Combine(dir, "Modules");
                    catalog.Catalogs.Add(new DirectoryCatalog(modulesFolder));

                    _container = new CompositionContainer(catalog);
                }
                // load and initialize modules
                try
                {
                    this._container.ComposeParts(this);

                    this._lazyModules = this._container.GetExports<IConfigurator>();
                }
                catch (CompositionException compositionException)
                {
                    MessageBox.Show(compositionException.ToString(), "Composition Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Modules Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetGeneral()
        {
            Nagios.Net.Client.Nsca.NscaSettings nsca = _appConfig.GetSection("nscaSettings") as Nagios.Net.Client.Nsca.NscaSettings;
            if (nsca != null)
            {
                this.nscaAddress.Text = nsca.NscaAddress;
                this.nscaPort.Text = nsca.Port.ToString();
                this.nscaPassword.Password = nsca.Password;
                this.nscaEcnryption.SelectedItem = nsca.EncryptionType.ToString();
                this.nscaHostName.Text = nsca.NscaHostName;
            }
            else
                MessageBox.Show("NSCA settings not founded in config file!", "Error config loading", MessageBoxButton.OK, MessageBoxImage.Error);

            Nagios.Net.Client.Nrpe.NrpeSettings nrpe = _appConfig.GetSection("nrpeSettings") as Nagios.Net.Client.Nrpe.NrpeSettings;
            if (nrpe != null)
            {
                this.nrpeAddresses.Text = nrpe.IP;
                this.nrpePort.Text = nrpe.Port.ToString();
                this.nrpeUseSsl.IsChecked = nrpe.SSL;
                _nrpeHosts = new ObservableCollection<Nagios.Net.Client.Nrpe.FilteredHost>();
                foreach (var h in nrpe.Hosts)
                    _nrpeHosts.Add((Nagios.Net.Client.Nrpe.FilteredHost)h);
                if (_nrpeHosts.Count == 0)
                    _nrpeHosts.Add(new Nagios.Net.Client.Nrpe.FilteredHost("Any"));

                this.nrpeHosts.ItemsSource = _nrpeHosts;
                this.nrpeHosts.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            }
            else
                MessageBox.Show("NRPE settings not founded in config file!", "Error config loading", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool SaveConfig()
        {
            bool rslt = false;
            Nagios.Net.Client.Nsca.NscaSettings nsca = _appConfig.GetSection("nscaSettings") as Nagios.Net.Client.Nsca.NscaSettings;
            if (nsca != null)
            {
                UpdateNsca(nsca);
            }
            else
            {
                MessageBox.Show("NSCA section of config has critical error, please restore config to original value", "Error config loading", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Nagios.Net.Client.Nrpe.NrpeSettings nrpe = _appConfig.GetSection("nrpeSettings") as Nagios.Net.Client.Nrpe.NrpeSettings;
            if (nrpe != null)
            {
                UpdateNrpe(nrpe);
            }
            else
            {
                MessageBox.Show("NRPE section of config has critical error, please restore config to original value", "Error config loading", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                _appConfig.Save(ConfigurationSaveMode.Full);
                this.statusMsg.Foreground = SystemColors.ControlTextBrush;
                this.statusMsg.Text = "Data saved at " + DateTime.Now.ToString("HH:mm:ss");
                rslt = true;
            }
            catch (Exception ex)
            {
                this.statusMsg.Foreground = new SolidColorBrush(Colors.Red);
                this.statusMsg.Text = "Erorr: " + ex.Message;
            }
            return rslt;
        }

        private void UpdateNsca(Nagios.Net.Client.Nsca.NscaSettings nsca)
        {
            nsca.NscaAddress = this.nscaAddress.Text;
            int port = 5667;
            if (int.TryParse(this.nscaPort.Text, out port) == false)
            {
                MessageBox.Show("Can't parse Port value, new value will be reset to default value (5667)", "Parse error", MessageBoxButton.OK, MessageBoxImage.Error);
                port = 5667;
            }
            nsca.Port = port;
            nsca.Password = this.nscaPassword.Password;
            nsca.EncryptionType = (Nagios.Net.Client.Nsca.NscaEncryptionType)Enum.Parse(typeof(Nagios.Net.Client.Nsca.NscaEncryptionType), (string)this.nscaEcnryption.SelectedItem);
            nsca.NscaHostName = this.nscaHostName.Text;
            nsca.SectionInformation.ForceSave = true;
        }

        private void UpdateNrpe(Nagios.Net.Client.Nrpe.NrpeSettings nrpe)
        {
            nrpe.IP = this.nrpeAddresses.Text;
            int port = 5666;
            if (int.TryParse(this.nrpePort.Text, out port) == false)
            {
                MessageBox.Show("Can't parse Port value, new value will be reset to default value (5666)", "Parse error", MessageBoxButton.OK, MessageBoxImage.Error);
                port = 5666;
            }
            nrpe.Port = port;
            nrpe.SSL = this.nrpeUseSsl.IsChecked == true ? true : false;
            nrpe.Hosts.Clear();

            System.Net.IPAddress nrpeIp = null;
            foreach (var h in _nrpeHosts.Where(x=> System.Net.IPAddress.TryParse(x.Host, out nrpeIp) == true))
                nrpe.Hosts.Add(h);
            nrpe.SectionInformation.ForceSave = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_hasChanges == true)
            {
                MessageBoxResult r = MessageBox.Show("You have unsaved configuration data. Do you want to save data before exit configurator?", "Save before exit", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    // save changes
                    SaveConfig();
                }
                else if (r == MessageBoxResult.Cancel)
                    return;
            }
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }

        private bool SaveAll()
        {
            bool rslt = SaveConfig();
            try
            {
                _modules.ForEach(x => x.SaveChanges());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Configs", MessageBoxButton.OK, MessageBoxImage.Error);
                rslt = false;
            }
            return rslt;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            GeneralConfig();
            _modules.ForEach(x => ((IConfig)x).LoadConfig());
        }

        private void AddHost_Click(object sender, RoutedEventArgs e)
        {
            Nagios.Net.Client.Nrpe.FilteredHost h = new Nagios.Net.Client.Nrpe.FilteredHost("New Host");
            _nrpeHosts.Add(h);
            this.nrpeHosts.SelectedItem = h;
        }

        private void RemoveHost_Click(object sender, RoutedEventArgs e)
        {
            if (this.nrpeHosts.SelectedItem == null)
                return;
            _nrpeHosts.Remove((Nagios.Net.Client.Nrpe.FilteredHost)this.nrpeHosts.SelectedItem);
            this.nrpeHosts.SelectedItem = _nrpeHosts.FirstOrDefault();
        }

        private const string fRuleNscaName = "Nagios Net NSCA";
        private const string fRuleNrpeName = "Nagios Net NRPE";
        private void FirewallButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveAll() == false)
                return;

            try
            {
                var fw = (NetFwTypeLib.INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr"));
                if (fw == null || fw.LocalPolicy.CurrentProfile.FirewallEnabled == false)
                {
                    MessageBox.Show("Can't set firewall rules. Windows Firewall diabled. Please enable firewall.", "Firewall", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // check firewall rules

                NetFwTypeLib.INetFwPolicy2 firewallPolicy = (NetFwTypeLib.INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                if (firewallPolicy != null)
                {
                    firewallPolicy.Rules.Remove(fRuleNscaName);
                    firewallPolicy.Rules.Remove(fRuleNrpeName);

                    Nagios.Net.Client.Nsca.NscaSettings nsca = _appConfig.GetSection("nscaSettings") as Nagios.Net.Client.Nsca.NscaSettings;
                    if (nsca != null)
                    {
                        NetFwTypeLib.INetFwRule nscaRule = (NetFwTypeLib.INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                        if (nscaRule != null)
                        {
                            nscaRule.Protocol = (int)NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                            nscaRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                            nscaRule.Description = "Used to allow Nagios Net client send NSCA messages to Nagios server";
                            nscaRule.Direction = NetFwTypeLib.NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                            nscaRule.Enabled = true;
                            nscaRule.InterfaceTypes = "All";
                            nscaRule.Name = fRuleNscaName;
                            nscaRule.RemoteAddresses = nsca.NscaAddress;
                            nscaRule.RemotePorts = nsca.Port.ToString();
                            firewallPolicy.Rules.Add(nscaRule);
                        }
                    }

                    Nagios.Net.Client.Nrpe.NrpeSettings nrpe = _appConfig.GetSection("nrpeSettings") as Nagios.Net.Client.Nrpe.NrpeSettings;
                    if (nrpe != null)
                    {
                        NetFwTypeLib.INetFwRule nrpeRule = (NetFwTypeLib.INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

                        if (nrpeRule != null)
                        {
                            nrpeRule.Protocol = (int)NetFwTypeLib.NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP; 
                            nrpeRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                            nrpeRule.Description = "Used to allow Nagios Net client receive NRPE commands and send replies to Nagios server";
                            nrpeRule.Direction = NetFwTypeLib.NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                            nrpeRule.Enabled = true;
                            nrpeRule.InterfaceTypes = "All";
                            nrpeRule.Name = fRuleNrpeName;

                            StringBuilder fhs = new StringBuilder();
                            foreach (Nagios.Net.Client.Nrpe.FilteredHost fh in nrpe.Hosts)
                            {
                                if (fhs.Length > 0)
                                    fhs.Append(",");
                                fhs.Append(fh.Host);
                            }
                            nrpeRule.RemoteAddresses = fhs.Length > 0 ? fhs.ToString() : "*";
                            nrpeRule.LocalAddresses = nrpe.IP.ToUpper().Contains("ANY") ? "*" : nrpe.IP;
                            nrpeRule.LocalPorts = nrpe.Port.ToString();
                            firewallPolicy.Rules.Add(nrpeRule);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Firewall Rules", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
