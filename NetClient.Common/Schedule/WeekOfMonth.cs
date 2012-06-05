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
