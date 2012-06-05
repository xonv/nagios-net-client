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
using System.IO;

namespace Nagios.Net.Client.Common
{
    public static class MemoryStreamExtensions
    {
        public static void WriteShort(this MemoryStream stream, short value)
        {
            byte[] shortBuf = new byte[2];
            shortBuf[1] = (byte)(value & 0xff);
            shortBuf[0] = (byte)((value >> 8) & 0xff);
            stream.Write(shortBuf, 0, shortBuf.Length);
        }

        public static void WriteInt(this MemoryStream stream, int value)
        {
            byte[] intBuf = new byte[4];
            intBuf[3] = (byte)(value & 0xff);
            intBuf[2] = (byte)((value >> 8) & 0xff);
            intBuf[1] = (byte)((value >> 16) & 0xff);
            intBuf[0] = (byte)((value >> 24) & 0xff);

            stream.Write(intBuf, 0, intBuf.Length);
        }

        private const string breakSymbol = @"<br>";

        public static void WriteFixedString(this MemoryStream stream, string value, int size)
        {
            if (value == null)
                return;

            var b = new byte[size];

            if (value.Length == 0)
            {
                stream.Write(b, 0, b.Length);
                return;
            }

            if (value.Length > size)
                value = value.Substring(0, size);

            var buffer = Encoding.ASCII.GetBytes(value).ToList();

            // change 0x0d & 0x0a symbols to Encoding.ASCII.GetBytes(breakSymbol);
            List<byte> fixedBuffer = new List<byte>();
            foreach (byte bx in buffer)
            {
                if (bx == 0x0d) // no needed symbol
                    continue;
                if (bx == 0x0a)
                {
                    fixedBuffer.AddRange(Encoding.ASCII.GetBytes(breakSymbol));
                    continue;
                }
                fixedBuffer.Add(bx);
            }
            var fb = fixedBuffer.ToArray();
            Buffer.BlockCopy(fb, 0, b, 0, Math.Min(fb.Count(), b.Count()));

            stream.Write(b, 0, b.Length);
        }
    }
}
