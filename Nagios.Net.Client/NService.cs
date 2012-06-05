using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Nagios.Net.Client.Common;
using System.Reflection;

namespace Nagios.Net.Client
{
    public partial class NService : ServiceBase
    {
        Main _service;

        public NService()
        {
            InitializeComponent();
            _service = new Main();
        }

        public override EventLog EventLog
        {
            get
            {
                EventLog log = Log.GetLog();
                if (log != null)
                    return log;
                return base.EventLog;
            }
        }

        protected override void OnContinue()
        {
            _service.Config();
            _service.RunModules();
            WriteLog("Service resumed", false);

            base.OnContinue();
        }

        protected override void OnPause()
        {
            _service.StopModules();
            WriteLog("Service paused", false);
            base.OnPause();
        }

        protected override void OnStart(string[] args)
        {
            _service.Config();
            _service.RunModules();
            WriteLog("Service started", false);
        }

        protected override void OnStop()
        {
            _service.StopModules();
            WriteLog("Service stopped", false);
        }

        #region Log

        private void WriteLog(string msg, bool isError)
        {
            Log.WriteLog(msg, isError);
        }

        #endregion
    }
}
