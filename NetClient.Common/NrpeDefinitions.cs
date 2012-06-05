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
using System.Runtime.InteropServices;

namespace Nagios.Net.Client.Nrpe
{
    /**************** PACKET STRUCTURE DEFINITION **********/

    public enum PacketType
    {
        QUERY_PACKET = 1, /* id code for a packet containing a query */
        RESPONSE_PACKET /* id code for a packet containing a response */
    }

    public enum PacketVersion /* packet version identifier */
    {
        NRPE_PACKET_VERSION_1 = 1, /* older packet version identifiers (no longer supported) */
        NRPE_PACKET_VERSION_2,
        NRPE_PACKET_VERSION_3 /* currently supported */
    }

    /* service state return codes */
    public enum MessageState
    {
        STATE_OK,
        STATE_WARNING,
        STATE_CRITICAL,
        STATE_UNKNOWN
    }

    public enum BOOL
    {
        FALSE,
        TRUE
    }

    public enum IsError
    {
        OK,
        ERROR
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct NrpePacketHeader
    {
        [FieldOffset(0)]
        public Int16 packet_version;
        [FieldOffset(2)]
        public Int16 packet_type;
        [FieldOffset(4)]
        public UInt32 crc32_value;
        [FieldOffset(8)]
        public Int16 result_code;
    }

    public class NrpeConstants
    {
        public const int MAX_PACKETBUFFER_LENGTH = 1024;	/* max amount of data we'll send in one query/response */
        public const int MAX_PACKET_LENGTH = 1036; // header + packet data + 2 bytes 0x00
        public const int DEFAULT_SOCKET_TIMEOUT = 10; /* timeout after 10 seconds */
        public const int DEFAULT_CONNECTION_TIMEOUT = 300;	/* timeout if daemon is waiting for connection more than this time */
        public const int MAX_INPUT_BUFFER = 2048; /* max size of most buffers we use */
        public const int MAX_FILENAME_LENGTH = 256;
        public const int MAX_HOST_ADDRESS_LENGTH = 256; /* max size of a host address */
        public const string NRPE_HELLO_COMMAND = "_NRPE_CHECK";
        public const int MAX_COMMAND_ARGUMENTS = 16;
    }
}
