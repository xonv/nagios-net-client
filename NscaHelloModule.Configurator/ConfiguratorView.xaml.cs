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
using System.ComponentModel.Composition;
using Nagios.Net.Client.Common;
using System.IO;
using System.Configuration;

namespace NscaHelloModule.Configurator
{
    [Export(typeof(IConfigurator))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConfiguratorView : UserControl, IConfigurator, IConfig
    {
        Configuration _cfg;
        private int _interval;

        public ConfiguratorView()
        {
            InitializeComponent();
            _interval = 3600;

            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NscaHelloModule.Configurator.ConfigConstants.ConfigFileName);
            _cfg = ConfigurationManager.OpenExeConfiguration(path);

            this.cfgDescription.Text = Description;
        }


        #region IConfigurator

        public string Name
        {
            get { return "Nsca Hello Module"; }
        }
        public string Description
        {
            get { return "NscaHelloModule module send \"Hello\" message every N seconds"; }
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
            if (_cfg.AppSettings.Settings.AllKeys.Contains("throwInterval") == true)
                int.TryParse(_cfg.AppSettings.Settings["throwInterval"].Value, out _interval);

            this.interval.Text = _interval.ToString();
        }

        #endregion

        #region Data Processing

        private void Save()
        {
            int.TryParse(this.interval.Text, out _interval);

            if (_cfg.AppSettings.Settings.AllKeys.Contains("throwInterval") == true)
                _cfg.AppSettings.Settings["throwInterval"].Value = _interval.ToString();
            else
                _cfg.AppSettings.Settings.Add("throwInterval", _interval.ToString());

            _cfg.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        #endregion
    }
}
