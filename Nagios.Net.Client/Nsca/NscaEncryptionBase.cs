namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    public abstract class NscaEncryptionBase
    {
        public abstract byte[] Encrypt(byte[] s, byte[] initVector, string password);
    }
}