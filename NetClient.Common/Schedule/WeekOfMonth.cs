using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common.Schedule
{
    /// <summary>
    /// Determines the week of month.
    /// </summary>
    public enum WeekOfMonth
    {
        /// <summary>
        /// Indicates no selected week.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates the first week of month.
        /// </summary>
        First = 1,

        /// <summary>
        /// Indicates the second week of month.
        /// </summary>
        Second = 2,

        /// <summary>
        /// Indicates the third week of month.
        /// </summary>
        Third = 3,

        /// <summary>
        /// Indicates the fourth week of month.
        /// </summary>
        Fourth = 4,

        /// <summary>
        /// Indicates the last week of month.
        /// </summary>
        Last = 5
    }

    /// <summary>
    /// <see cref="WeekOfMonth"/> extensions methods.
    /// </summary>
    public static class WeekOfMonthExtensions
    {
        /// <summary>
        /// Convert a value to <see cref="int" />.
        /// </summary>
        /// <param name="weekOfMonth">The week of month.</param>
        /// <returns>Converted to int value.</returns>
        public static int ToInt32(this WeekOfMonth weekOfMonth)
        {
            if (weekOfMonth == WeekOfMonth.Last)
                return -1;

            return (int)weekOfMonth;
        }
    }

}
