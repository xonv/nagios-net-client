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