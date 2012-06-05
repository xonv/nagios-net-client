using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common.Schedule
{
    /// <summary>Specifies the frequency of a recurrence.</summary>
    public enum RecurrenceFrequency
    {
        /// <summary>Indicates no recurrence.</summary>
        None,

        /// <summary>
        /// A frequency of every second.
        /// </summary>
        Secondly,

        /// <summary>
        /// A frequency of every minute.
        /// </summary>
        Minutely,

        /// <summary>
        /// A frequency of every hour.
        /// </summary>
        Hourly,

        /// <summary>
        /// A frequency of every day.
        /// </summary>
        Daily,

        /// <summary>
        /// A frequency of every week.
        /// </summary>
        Weekly,

        /// <summary>
        /// A frequency of every month.
        /// </summary>
        Monthly
    }

}
