using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nagios.Net.Client
{
    public class Log
    {
        public const string EventLogSourceName = "Nagios.Net";
        public const string EventLogName = "Nagios .NET Client";

        public static void WriteLog(string msg, bool isError, bool toConsole = false)
        {
            if (toConsole == true)
            {
                Console.WriteLine(msg);
                return;
            }
            if (!EventLog.SourceExists(Log.EventLogSourceName))
            {
                EventLog.CreateEventSource(Log.EventLogSourceName, Log.EventLogName);
            }

            EventLog log = new EventLog(EventLogName);
            if (log != null)
            {
                log.Source = EventLogSourceName;
                log.WriteEntry(msg, isError ? EventLogEntryType.Error : EventLogEntryType.Information);
            }
        }

        public static EventLog GetLog()
        {
            if (!EventLog.SourceExists(Log.EventLogSourceName))
            {
                EventLog.CreateEventSource(Log.EventLogSourceName, Log.EventLogName);
            }

            EventLog log = new EventLog(EventLogName);
            return log;
        }
    }
}
