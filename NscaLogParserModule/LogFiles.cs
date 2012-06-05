using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace NscaLogParserModule
{
    [Serializable]
    public class LogFiles
    {
        public LogFiles()
        {
            Files = new List<LogFile>();
        }

        public List<LogFile> Files { get; set; }

        #region Factory Methods

        public static LogFiles Load(string data)
        {
            if (string.IsNullOrWhiteSpace(data) == true)
                throw new ArgumentException("The LogFiles settings data is not set.");

            return Deserialize(data);
        }

        #endregion

        #region Serialization
        protected static DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(LogFiles));
        }

        protected static LogFiles Deserialize(string data)
        {
            LogFiles o = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            try
            {
                DataContractJsonSerializer ser = GetSerializer();
                o = ser.ReadObject(ms) as LogFiles;
            }
            catch (Exception e)
            {
                throw new LogFilesLoadException(string.Format("Error thrown then read LogFiles settings: {0}", e.Message));
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

        #endregion
    }

    public class LogFile
    {
        public string Folder { get; set; }
        public string FileTemplate { get; set; }
        public string MessageTemplate { get; set; }

        public string NagiosServiceName { get; set; }
        public string NagiosServiceDescription { get; set; }
        public Nagios.Net.Client.Nsca.Level MessageLevel { get; set; }
    }

    public class LogFilesLoadException : Exception
    {
        public LogFilesLoadException() : base() { }
        public LogFilesLoadException(string m) : base(m) { }
    }
}
