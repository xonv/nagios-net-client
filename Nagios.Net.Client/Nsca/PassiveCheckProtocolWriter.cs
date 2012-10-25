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
            byte[] bufOut = null;
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
                bufOut = encryptor.Encrypt(stream.ToArray(), initVector, settings.Password);
            }
            return bufOut;
        }
    }
}