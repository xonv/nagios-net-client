using System;

namespace Nagios.Net.Client.Nsca
{
    public class EncryptorFactory
    {
        public static NscaEncryptionBase CreateEncryptor(NscaEncryptionType encryptionType)
        {
            switch (encryptionType)
            {
                case NscaEncryptionType.Xor:
                    return new XorEncryption();
                case NscaEncryptionType.TripleDES:
                    return new TripleDESEncryption();
                case NscaEncryptionType.None:
                    return new NoEncryption();
                case NscaEncryptionType.Blowfish:
                    return new BlowfishEncryption();
                case NscaEncryptionType.AES256:
                    return new Aes256Encryption();
                case NscaEncryptionType.AES192:
                    return new Aes192Encryption();
                case NscaEncryptionType.AES128:
                    return new Aes128Encryption();
                default:
                    throw new ArgumentOutOfRangeException("encryptionType");
            }
        }
    }
}