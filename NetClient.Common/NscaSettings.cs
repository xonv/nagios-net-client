﻿// Copyright (c) 2012, XBRL Cloud Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer. Redistributions in
// binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or
// other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
// IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
