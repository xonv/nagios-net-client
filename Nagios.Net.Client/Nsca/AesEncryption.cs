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
    public class Aes256Encryption : NscaEncryptionBase
    {
        int ivSize = 32;
        int keySize = 32;
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            byte[] keyBytes = new byte[keySize];
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, Math.Min(keySize, passwordBytes.Length));

            byte[] iv = new byte[ivSize];
            Buffer.BlockCopy(initVector, 0, iv, 0, Math.Min(ivSize, iv.Length));

            using (MCryptEncryptor crypto = new MCryptEncryptor(NativeConstants.MCRYPT_RIJNDAEL_256, keyBytes, iv))
            {
                byte[] outBuf = crypto.Encrypt(s);
                return outBuf;
            }
        }
    }

    public class Aes192Encryption : NscaEncryptionBase
    {
        int ivSize = 24;
        int keySize = 32;
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            byte[] keyBytes = new byte[keySize];
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, Math.Min(keySize, passwordBytes.Length));

            byte[] iv = new byte[ivSize];
            Buffer.BlockCopy(initVector, 0, iv, 0, Math.Min(ivSize, iv.Length));

            using (MCryptEncryptor crypto = new MCryptEncryptor(NativeConstants.MCRYPT_RIJNDAEL_192, keyBytes, iv))
            {
                byte[] outBuf = crypto.Encrypt(s);
                return outBuf;
            }
        }
    }

    public class Aes128Encryption : NscaEncryptionBase
    {
        int ivSize = 16;
        int keySize = 32;
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            byte[] keyBytes = new byte[keySize];
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, Math.Min(keySize, passwordBytes.Length));

            byte[] iv = new byte[ivSize];
            Buffer.BlockCopy(initVector, 0, iv, 0, Math.Min(ivSize, iv.Length));

            using (MCryptEncryptor crypto = new MCryptEncryptor(NativeConstants.MCRYPT_RIJNDAEL_128, keyBytes, iv))
            {
                byte[] outBuf = crypto.Encrypt(s);
                return outBuf;
            }
        }
    }
}