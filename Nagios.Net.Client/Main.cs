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
using System.IO;
using System.ComponentModel.Composition.Hosting;
using Nagios.Net.Client.Common;
using System.Reflection;
using System.ComponentModel.Composition;
using Nagios.Net.Client.Nrpe;

namespace Nagios.Net.Client
{
    public sealed class Main : IDisposable
    {
        FileSystemWatcher watcherConfig;
        private CompositionContainer _container;
        IEnumerable<Lazy<IModule>> _lazyModules;
        List<IModule> _modules;
        NrpeListener _nrpeListener;

        public Main()
        {
            _modules = new List<IModule>();

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            _nrpeListener = new NrpeListener();
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            watcherConfig = new FileSystemWatcher(dir, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".exe.config");
            watcherConfig.Changed += new FileSystemEventHandler(OnConfigChanghed);

            watcherConfig.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            watcherConfig.EnableRaisingEvents = false;
        }

        #region Config

        void OnConfigChanghed(object sender, FileSystemEventArgs e)
        {
            watcherConfig.EnableRaisingEvents = false;
            WriteLog("Load updated config data", false);
            Config();
            watcherConfig.EnableRaisingEvents = true;
            WriteLog("Service updated by new config", false);
        }

        public void Config()
        {
            UpdateNrpeSettings();
            try
            {
                string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (_container == null)
                {
                    var catalog = new AggregateCatalog();
                    catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
                    string modulesFolder = Path.Combine(dir, "Modules");
                    catalog.Catalogs.Add(new DirectoryCatalog(modulesFolder));

                    _container = new CompositionContainer(catalog);
                }
                // load and initialize modules
                try
                {
                    this._container.ComposeParts(this);

                    this._lazyModules = this._container.GetExports<IModule>();
                    WriteLog("Loaded " + this._lazyModules.Count().ToString() + " modules", false);

                }
                catch (CompositionException compositionException)
                {
                    WriteLog(compositionException.ToString(), true);
                }
                catch (ReflectionTypeLoadException tex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(tex.Message);
                    if (tex.LoaderExceptions != null)
                    {
                        tex.LoaderExceptions.ToList().ForEach(x => sb.AppendLine(x.Message));
                    }
                    WriteLog(sb.ToString(), true);
                }


            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, true);
            }
        }


        #endregion


        public void RunModules()
        {
            StartListener();
            // run modules
            if (_lazyModules.Count() > 0)
            {
                foreach (Lazy<IModule> m in _lazyModules)
                {
                    try
                    {
                        var module = m.Value;
                        if (module != null)
                        {
                            _modules.Add(module);

                            if (module is INsca)
                            {
                                ((INsca)module).NscaCheck += new NscaCheckEventHandler(Main_NscaCheck);

                                WriteLog(string.Format("NSCA module {0} initialized", module.ModuleName), false);
                                ((INsca)module).Run();
                                WriteLog(string.Format("NSCA module {0} run", module.ModuleName), false);

                            }
                            else if (module is INrpe)
                            {
                                ((INrpe)module).CommandsChanged += (s, e) => {
                                    NrpeRegisteredCommands.RegisterModule(module);
                                };
                                NrpeRegisteredCommands.RegisterModule(module);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message, true);
                    }
                }
            }
        }

        void Main_NscaCheck(object sender, NscaCheckEventArgs e)
        {
            try
            {
                Nsca.NscaClientSender ns = new Nsca.NscaClientSender();
                if (false == ns.SendPassiveCheck(e.Level, System.Environment.MachineName, e.Service, e.Message))
                    WriteLog("NSCA message is not sent: " + e.Service + " - " + e.Message, false);
            }
            catch (Exception ex)
            {
                WriteLog("Send NSCA message: " + ex.Message, true);
            }
        }

        public void StopModules()
        {
            StopListener();

            foreach (IModule m in _modules)
            {
                try
                {
                    if (m is INsca)
                    {
                        if (true == ((INsca)m).CanStop)
                        {
                            ((INsca)m).Stop();
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(string.Format("Can't stop module {0}: {1}", m.ModuleName, ex.Message), true);
                }
            }
        }

        #region Listener

        private void UpdateNrpeSettings()
        {
            _nrpeListener.UpdateConfig();
        }

        private void StartListener()
        {
            _nrpeListener.Start();
        }

        private void StopListener()
        {
            _nrpeListener.Stop();
        }

        #endregion

        #region Log

        private void WriteLog(string msg, bool isError)
        {
            Log.WriteLog(msg, isError);
        }

        #endregion


    }
}
