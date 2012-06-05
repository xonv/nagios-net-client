// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
