using System;
using System.Configuration;

namespace Nagios.Net.Client.Nsca
{
    // Created by Robert Mircea https://github.com/robertmircea/nagios-Nsca-client
    // 2011
    // Free license

    /// <summary>
    /// Settings necessary for sending passive checks to Nagios Nsca daemon
    /// </summary>
    public sealed class NscaSettings : ConfigurationSection
    {

        [ConfigurationProperty("nscaAddress", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string NscaAddress
        {
            get { return (string) this["nscaAddress"]; }
            set { this["nscaAddress"] = value; }
        }

        [ConfigurationProperty("nscaHostName", DefaultValue = "", IsRequired = false)]
        public string NscaHostName
        {
            get { return (string)this["nscaHostName"]; }
            set { this["nscaHostName"] = value; }
        }

        /// <summary>
        /// The port on which Nsca is listening. Defaults to 5667
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = 5667, IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 65535, MinValue = 1)]
        public int Port
        {
            get { return (int) this["port"]; }
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentOutOfRangeException("port", "Port value must be between 1 and 65535");
                this["port"] = value;
            }
        }

        /// <summary>
        /// The password configured in the ncsa.cfg file used by Nsca
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        public string Password
        {
            get { return (string) this["password"]; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("encryptionType", DefaultValue = NscaEncryptionType.Xor, IsRequired = false)]
        public NscaEncryptionType EncryptionType
        {
            get { return (NscaEncryptionType) this["encryptionType"]; }
            set { this["encryptionType"] = value; }
        }
    }

}
