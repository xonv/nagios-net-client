using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common
{
    public interface INrpe : IModule
    {
        void RunCommand(string command, string[] parameters, out Nagios.Net.Client.Nrpe.MessageState level, out string message);
        List<string> GetRegisteredCommands(); // return the list of registered commands
        event EventHandler CommandsChanged;
        void Initialize(); // load config file and init module
    }
}
