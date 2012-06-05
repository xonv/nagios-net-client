using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NscaWinServicesModule
{
    public sealed class ConfigConstants
    {
        public const string ConfigFileName = "NscaWinServicesModule.dll"; // assembly name must be set as string here because this class used in other project by link

        public const string Services = "services"; // config key name
        public const string NagiosServiceStartPause = "nagiosServiceStartPause"; // config key name
    }
}
