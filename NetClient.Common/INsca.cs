using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common
{
    public delegate void NscaCheckEventHandler(object sender, NscaCheckEventArgs e);

    public interface INsca : IModule
    {
        event NscaCheckEventHandler NscaCheck;

        void Run(); // run module timer for execute embedded commands
        bool CanStop { get; } // == true if any commands are not running
        void Stop(); // stop timer
    }

    public class NscaCheckEventArgs : EventArgs
    {
        public NscaCheckEventArgs(string service, Nagios.Net.Client.Nsca.Level level, string message)
        {
            Service = service;
            Level = level;
            Message = message;
        }

        public string Service { get; private set; }
        public Nagios.Net.Client.Nsca.Level Level { get; private set; }
        public string Message { get; private set; }
    }
}
