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
using System.Net.Sockets;
using System.ComponentModel.Composition;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license
    [Export(typeof(INscaClientSender))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NscaClientSender : INscaClientSender
    {
        private readonly NscaSettings settings;
        private readonly PassiveCheckProtocolWriter protocolWriter;

        public NscaClientSender()
            : this((NscaSettings)System.Configuration.ConfigurationManager.GetSection("nscaSettings"))
        {
        }

        public NscaClientSender(NscaSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            this.settings = settings;

            protocolWriter = new PassiveCheckProtocolWriter(settings);
        }

        public bool SendPassiveCheck(Level level, string hostName, string serviceName, string message)
        {
            if (string.IsNullOrEmpty(hostName)) throw new ArgumentNullException("hostName");
            if (message == null) throw new ArgumentNullException("message");

            string host = hostName;
            if (string.IsNullOrWhiteSpace(settings.NscaHostName) == false)
                host = settings.NscaHostName;
            bool rVal = false;
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(settings.NscaAddress, settings.Port);
                    var stream = tcpClient.GetStream();

                    try
                    {
                        byte[] initVector = new byte[128];
                        stream.Read(initVector, 0, 128);

                        byte[] timestamp = new byte[4];
                        stream.Read(timestamp, 0, 4);

                        var bytesToSend = protocolWriter.EncodeToProtocol(level, timestamp, host, serviceName, message, initVector);
                        if (bytesToSend != null && bytesToSend.Length > 0)
                        {
                            stream.Write(bytesToSend, 0, bytesToSend.Length);
                            rVal = true;
                        }
                    }
                    catch (Exception eio)
                    {
                        Log.WriteLog("NscaClientSender stream: " + eio.Message, true);

                        System.Diagnostics.Debug.WriteLine(eio.Message);
                        System.Diagnostics.Debug.WriteLine(eio.StackTrace);
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("NscaClientSender: " + ex.Message, true);

                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                //intentionally swallow exceptions, maybe some logging is required.
            }
            return rVal;
        }
    }
}