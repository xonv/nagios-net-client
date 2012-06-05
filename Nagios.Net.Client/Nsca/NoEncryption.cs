namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    public class NoEncryption : NscaEncryptionBase
    {
        public override byte[] Encrypt(byte[] s, byte[] initVector, string password)
        {
            return s;
        }
    }
}