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
using System.Security.Cryptography;
using System.Text;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    // WARNING!!! This encryptor use the native MS 3DES Crypto Service Provider
    public class TripleDESEncryption : NscaEncryptionBase
    {
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            byte[] keyBytes = new byte[24];
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, Math.Min(24, passwordBytes.Length));

            byte[] iv = new byte[8];
            Buffer.BlockCopy(initVector, 0, iv, 0, Math.Min(8, iv.Length));

            using (TripleDES crypto = new TripleDESCryptoServiceProvider())
            {
                crypto.Mode = CipherMode.CFB;
                crypto.Padding = PaddingMode.PKCS7;

                byte[] outBuf = crypto.CreateEncryptor(keyBytes, iv).TransformFinalBlock(s, 0, s.Length);
                return outBuf;
            }
        }
    }
}