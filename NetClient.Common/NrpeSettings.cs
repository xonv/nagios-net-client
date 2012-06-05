// Copyright (c) 2012, XBRL Cloud Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;

namespace Nagios.Net.Client.Nrpe
{
    public sealed class NrpeSettings : ConfigurationSection
    {
        [ConfigurationProperty("ip", DefaultValue = "Any", IsRequired = false)]
        public string IP
        {
            get { return (string)this["ip"]; }
            set { this["ip"] = value; }
        }

        /// <summary>
        /// The port on which Nrpe is listening. Defaults to 5666
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = 5666, IsRequired = false)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 65535, MinValue = 1)]
        public int Port
        {
            get { return (int)this["port"]; }
            set
            {
                if (value < 1 || value > 65535)
                    throw new ArgumentOutOfRangeException("port", "Port value must be between 1 and 65535");
                this["port"] = value;
            }
        }

        [ConfigurationProperty("ssl", DefaultValue = false, IsRequired = false)]
        public bool SSL
        {
            get { return (bool)this["ssl"]; }
            set { this["ssl"] = value; }
        }

        [ConfigurationProperty("hosts")]
        [ConfigurationCollection(typeof(FilteredHosts), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public FilteredHosts Hosts
        {
            get
            {
                FilteredHosts hs = (FilteredHosts)this["hosts"];
                return hs;
            }
        }
    }

    public class FilteredHosts : ConfigurationElementCollection
    {
        public FilteredHosts()
        {
            FilteredHost host = (FilteredHost)CreateNewElement();
            Add(host);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FilteredHost();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((FilteredHost)element).Host;
        }

        public FilteredHost this[int index]
        {
            get
            {
                return (FilteredHost)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public FilteredHost this[string Host]
        {
            get
            {
                return (FilteredHost)BaseGet(Host);
            }
        }

        public int IndexOf(FilteredHost host)
        {
            return BaseIndexOf(host);
        }

        public void Add(FilteredHost host)
        {
            BaseAdd(host);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(FilteredHost host)
        {
            if (BaseIndexOf(host) >= 0)
                BaseRemove(host.Host);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }

    public class FilteredHost : ConfigurationElement, INotifyPropertyChanged
    {
        public FilteredHost(string Host)
        {
            this.Host = Host;
        }

        public FilteredHost()
        {
            //Host = "Any";
        }

        [ConfigurationProperty("host", DefaultValue = "127.0.0.1", IsRequired = false)]
        public string Host
        {
            get { return (string)this["host"]; }
            set { this["host"] = value; RaiseProperyChanged("Host"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaiseProperyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
