using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common
{
    public interface IModule
    {
        string ModuleName { get; }
        string Version { get;  }
        string Description { get; }
    }
}
