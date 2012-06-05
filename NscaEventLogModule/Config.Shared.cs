using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NscaEventLogModule
{
    public sealed class ConfigConstants
    {
        public const string ConfigFileName = "NscaEventLogModule.dll"; // assembly name must be set as string here because this class used in other project by link

        public const string EventLogs = "eventLogs"; // config key name
    }
}