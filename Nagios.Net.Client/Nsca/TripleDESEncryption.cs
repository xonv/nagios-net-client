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