using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Nagios.Net.Client.Common
{
    public interface IConfigurator
    {
        string Name { get; }
        string Description { get; }
        UserControl ConfigForm { get; }
        void SaveChanges();
    }
}
