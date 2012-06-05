namespace Nagios.Net.Client.Nsca
{
    public class NullNscaClientSender : INscaClientSender
    {
        // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
        // 2011
        // Free license

        private static readonly NullNscaClientSender instance = new NullNscaClientSender();

        public static NullNscaClientSender Instance { get { return instance; } }

        public bool SendPassiveCheck(Level level, string hostName, string serviceName, string message)
        {
            return true;
        }
    }
}