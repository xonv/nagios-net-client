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
