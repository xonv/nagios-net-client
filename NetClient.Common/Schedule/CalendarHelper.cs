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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Nagios.Net.Client.Common.Schedule
{
    /// <summary>
    /// Holds helper methods for date and time operations.
    /// </summary>
    public static class CalendarHelper
    {
        internal const int DaysInWeek = 7;

        /// <summary>
        /// Gets the days of week start with first day of week.
        /// </summary>
        /// <param name="firstDayOfWeek">The first day of week.</param>
        /// <returns> Ordered <see cref="IEnumerable{DayOfWeek}"/>.</returns>
        public static IEnumerable<DayOfWeek> GetDaysOfWeekStartWithFirstDayOfWeek(DayOfWeek firstDayOfWeek)
        {
            var linkedList = new LinkedList<DayOfWeek>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                linkedList.AddLast(dayOfWeek);
            }
            LinkedListNode<DayOfWeek> dayOfWeekNode = linkedList.Find(firstDayOfWeek);

            Debug.Assert(dayOfWeekNode != null, "Day of week haven't be null.");

            for (int i = 0; i < DaysInWeek; i++)
            {
                yield return dayOfWeekNode.Value;
                dayOfWeekNode = dayOfWeekNode.Next ?? linkedList.First;
                if (dayOfWeekNode.Value == firstDayOfWeek)
                    yield break;
            }
        }

        /// <summary>
        /// Gets the last date end of the specified month and year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>The last date of the specified month.</returns>
        public static DateTime GetEndOfMonth(int year, int month)
        {
            return new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59);
        }

        /// <summary>
        /// Gets the last date of month for the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The last date of the specified month.</returns>
        public static DateTime GetEndOfMonth(DateTime date)
        {
            return GetEndOfMonth(date.Year, date.Month);
        }

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="weekStart">The week start.</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(DateTime dateTime, DayOfWeek weekStart)
        {
            var selectedDay = (int)dateTime.DayOfWeek;
            if (selectedDay < (int)weekStart)
            {
                selectedDay += DaysInWeek;
            }

            int daysToSubtract = selectedDay - (int)weekStart;
            DateTime result = dateTime.Subtract(TimeSpan.FromDays(daysToSubtract));
            return result;
        }

        /// <summary>
        /// Gets the last day of week.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="weekStart">The week start.</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(DateTime dateTime, DayOfWeek weekStart)
        {
            DateTime firstDay = GetFirstDayOfWeek(dateTime, weekStart);
            return firstDay.AddDays(DaysInWeek).AddSeconds(-1);
        }

        /// <summary>
        /// Gets the names of days.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>Dictionary of <see cref="DayOfWeek"/>, NameOfDay pair.</returns>
        public static IDictionary<DayOfWeek, string> GetNamesOfDays(CultureInfo culture)
        {
            var dayNames = new Dictionary<DayOfWeek, string>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                dayNames.Add(dayOfWeek, culture.DateTimeFormat.GetDayName(dayOfWeek));
            }

            return dayNames;
        }

        /// <summary>
        /// Gets the names of months.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>Dictionary of NumberOfMonth, NameOfMonth pair.</returns>
        public static IDictionary<int, string> GetNamesOfMonths(CultureInfo culture)
        {
            var monthNames = new Dictionary<int, string>();
            int monthsInYear = culture.Calendar.GetMonthsInYear(DateTime.Today.Year);
            for (int i = 1; i <= monthsInYear; i++)
            {
                monthNames.Add(i, culture.DateTimeFormat.GetMonthName(i));
            }

            return monthNames;
        }

        /// <summary>
        /// Gets the first date of the specified month and year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <returns>The first date of the specified month and year.</returns>
        public static DateTime GetStartOfMonth(int year, int month)
        {
            return new DateTime(year, month, 1, 0, 0, 0);
        }
    }

}
