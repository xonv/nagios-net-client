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
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;

namespace Nagios.Net.Client.Nrpe
{
    public sealed class NrpeListener
    {
        volatile List<TcpListener> _listeners;
        private NrpeSettings _settings;

        public NrpeListener()
            : this((NrpeSettings)System.Configuration.ConfigurationManager.GetSection("nrpeSettings"))
        { }

        public NrpeListener(NrpeSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            this._settings = settings;
            _listeners = new List<TcpListener>();
        }

        public void UpdateConfig()
        {
            System.Configuration.ConfigurationManager.RefreshSection("nrpeSettings");
            this._settings = (NrpeSettings)System.Configuration.ConfigurationManager.GetSection("nrpeSettings");
        }

        private static bool IsListen = false;
        public void Start()
        {
            if (true == IsListen)
                Stop();

            List<IPAddress> ips = ParseIPBinding(_settings.IP);
            bool useSsl = _settings.SSL;

            int port = _settings.Port;

            ThreadPool.SetMinThreads(50, 50);

            _listeners.Clear();
            IsListen = true;
            foreach (IPAddress ip in ips)
            {
                ThreadPool.QueueUserWorkItem(ListenClients, Tuple.Create<IPAddress, int, bool, List<TcpListener>>(ip, port, useSsl, _listeners));
            }
        }

        public void Stop()
        {
            IsListen = false;
            _listeners.ForEach(x => { x.Stop(); });
        }

        private void ListenClients(object tObj)
        {
            Tuple<IPAddress, int, bool, List<TcpListener>> ctx = (Tuple<IPAddress, int, bool, List<TcpListener>>)tObj;
            TcpListener listener = new TcpListener(ctx.Item1, ctx.Item2);
            ctx.Item4.Add(listener);

            listener.Start();
            while (IsListen)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                if (CheckAllowedHosts(tcpClient) == false)
                    tcpClient.Close();
                else
                    ThreadPool.QueueUserWorkItem(ProcessNrpeCall, tcpClient);
            }
        }

        private bool CheckAllowedHosts(TcpClient client)
        {
            bool rslt = false;
            foreach (FilteredHost f in _settings.Hosts)
            {
                if (f.Host.ToUpper() == "ANY")
                {
                    rslt = true;
                    break;
                }
                IPAddress ip = null;
                if (IPAddress.TryParse(f.Host, out ip) == true)
                {
                    if (ip.Equals(((IPEndPoint)client.Client.RemoteEndPoint).Address))
                    {
                        rslt = true;
                    }
                }
            }
            return rslt;
        }

        private void ProcessNrpeCall(object client)
        {
            TcpClient tcpClient = client as TcpClient;
            if (tcpClient == null)
                return;

            if (true == _settings.SSL)
            {
                new NrpeReader().BeginSsl(tcpClient);
            }
            else
            {
                new NrpeReader().Begin(tcpClient);
            }
        }
        #region utils

        private List<IPAddress> ParseIPBinding(string ips)
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();

            string[] ss = ips.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Any(x => x.ToUpper() == "ANY"))
            {
                // bind to all IPs
                ipAddresses.Add(IPAddress.Any);
            }
            else
            {
                // bind to selected IPs
                foreach (string s in ss)
                {
                    IPAddress i = (IPAddress)null;
                    if (true == IPAddress.TryParse(s, out i))
                        ipAddresses.Add(i);
                }
            }

            return ipAddresses;
        }

        #endregion
    }
}
