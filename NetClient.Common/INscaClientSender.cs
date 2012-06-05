using System;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-nsca-client
    // 2011
    // Free license

    public interface INscaClientSender
    {
        /// <summary>
        /// Send a passive check described by message payload to a Nagios Nsca daemon
        /// </summary>        
        bool SendPassiveCheck(Level level, string hostName, string serviceName, string message);
    }
}