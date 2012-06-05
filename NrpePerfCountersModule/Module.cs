using System;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Reflection;
using System.Threading;
using Nagios.Net.Client.Common;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using NrpePerfCountersModule.Configurator;
using Nagios.Net.Client;

namespace NrpePerfCountersModule
{
    [Export(typeof(IModule))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Module : IModule, INrpe, IDisposable
    {
        FileSystemWatcher watcherConfig;
        System.Timers.Timer _timer;
        List<PCounter> _counters;

        public Module()
        {
            _counters = new List<PCounter>();

            _timer = new System.Timers.Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filter = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll.config";
            watcherConfig = new FileSystemWatcher(path, filter);
            watcherConfig.Changed += new FileSystemEventHandler(OnConfigChanghed);

            Configurate();
            watcherConfig.EnableRaisingEvents = true;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (PCounter c in _counters)
            {
                c.Check();
            }
        }

        void OnConfigChanghed(object sender, FileSystemEventArgs e)
        {
            watcherConfig.EnableRaisingEvents = false;
            try
            {
                Configurate();
            }
            catch (Exception ex)
            {
                Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
                watcherConfig.EnableRaisingEvents = true;
            }
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
                return "NrpePerfCountersModule module check system performance counters and retrive values by Nagios server queries";
            }
        }

        #endregion

        #region INrpe

        public void RunCommand(string command, string[] parameters, out Nagios.Net.Client.Nrpe.MessageState level, out string message)
        {
            // module process commands without parameters
            if (GetRegisteredCommands().Contains(command) == false)
            {
                level = Nagios.Net.Client.Nrpe.MessageState.STATE_UNKNOWN;
                message = "Incorrect Command";
                return;
            }

            PCounter c = _counters.Where(x => x.Config.Command == command).FirstOrDefault();
            if (c == null)
            {
                level = Nagios.Net.Client.Nrpe.MessageState.STATE_UNKNOWN;
                message = "Incorrect Command";
                return;
            }
            float value = c.Value;
            if ((c.Config.CriticalMethod == (int)(ExpressionMethods.Equal) && c.Config.CriticalValue == value) ||
                (c.Config.CriticalMethod == (int)(ExpressionMethods.GreateThen) && c.Config.CriticalValue < value) ||
                (c.Config.CriticalMethod == (int)(ExpressionMethods.LessThen) && c.Config.CriticalValue > value) ||
                (c.Config.CriticalMethod == (int)(ExpressionMethods.GreaterOrEqual) && c.Config.CriticalValue <= value) ||
                (c.Config.CriticalMethod == (int)(ExpressionMethods.LessOrEqual) && c.Config.CriticalValue >= value))
            {
                level = Nagios.Net.Client.Nrpe.MessageState.STATE_CRITICAL;
            }
            else if ((c.Config.WarningMethod == (int)(ExpressionMethods.Equal) && c.Config.WarningValue == value) ||
                (c.Config.WarningMethod == (int)(ExpressionMethods.GreateThen) && c.Config.WarningValue < value) ||
                (c.Config.WarningMethod == (int)(ExpressionMethods.LessThen) && c.Config.WarningValue > value) ||
                (c.Config.WarningMethod == (int)(ExpressionMethods.GreaterOrEqual) && c.Config.WarningValue <= value) ||
                (c.Config.WarningMethod == (int)(ExpressionMethods.LessOrEqual) && c.Config.WarningValue >= value))
            {
                level = Nagios.Net.Client.Nrpe.MessageState.STATE_WARNING;
            }
            else
                level = Nagios.Net.Client.Nrpe.MessageState.STATE_OK;
            message = value.ToString();
        }

        public List<string> GetRegisteredCommands()
        {
            return _counters.Select(x => x.Config.Command).ToList();
        }

        public void Initialize()
        {
            Configurate();
        }

        public event EventHandler CommandsChanged;
        private void RaiseCommandsChanged()
        {
            if (CommandsChanged != null)
                CommandsChanged.Invoke(this, new EventArgs());
        }
        #endregion

        private void Configurate()
        {
            _timer.Enabled = false;

            // read config
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), NrpePerfCountersModule.Configurator.ConfigConstants.ConfigFileName);
                System.Configuration.Configuration _cfg = ConfigurationManager.OpenExeConfiguration(path);
                if (_cfg.AppSettings.Settings.AllKeys.Contains(ConfigConstants.Counters) == true)
                {
                    _counters.Clear();
                    PerfCounters cc = PerfCounters.Load(_cfg.AppSettings.Settings[ConfigConstants.Counters].Value);
                    if (cc != null)
                    {
                        this._counters.ForEach(x => x.Dispose());
                        this._counters.Clear();
                        foreach (PerfCounter pc in cc.Counters)
                        {
                            PCounter c = new PCounter();
                            c.Config = pc;
                            if (pc.Instance != "Default")
                                c.Counter = new System.Diagnostics.PerformanceCounter(pc.Category, pc.Counter, pc.Instance, true);
                            else
                                c.Counter = new System.Diagnostics.PerformanceCounter(pc.Category, pc.Counter, true);
                            _counters.Add(c);
                        }
                        RaiseCommandsChanged();
                    }
                }

            }
            catch (Exception ex)
            {
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
            }

            _timer.Enabled = true;
        }

        public void Dispose()
        {
            if (_counters.Count > 0)
            {
                _counters.ForEach(x => { x.Dispose(); });
                _counters.Clear();
            }
        }
    }
}
