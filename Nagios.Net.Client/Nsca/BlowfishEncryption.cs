using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nagios.Net.Client.Nsca
{
    public class BlowfishEncryption : NscaEncryptionBase
    {
        int ivSize = 8;
        int keySize = 56;
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            byte[] keyBytes = new byte[keySize];
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, keyBytes, 0, Math.Min(keySize, passwordBytes.Length));

            byte[] iv = new byte[ivSize];
            Buffer.BlockCopy(initVector, 0, iv, 0, Math.Min(ivSize, iv.Length));

            using (MCryptEncryptor crypto = new MCryptEncryptor(NativeConstants.MCRYPT_BLOWFISH, keyBytes, iv))
            {
                byte[] outBuf = crypto.Encrypt(s);
                return outBuf;
            }
        }
    }
}
