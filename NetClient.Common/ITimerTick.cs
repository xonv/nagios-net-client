using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common
{
    public interface ITimerTick
    {
        void Tick();
    }
}
