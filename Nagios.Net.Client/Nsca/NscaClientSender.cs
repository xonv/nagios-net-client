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
            : this((NscaSettings) System.Configuration.ConfigurationManager.GetSection("nscaSettings"))
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
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    tcpClient.Connect(settings.NscaAddress, settings.Port);
                    using (var stream = tcpClient.GetStream())
                    {
                        byte[] initVector = new byte[128];
                        stream.Read(initVector, 0, 128);

                        byte[] timestamp = new byte[4];
                        stream.Read(timestamp, 0, 4);

                        var bytesToSend = protocolWriter.EncodeToProtocol(level, timestamp, host, serviceName, message, initVector);
                        stream.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("NscaClientSender: " + ex.Message, true);

                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                //intentionally swallow exceptions, maybe some logging is required.
                return false;
            }
        }
    }
}