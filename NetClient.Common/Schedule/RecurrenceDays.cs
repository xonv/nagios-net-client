using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nagios.Net.Client.Common.Schedule
{
    /// <summary>
    /// 	<para>Specifies the days of the week. Members might be combined using bitwise
    ///     operations to specify multiple days.</para>
    /// </summary>
    /// <remarks>
    ///     The constants in the <see cref="RecurrenceDays"/> enumeration might be combined
    ///     with bitwise operations to represent any combination of days. It is designed to be
    ///     used in conjunction with the <see cref="RecurrencePattern"/> class to filter
    ///     the days of the week for which the recurrence pattern applies.
    /// </remarks>
    /// <example>
    /// 	<para>Consider the following example that demonstrates the basic usage pattern of
    ///     RecurrenceDays. The most common operators used for manipulating bit fields
    ///     are:</para>
    /// 	<list type="bullet">
    /// 		<item>Bitwise OR: Turns a flag on.</item>
    /// 		<item>Bitwise XOR: Toggles a flag.</item>
    /// 		<item>Bitwise AND: Checks if a flag is turned on.</item>
    /// 		<item>Bitwise NOT: Turns a flag off.</item>
    /// 	</list>
    /// 	<code lang="CS">
    ///  
    /// namespace RecurrenceExamples
    /// {
    ///     class RecurrenceDayExample
    ///     {
    ///         static void Main()
    ///         {
    ///             // Selects Friday, Saturday and Sunday.
    ///             RecurrenceDays dayMask = RecurrenceDays.Friday | RecurrenceDays.WeekendDays;
    ///             PrintSelectedDays(dayMask);
    ///  
    ///             // Selects all days, except Thursday.
    ///             dayMask = RecurrenceDays.EveryDay ^ RecurrenceDays.Thursday;
    ///             PrintSelectedDays(dayMask);
    ///         }
    ///  
    ///         static void PrintSelectedDays(RecurrenceDays dayMask)
    ///         {
    ///             Console.WriteLine("Value: {0,3} - {1}", (int) dayMask, dayMask);
    ///         }
    ///     }
    /// }
    ///  
    /// /*
    /// This example produces the following results:
    ///  
    /// Value: 112 - Friday, WeekendDays
    /// Value: 119 - Monday, Tuesday, Wednesday, Friday, WeekendDays
    /// */
    ///     </code>
    /// </example>
    [Flags]
    public enum RecurrenceDays
    {
        /// <summary>Indicates no selected day.</summary>
        None = 0,

        /// <summary>Indicates Sunday.</summary>
        Sunday = 1,

        /// <summary>Indicates Monday.</summary>
        Monday = 1 << 1,

        /// <summary>Indicates Tuesday.</summary>
        Tuesday = 1 << 2,

        /// <summary>Indicates Wednesday.</summary>
        Wednesday = 1 << 3,

        /// <summary>Indicates Thursday.</summary>
        Thursday = 1 << 4,

        /// <summary>Indicates Friday.</summary>
        Friday = 1 << 5,

        /// <summary>Indicates Saturday.</summary>
        Saturday = 1 << 6,

        /// <summary><para>Indicates the range from Sunday to Saturday inclusive.</para></summary>
        EveryDay = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday,

        /// <summary>Indicates the range from Monday to Friday inclusive.</summary>
        WeekDays = Monday | Tuesday | Wednesday | Thursday | Friday,

        /// <summary>Indicates the range from Saturday to Sunday inclusive.</summary>
        WeekendDays = Saturday | Sunday
    }


    /// <summary>
    /// <see cref="RecurrenceDays"/> extensions methods.
    /// </summary>
    public static class RecurrenceDaysExtensions
    {
        /// <summary>
        /// Adds the day.
        /// </summary>
        /// <param name="recurrenceDays">The recurrence days.</param>
        /// <param name="dayToAdd">The day to add.</param>
        public static RecurrenceDays AddDay(this RecurrenceDays recurrenceDays, RecurrenceDays dayToAdd)
        {
            return recurrenceDays | dayToAdd;
        }

        /// <summary>
        /// Gets the day of week.
        /// </summary>
        /// <param name="recurrenceDays">The recurrence days.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="recurrenceDays"/></exception>
        public static DayOfWeek GetDayOfWeek(this RecurrenceDays recurrenceDays)
        {
            List<DayOfWeek> daysOfWeek = GetDaysOfWeek(recurrenceDays).ToList();

            if (daysOfWeek.Count != 1)
            {
                throw new ArgumentException("Invalid Recurrence Days parameter - days of week count <> 1", "recurrenceDays");
            }

            return daysOfWeek[0];
        }

        /// <summary>
        /// Gets the days of week.
        /// </summary>
        /// <param name="recurrenceDays">The recurrence days.</param>
        /// <returns></returns>
        public static IEnumerable<DayOfWeek> GetDaysOfWeek(this RecurrenceDays recurrenceDays)
        {
            var dayOfWeeks = new List<DayOfWeek>();
            foreach (DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (recurrenceDays.HasDay(dayOfWeek))
                {
                    dayOfWeeks.Add(dayOfWeek);
                }
            }
            dayOfWeeks.Sort();

            return dayOfWeeks;
        }

        /// <summary>
        /// Gets the recurrence day.
        /// </summary>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>dayOfWeek</c> is out of range.</exception>
        public static RecurrenceDays GetRecurrenceDay(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Friday:
                    return RecurrenceDays.Friday;
                case DayOfWeek.Monday:
                    return RecurrenceDays.Monday;
                case DayOfWeek.Saturday:
                    return RecurrenceDays.Saturday;
                case DayOfWeek.Sunday:
                    return RecurrenceDays.Sunday;
                case DayOfWeek.Thursday:
                    return RecurrenceDays.Thursday;
                case DayOfWeek.Tuesday:
                    return RecurrenceDays.Tuesday;
                case DayOfWeek.Wednesday:
                    return RecurrenceDays.Wednesday;
                default:
                    throw new ArgumentOutOfRangeException("dayOfWeek");
            }
        }

        /// <summary>
        /// Determines whether the specified recurrence days has day.
        /// </summary>
        /// <param name="recurrenceDays">The recurrence days.</param>
        /// <param name="dayToCompare">The day to compare.</param>
        /// <returns>
        /// 	<c>true</c> if the specified recurrence days has day; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDay(this RecurrenceDays recurrenceDays, RecurrenceDays dayToCompare)
        {
            if ((recurrenceDays & dayToCompare) == dayToCompare)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified recurrence days has day.
        /// </summary>
        /// <param name="recurrenceDays">The recurrence days.</param>
        /// <param name="dayToCompare">The day to compare.</param>
        /// <returns>
        /// 	<c>true</c> if the specified recurrence days has day; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDay(this RecurrenceDays recurrenceDays, DayOfWeek dayToCompare)
        {
            RecurrenceDays recurrenceDayToCompare = dayToCompare.GetRecurrenceDay();
            return recurrenceDays.HasDay(recurrenceDayToCompare);
        }
    }

}
