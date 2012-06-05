using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common.Schedule
{
    /// <summary>
    /// Determines the types of recurrent appointments. 
    /// </summary>
    public enum RecurrenceType
    {
        /// <summary>
        /// Recurs every day.
        /// </summary>
        Daily,

        /// <summary>
        /// Recurs every weekday (workday).
        /// </summary>
        WeekDays,

        /// <summary>
        /// Recurs every week.
        /// </summary>
        Weekly,

        /// <summary>
        /// Recurs every month.
        /// </summary>
        Monthly,

        /// <summary>
        /// Recurs every nth day of nth month.
        /// </summary>
        MonthlyNth
    }

}
