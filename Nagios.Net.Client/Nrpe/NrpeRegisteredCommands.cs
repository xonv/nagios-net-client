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
