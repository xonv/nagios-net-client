using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NscaEventLogModule
{
    public delegate void LogEventHandler(object sender, EventWatcherArgs e);

    public sealed class EventWatchers
    {
        List<EventWatcher> _watchers;
        public EventWatchers()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); // need to fix MS bug
            _watchers = new List<EventWatcher>();
        }

        public event LogEventHandler SendLogEvent;

        public void Init(List<EventLogDescription> Logs)
        {
            if (Logs == null)
                return;

            try
            {
                Stop();
                _watchers.ForEach(x => { x.EventRaised -= new EventWatcherHandler(w_EventRaised); });
                _watchers.Clear();
            }
            catch (Exception e)
            {
                Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", e.Message, e.StackTrace), true);
            }

            foreach (EventLogDescription ld in Logs)
            {
                try
                {
                    EventWatcher w = new EventWatcher(ld);
                    w.EventRaised += new EventWatcherHandler(w_EventRaised);
                    _watchers.Add(w);
                }
                catch (Exception ex)
                {
                    Nagios.Net.Client.Log.WriteLog(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), true);
                }
            }
        }

        void w_EventRaised(object sender, EventWatcherArgs e)
        {
            if (SendLogEvent != null)
                SendLogEvent.Invoke(this, e);
        }

        public void Start()
        {
            foreach (EventWatcher w in _watchers)
                w.Start();
        }

        public void Stop()
        {
            foreach (EventWatcher w in _watchers)
                w.Stop();
        }
    }

    public delegate void EventWatcherHandler(object sender, EventWatcherArgs e);

    public class EventWatcher : IDisposable
    {
        EventLogWatcher watcher;
        EventLogDescription decription;
        int[] supressedIDs;
        Regex _rxFilter;

        public EventWatcher()
        {

        }

        public EventWatcher(EventLogDescription description)
            : this()
        {
            this.decription = description;
            InitWatcher();
        }

        public EventLogDescription EventDescription
        {
            get { return decription; }
            set
            {
                decription = value;
                InitWatcher();
            }
        }

        private void InitWatcher()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); // need to fix MS bug

            if (string.IsNullOrWhiteSpace(EventDescription.Keywords))
            {
                if (_rxFilter != null)
                    _rxFilter = null;
            }
            else
            {
                _rxFilter = new Regex(EventDescription.Keywords);
            }

            // create X-Path query
            // for example:
            // *[System[Provider[@Name='.NET Runtime' or @Name='.NET Runtime Optimization Service' 
            //  or @Name='Microsoft-Windows-Dhcp-Client'] and 
            // (Level=1  or Level=2 or Level=3 or Level=4 or Level=0 or Level=5) and 
            // (EventID=1 or EventID=2 or  (EventID &gt;= 4 and EventID &lt;= 7) )]]
            StringBuilder xq = new StringBuilder();
            xq.Append("*");

            StringBuilder paths = new StringBuilder();
            if (string.IsNullOrWhiteSpace(EventDescription.LogSources) == false && EventDescription.LogSources.StartsWith("Please") == false)
            {
                // add paths
                string[] ps = EventDescription.LogSources.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                ps.ToList().ForEach(x => { if (paths.Length > 0) paths.Append(" or "); paths.Append("@Name='" + x.Trim() + "'"); });
            }

            StringBuilder ls = new StringBuilder();
            if (EventDescription.IsCritical || EventDescription.IsError || EventDescription.IsInformation || EventDescription.IsVerbose || EventDescription.IsWarning)
            {
                // add levels
                if (EventDescription.IsCritical)
                {
                    ls.Append("Level=" + ((int)StandardEventLevel.Critical).ToString());
                }
                if (EventDescription.IsError)
                {
                    if (ls.Length > 0)
                        ls.Append(" or ");
                    ls.Append("Level=" + ((int)StandardEventLevel.Error).ToString());
                }
                if (EventDescription.IsInformation)
                {
                    if (ls.Length > 0)
                        ls.Append(" or ");
                    ls.Append("Level=" + ((int)StandardEventLevel.Informational).ToString() + " or Level=" + ((int)StandardEventLevel.LogAlways).ToString());
                }
                if (EventDescription.IsVerbose)
                {
                    if (ls.Length > 0)
                        ls.Append(" or ");
                    ls.Append("Level=" + ((int)StandardEventLevel.Verbose).ToString());
                }
                if (EventDescription.IsWarning)
                {
                    if (ls.Length > 0)
                        ls.Append(" or ");
                    ls.Append("Level=" + ((int)StandardEventLevel.Warning).ToString());
                }
            }

            StringBuilder iid = new StringBuilder();
            // append Event IDs (without exclude expression)
            if (string.IsNullOrWhiteSpace(EventDescription.EventIds) == false)
            {
                // supressed IDs
                supressedIDs = EventDescription.EventIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.StartsWith("-") == true).Select(x => x.Substring(1)).Cast<int>().ToArray();

                List<string> iids = EventDescription.EventIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.StartsWith("-") == false).ToList();
                if (iids.Where(x => x.Contains("-") == false).Count() > 0)
                    iids.Where(x => x.Contains("-") == false).ToList().ForEach(x => { if (iid.Length > 0) iid.Append(" or "); iid.Append("EventID = " + x); });
                if (iids.Where(x => x.Contains("-") == true).Count() > 0)
                    iids.Where(x => x.Contains("-") == true).ToList().ForEach(x =>
                    {
                        if (iid.Length > 0) iid.Append(" or ");
                        string[] xx = x.Split(new char[] { '-' });
                        iid.Append("(EventID >= " + xx[0] + " and EventID <= " + xx[1] + ")");
                    });
            }


            if (paths.Length > 0 || ls.Length > 0 || iid.Length > 0)
            {
                xq.Append("[System[");

                if (paths.Length > 0)
                {
                    xq.Append("Provider[");
                    xq.Append(paths.ToString());
                    xq.Append("]");
                }

                if (ls.Length > 0)
                {
                    if (paths.Length > 0)
                        xq.Append(" and ");
                    xq.Append("(" + ls.ToString() + ")");
                }

                if (iid.Length > 0)
                {
                    if (paths.Length > 0 || ls.Length > 0)
                        xq.Append(" and ");
                    xq.Append("(");
                    xq.Append(iid.ToString());
                    xq.Append(")");

                }

                xq.Append("]]");
            }


            EventLogQuery subscriptionQuery = new EventLogQuery(decription.EventLogName, PathType.LogName, xq.ToString());

            watcher = new EventLogWatcher(subscriptionQuery);
            watcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(EventLogEventRead);
        }

        public void Start()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); // need to fix MS bug
            if (watcher != null)
                watcher.Enabled = true;
        }

        public void Stop()
        {
            if (watcher != null)
                watcher.Enabled = false;
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.Enabled = false;
                watcher.Dispose();
            }
        }


        public void EventLogEventRead(object obj, EventRecordWrittenEventArgs arg)
        {
            try
            {
                if (arg.EventRecord != null)
                {
                    // check on keywords in the General Description and send message to the Nagios server
                    if (supressedIDs != null && supressedIDs.Contains(arg.EventRecord.Id))
                        return;

                    if (EventRaised != null)
                    {
                        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); // need to fix MS bug

                        EventLogRecord r = (EventLogRecord)arg.EventRecord;
                        string msg = r.FormatDescription();
                        string mPath = "";

                        if (string.IsNullOrWhiteSpace(msg))
                        {
                            using (var eln = new System.Diagnostics.EventLog(r.LogName, r.MachineName))
                            {
                                System.Diagnostics.EventLogEntryCollection eCollection = eln.Entries;
                                int cnt = eCollection.Count;
                                for (int i = cnt - 1; i >= Math.Max(0, cnt - 200); i--)
                                {
                                    var xe = eCollection[i];
                                    if (xe.Index == r.RecordId)
                                    {
                                        msg = xe.Message;
                                        mPath = " s";
                                        break;
                                    }
                                }
                            }
                        }

                        if (_rxFilter != null && string.IsNullOrWhiteSpace(msg) == false && !_rxFilter.IsMatch(msg))
                            return;
                        
                        string fMsg = string.Format("{0}, EventID = {1}{2}{3}", arg.EventRecord.TimeCreated.HasValue ? arg.EventRecord.TimeCreated : DateTime.Now, r.Id & 0xFFFF, System.Environment.NewLine, msg);
                        EventRaised.Invoke(this, new EventWatcherArgs(this.EventDescription.NagiosServiceName, this.EventDescription.MessageLevel,
                            fMsg));
                    }

                }
            }
            catch (Exception ex)
            {
                Nagios.Net.Client.Log.WriteLog(ex.Message + "\n" + ex.StackTrace, true);

            }
        }

        public event EventWatcherHandler EventRaised;
    }

    public class EventWatcherArgs : EventArgs
    {
        string serviceName;
        Nagios.Net.Client.Nsca.Level level;
        string message;

        public EventWatcherArgs(string serviceName, Nagios.Net.Client.Nsca.Level level, string message)
        {
            this.serviceName = serviceName;
            this.level = level;
            this.message = message;
        }

        public string ServiceName
        {
            get { return serviceName; }
        }
        public Nagios.Net.Client.Nsca.Level Level
        {
            get { return level; }
        }
        public string Message
        {
            get { return message; }
        }
    }
}
