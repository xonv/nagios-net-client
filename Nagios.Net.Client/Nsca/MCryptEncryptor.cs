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

namespace Nagios.Net.Client.Nsca
{

    public class MCryptEncryptor : IDisposable
    {
        IntPtr td;
        byte[] algName;
        byte[] mode;

        public MCryptEncryptor(string algorithm, byte[] key, byte[] iv)
        {
            unsafe
            {
                algName = System.Text.ASCIIEncoding.ASCII.GetBytes(algorithm);
                mode = System.Text.ASCIIEncoding.ASCII.GetBytes(NativeConstants.MCRYPT_CFB);
                fixed (byte* alg = algName)
                {
                    fixed (byte* m = mode)
                    {
                        td = NativeMethods.mcrypt_module_open(new IntPtr(alg), IntPtr.Zero, new IntPtr(m), IntPtr.Zero);
                    }
                }
                fixed (byte* k = key)
                {
                    fixed (byte* i = iv)
                    {
                        NativeMethods.mcrypt_generic_init(td, new IntPtr(k), key.Length, new IntPtr(i));
                    }
                }
            }
        }

        public byte[] Encrypt(byte[] buf)
        {
            byte[] outBuf = new byte[buf.Length];
            Array.Copy(buf, outBuf, buf.Length);

            for (int x = 0; x < buf.Length; x++)
                unsafe
                {
                    fixed (byte* p = outBuf)
                    {
                        NativeMethods.mcrypt_generic(td, new IntPtr(p + x), 1);
                    }
                }
            return outBuf;
        }

        public void Dispose()
        {
            unsafe
            {
                NativeMethods.mcrypt_generic_end(td);
            }
        }
    }
}
