using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace NrpePerfCountersModule
{
    public class PerfCounters
    {
        public PerfCounters()
        {
            Counters = new List<PerfCounter>();
        }

        public List<PerfCounter> Counters { get; set; }

        #region Factory Methods

        public static PerfCounters Load(string data)
        {
            if (string.IsNullOrWhiteSpace(data) == true)
                throw new ArgumentException("The PerfCounters data is not set.");

            return Deserialize(data);
        }

        #endregion

        #region Serialization
        protected static DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(PerfCounters));
        }

        protected static PerfCounters Deserialize(string data)
        {
            PerfCounters o = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            try
            {
                DataContractJsonSerializer ser = GetSerializer();
                o = ser.ReadObject(ms) as PerfCounters;
            }
            catch (Exception e)
            {
                throw new PerfCountersLoadException(string.Format("Error thrown then read Perf Counters settings: {0}", e.Message));
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

    public struct PerfCounter
    {
        public string Category { get; set; }
        public string Instance { get; set; }
        public string Counter { get; set; }
        public int Duration { get; set; }
        public int CalcMethod { get; set; }
        public string Command { get; set; }
        public int CriticalMethod { get; set; }
        public float CriticalValue { get; set; }
        public int WarningMethod { get; set; }
        public float WarningValue { get; set; }
    }

    public class PerfCountersLoadException : Exception
    {
        public PerfCountersLoadException() : base() { }
        public PerfCountersLoadException(string m) : base(m) { }
    }

}
