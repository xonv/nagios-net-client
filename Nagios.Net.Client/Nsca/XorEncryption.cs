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