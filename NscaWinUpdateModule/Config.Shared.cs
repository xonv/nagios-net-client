using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NscaWinUpdateModule.Configurator
{
    public sealed class ConfigConstants
    {
        public const string ConfigFileName = "NscaWinUpdateModule.dll"; // assembly name must be set as string here because this class used in other project by link

        public const string CheckSecurityUpdates = "checkSecurityUpdates";
        public const string CheckCriticalUpdates = "checkCriticalUpdates";
        public const string CheckUpdates = "checkUpdates";
        public const string CheckDefinitionUpdates = "checkDefinitionUpdates";
        public const string CheckFuturePacks = "checkFuturePacks";
        public const string CheckOtherUpdates = "checkOtherUpdates";

        public const string SendSecurityUpdatesAsCritical = "sendSecurityUpdatesAsCritical";
        public const string SendCriticalUpdatesAsCritical = "sendCriticalUpdatesAsCritical";
        public const string SendUpdatesAsCritical = "sendUpdatesAsCritical";
        public const string SendDefinitionUpdatesAsCritical = "sendDefinitionUpdatesAsCritical";
        public const string SendFuturePacksAsCritical = "sendFuturePacksAsCritical";
        public const string SendOtherUpdatesAsCritical = "sendOtherUpdatesAsCritical";

        public const string CriticalSchedule = "criticalSchedule";
        public const string WarningSchedule = "warningSchedule";

        public const string CriticalServiceName = "criticalServiceName";
        public const string WarningServiceName = "warningServiceName";
    }
}
