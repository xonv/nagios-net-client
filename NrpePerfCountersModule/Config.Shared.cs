using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NrpePerfCountersModule.Configurator
{
    public sealed class ConfigConstants
    {
        public const string ConfigFileName = "NrpePerfCountersModule.dll"; // assembly name must be set as string here because this class used in other project by link

        public const string Counters = "Counters"; // config key name
    }
}
