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
using System.Windows.Data;
using System.ServiceProcess;

namespace NscaWinServicesModule.Configurator.Formatters
{
    public class ServiceControllerStatusFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                ServiceControllerStatus v = (ServiceControllerStatus)value;

                if (v == ServiceControllerStatus.Paused)
                    return "Paused";
                else if (v == ServiceControllerStatus.Running)
                    return "Running";
                else if (v == ServiceControllerStatus.Stopped)
                    return "Stopped";
            }
            catch { }
            return "Running";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((string)value == "Paused")
                    return ServiceControllerStatus.Paused;
                else if ((string)value == "Running")
                    return ServiceControllerStatus.Running;
                else if ((string)value == "Stopped")
                    return ServiceControllerStatus.Stopped;
            }
            catch { }
            return ServiceControllerStatus.Running;
        }
    }

    public class ServiceControllerPendingStatusFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                ServiceControllerStatus v = (ServiceControllerStatus)value;

                if (v == ServiceControllerStatus.ContinuePending)
                    return "ContinuePending";
                else if (v == ServiceControllerStatus.PausePending)
                    return "PausePending";
                else if (v == ServiceControllerStatus.StartPending)
                    return "StartPending";
                else if (v == ServiceControllerStatus.StopPending)
                    return "StopPending";
            }
            catch { }
            return "StartPending";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if ((string)value == "ContinuePending")
                    return ServiceControllerStatus.ContinuePending;
                else if ((string)value == "PausePending")
                    return ServiceControllerStatus.PausePending;
                else if ((string)value == "StartPending")
                    return ServiceControllerStatus.StartPending;
                else if ((string)value == "StopPending")
                    return ServiceControllerStatus.StopPending;
            }
            catch { }
            return ServiceControllerStatus.StartPending;
        }
    }
}
