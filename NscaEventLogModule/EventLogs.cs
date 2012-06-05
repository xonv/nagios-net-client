using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NscaEventLogModule
{
    [Serializable]
    public class EventLogs
    {
        public EventLogs()
        {
            Logs = new List<EventLogDescription>();
        }

        public List<EventLogDescription> Logs { get; set; }

        #region Factory methods

        public static EventLogs Load(string data)
        {
            if (string.IsNullOrWhiteSpace(data) == true)
                throw new ArgumentException("The EventLogs settings data is not set.");

            return Deserialize(data);
        }

        #endregion Factory methods

        #region Serilalization

        protected static DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(EventLogs));
        }

        protected static EventLogs Deserialize(string data)
        {
            EventLogs o = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            try
            {
                DataContractJsonSerializer ser = GetSerializer();
                o = ser.ReadObject(ms) as EventLogs;
            }
            catch (Exception e)
            {
                throw new EventLogLoadException(string.Format("Error thrown then read EventLog settings: {0}", e.Message));
            }
            finally
            {
                ms.Close();
            }
            return o;
        }

        protected string Serialize()
        {
            MemoryStream ms = new MemoryStream();
            GetSerializer().WriteObject(ms, this);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public string GetSerialized()
        {
            return Serialize();
        }

        #endregion Serilalization
    }

    public class EventLogDescription
    {
        public string EventLogName { get; set; }

        public string LogSources { get; set; }

        public string EventIds { get; set; }

        public string Keywords { get; set; }

        public bool IsCritical { get; set; }
        public bool IsWarning { get; set; }
        public bool IsVerbose { get; set; }
        public bool IsError { get; set; }
        public bool IsInformation { get; set; }

        public string NagiosServiceName { get; set; }
        public string NagiosServiceDescription { get; set; }
        public Nagios.Net.Client.Nsca.Level MessageLevel { get; set; }
    }

    public class EventLogLoadException : Exception
    {
        public EventLogLoadException() : base() { }

        public EventLogLoadException(string m) : base(m) { }
    }
}