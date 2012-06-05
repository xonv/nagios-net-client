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
using System.Net.Sockets;
using System.Net.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Runtime.InteropServices;
using Nagios.Net.Client.Common;

namespace Nagios.Net.Client.Nrpe
{
    internal class NrpeReader
    {
        volatile TcpClient _client;
        volatile Stream _stream;
        byte[] _data = new byte[Nagios.Net.Client.Nrpe.NrpeConstants.MAX_INPUT_BUFFER];
        volatile int _bytesRead = 0;

        internal void Begin(TcpClient c)
        {
            try
            {
                _client = c;
                _stream = c.GetStream();
                _stream.ReadTimeout = Nagios.Net.Client.Nrpe.NrpeConstants.DEFAULT_SOCKET_TIMEOUT;
                _stream.WriteTimeout = Nagios.Net.Client.Nrpe.NrpeConstants.DEFAULT_SOCKET_TIMEOUT;
                Read();
            }
            catch (Exception ex) { ProcessException(ex); }
        }

        internal void BeginSsl(TcpClient c)
        {
            try
            {
                _client = c;

                // Creates the key pair
                byte[] dh512_p = {
                                     0xA4,0x56,0x47,0x7F,0x90,0xF0,0xDE,0xFE,0x73,0x1A,0xBD,0x3E,
                                     0xA9,0xF5,0x69,0x46,0x29,0x0B,0x47,0x55,0x8C,0xE8,0xF3,0xDF,
                                     0xF6,0x1B,0xC5,0x29,0x1B,0x81,0x97,0x3E,0xE4,0xD9,0xC8,0x2B,
                                     0xBB,0x2B,0x7A,0x37,0xE1,0x18,0xDF,0xEC,0x6B,0xEC,0x04,0x77,
                                     0x6D,0x51,0x3C,0x7C,0xB7,0x81,0xBD,0x7F,0xC9,0x5A,0x04,0xB4,
                                     0xA4,0x3E,0x8B,0x5B
                                 };

                OpenSSL.Core.BigNumber b1 = OpenSSL.Core.BigNumber.FromArray(dh512_p);
                OpenSSL.Core.BigNumber b2 = OpenSSL.Core.BigNumber.FromArray(new byte[] { 0x02 });
                var dh = new OpenSSL.Crypto.DH(b1, b2);


                _stream = new OpenSSL.SSL.SslAnonStream(_client.GetStream(), false);
                ((OpenSSL.SSL.SslAnonStream)_stream).AuthenticateAsServer(dh, OpenSSL.SSL.SslProtocols.Tls, OpenSSL.SSL.SslStrength.All);

                _stream.ReadTimeout = Nagios.Net.Client.Nrpe.NrpeConstants.DEFAULT_SOCKET_TIMEOUT;
                _stream.WriteTimeout = Nagios.Net.Client.Nrpe.NrpeConstants.DEFAULT_SOCKET_TIMEOUT;
                Read();
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
        }

        void Read()
        {
            _stream.BeginRead(_data, _bytesRead, Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH - _bytesRead, ReadCallback, null);
        }

        void ReadCallback(IAsyncResult r)
        {
            try
            {
                int chunkSize = _stream.EndRead(r);
                _bytesRead += chunkSize;
                if (chunkSize > 0 && _bytesRead < Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH)
                {
                    Read(); // More data to read!
                    return;
                }
                ProcessCommand();
                _stream.BeginWrite(_data, 0, Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH, WriteCallback, null);
            }
            catch (Exception ex) { ProcessException(ex); }
        }

        void WriteCallback(IAsyncResult r)
        {
            try
            {
                _stream.EndWrite(r);
            }
            catch (Exception ex)
            {
                ProcessException(ex);
            }
            Cleanup();
        }

        void ProcessException(Exception ex)
        {
            Cleanup();
            Log.WriteLog("Error: " + ex.Message + "\n" + ex.StackTrace, true);
        }

        void Cleanup()
        {
            if (_stream != null) _stream.Close();
            if (_client != null) _client.Close();
        }

        void ProcessCommand()
        {
            byte[] requestMessage = new byte[Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKETBUFFER_LENGTH];
            Array.Copy(_data, 10, requestMessage, 0, Math.Min(_data.Length - 10, requestMessage.Length));

            string tmp = System.Text.ASCIIEncoding.ASCII.GetString(requestMessage);

            string request = tmp.Substring(0, tmp.IndexOf('\0'));

            string[] prms = request.Split(new char[] { '!' }, StringSplitOptions.RemoveEmptyEntries);
            MessageState msgState = MessageState.STATE_UNKNOWN;
            string response = request;
            if (prms.Count() > 0)
            {
                string cmdName = prms[0];

                // find module that can run command
                IModule module = NrpeRegisteredCommands.GetModule(cmdName);
                if (module != null && module is INrpe)
                {
                    ((INrpe)module).RunCommand(cmdName, null, out msgState, out response);
                }

            }
            byte[] rsp = System.Text.ASCIIEncoding.ASCII.GetBytes(response);
            byte[] responceMessage = new byte[Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKETBUFFER_LENGTH];
            Array.Copy(rsp, 0, responceMessage, 0, Math.Min(rsp.Length, responceMessage.Length));

            // send response
            NrpePacketHeader hdr = new NrpePacketHeader();
            hdr.packet_version = BitConverter.ToInt16(new byte[] { _data[1], _data[0] }, 0);
            hdr.packet_type = BitConverter.ToInt16(new byte[] { _data[3], _data[2] }, 0);
            hdr.crc32_value = BytesUtils.ReverseBytes(BitConverter.ToUInt32(new byte[] { _data[4], _data[5], _data[6], _data[7] }, 0));

            using (MemoryStream ms = new MemoryStream(_data))
            {
                ms.Seek(0, SeekOrigin.Begin);
                ms.Position = 4;
                ms.WriteInt(0);

                int hash = Nagios.Net.Client.Nsca.CRC32.Compute(ms.ToArray(), 0, Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH);

                ms.Seek(0, SeekOrigin.Begin);
                ms.WriteShort((short)PacketVersion.NRPE_PACKET_VERSION_2);
                ms.WriteShort((short)PacketType.RESPONSE_PACKET);
                ms.WriteInt(0);
                ms.WriteShort((short)msgState);
                ms.Write(responceMessage, 0, responceMessage.Length);
                ms.Position = Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH - 2;
                ms.WriteShort(0);

                int hash2 = Nagios.Net.Client.Nsca.CRC32.Compute(ms.ToArray(), 0, Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH);
                ms.Position = 4;
                ms.WriteInt(hash2);

                _data = ms.ToArray().Take(Nagios.Net.Client.Nrpe.NrpeConstants.MAX_PACKET_LENGTH).ToArray();
            }
        }
    }
}
