using System;
using System.IO;
using System.Text;
using Nagios.Net.Client.Common;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    public class PassiveCheckProtocolWriter
    {
        private readonly NscaSettings settings;
        private const short Nsca_VERSION = 3;
        private const int PLUGIN_OUTPUT_SIZE = 512;
        private const int HOST_NAME_SIZE = 64;
        private const int SERVICE_NAME_SIZE = 128;

        public PassiveCheckProtocolWriter(NscaSettings settings)
        {
            this.settings = settings;
        }

        public byte[] EncodeToProtocol(Level level, byte[] timestamp, string hostName, string serviceName, string message, byte[] initVector)
        {
            using (var stream = new MemoryStream(16 + HOST_NAME_SIZE + SERVICE_NAME_SIZE + PLUGIN_OUTPUT_SIZE))
            {
                stream.WriteShort(Nsca_VERSION); //bytes 0-1
                stream.WriteShort(0); //bytes 2-3
                stream.WriteInt(0); //bytes 4-8
                stream.Write(timestamp, 0, 4); //bytes 9-13
                stream.WriteShort((short)level); //bytes 14-15                
                stream.WriteFixedString(hostName, HOST_NAME_SIZE);
                if (string.IsNullOrWhiteSpace(serviceName) == false)
                    stream.WriteFixedString(serviceName, SERVICE_NAME_SIZE); // process service check result
                else
                    stream.WriteFixedString("\x0", SERVICE_NAME_SIZE); // process host check result
                stream.WriteFixedString(message, PLUGIN_OUTPUT_SIZE);
                stream.WriteShort(0);


                int hash = CRC32.Compute(stream.ToArray());
                stream.Position = 4;
                stream.WriteInt(hash);

                var encryptor = EncryptorFactory.CreateEncryptor(settings.EncryptionType);
                byte[] bufOut = encryptor.Encrypt(stream.ToArray(), initVector, settings.Password);

                return bufOut;
            }
        }
    }


    
}