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

using System.Text;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    public class XorEncryption : NscaEncryptionBase
    {
        private const int INITIALISATION_VECTOR_SIZE = 128;


        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            for (int y = 0, x = 0; y < s.Length; y++, x++)
            {
                if (x >= INITIALISATION_VECTOR_SIZE)
                {
                    x = 0;
                }
                s[y] ^= initVector[x];
            }

            if (!string.IsNullOrEmpty(password))
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                for (int y = 0, x = 0; y < s.Length; y++, x++)
                {
                    if (x >= passwordBytes.Length)
                    {
                        x = 0;
                    }
                    s[y] ^= passwordBytes[x];
                }
            }

            return s;
        }

    }
}