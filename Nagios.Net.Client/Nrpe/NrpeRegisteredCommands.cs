using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nagios.Net.Client.Common;

namespace Nagios.Net.Client.Nrpe
{
    public sealed class NrpeRegisteredCommands
    {
        private Dictionary<string, IModule> _commands;

        NrpeRegisteredCommands()
        {
            _commands = new Dictionary<string, IModule>();
        }

        public static void RegisterModule(IModule module)
        {
            if (module != null && module is INrpe)
            {
                // remove all commands that module registered previously
                List<string> mustBeRemoved = Instance._commands.Where(x => x.Value.ModuleName == module.ModuleName).Select(x => x.Key).ToList();
                mustBeRemoved.ForEach(x => { Instance._commands.Remove(x); });

                List<string> commands = ((INrpe)module).GetRegisteredCommands();
                foreach (string command in commands)
                {
                    if (Instance._commands.ContainsKey(command))
                        Instance._commands.Remove(command);
                    Instance._commands.Add(command, module);
                }
            }
        }

        public static IModule GetModule(string command)
        {
            if (Instance._commands.ContainsKey(command))
                return Instance._commands[command];
            return null;
        }

        private static NrpeRegisteredCommands Instance
        {
            get
            {
                return NrpeRegisteredCommandsNested.instance;
            }
        }

        class NrpeRegisteredCommandsNested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static NrpeRegisteredCommandsNested()
            {
            }

            internal static readonly NrpeRegisteredCommands instance = new NrpeRegisteredCommands();
        }
    }


}
