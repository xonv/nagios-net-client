using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Nagios.Net.Client.Common.Schedule
{
    [Serializable]
    public class RecurrencePattern : INotifyPropertyChanged
    {
        internal static readonly int MaxMonthlyDays = 28;
        internal static readonly int MaxWeeklyDays = 7;
        internal static readonly int MaxYearlyDays = 336;

        private RecurrenceFrequency dailyFrequency;
        private int dailyInterval;

        private int? dayOfMonth;
        private int? dayOrdinal;
        private RecurrenceDays daysOfWeekMask;
        private DayOfWeek firstDayOfWeek;
        private RecurrenceFrequency frequency;
        private int interval;
        private int? monthOfYear;

        private TimeSpan startTimeDefault = new TimeSpan(0, 0, 0);
        private TimeSpan stopTimeDefault = new TimeSpan(23, 59, 59);

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurrencePattern"/> class.
        /// </summary>
        public RecurrencePattern()
        {
            this.dayOfMonth = null;
            this.daysOfWeekMask = RecurrenceDays.None;
            this.frequency = RecurrenceFrequency.Daily;
            this.interval = 1;
            this.monthOfYear = null;
            this.dayOrdinal = null;
            this._StartTime = new TimeSpan(0, 0, 0);
            this._StopTime = new TimeSpan(23, 59, 59);
            this._Times = new List<TimeSpan>();
            this.dailyFrequency = RecurrenceFrequency.Minutely;
            this.dailyInterval = 1;
            this.firstDayOfWeek = DayOfWeek.Monday;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurrencePattern"/> class.
        /// </summary>
        public RecurrencePattern(
            TimeSpan startTime,
            TimeSpan stopTime,
            List<TimeSpan> times,
            RecurrenceFrequency dailyFrequency,
            int dailyInterval,
            int? dayOfMonth,
            RecurrenceDays daysOfWeekMask,
            RecurrenceFrequency frequency,
            int interval,
            int? monthOfYear,
            int? dayOrdinal)
            : this()
        {
            this._StartTime = startTime;
            this._StopTime = stopTime;
            this._Times = times;
            this.dailyFrequency = dailyFrequency;
            this.dailyInterval = dailyInterval;
            this.dayOfMonth = dayOfMonth;
            this.daysOfWeekMask = daysOfWeekMask;
            this.frequency = frequency;
            this.interval = interval;
            this.monthOfYear = monthOfYear;
            this.dayOrdinal = dayOrdinal;
        }

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when some of property was changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Properties

        #region Daily Settings
        /// <summary>
        /// Gets or sets the daily start time
        /// </summary>
        /// <value>The time interval. The default value is 0:00:00</value>
        private TimeSpan _StartTime;
        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    _StartTime = value;
                    this.RaisePropertyChanged("StartTime");
                }
            }
        }

        /// <summary>
        /// Gets or sets the daily stop time.
        /// </summary>
        /// <value>The time interval. The default value is 23:59:59</value>
        private TimeSpan _StopTime;
        public TimeSpan StopTime
        {
            get { return _StopTime; }
            set
            {
                if (_StopTime != value)
                {
                    _StopTime = value;
                    RaisePropertyChanged("StopTime");
                }
            }
        }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public RecurrenceFrequency DailyFrequency
        {
            get
            {
                return this.dailyFrequency;
            }
            set
            {
                if (this.dailyFrequency != value)
                {
                    this.dailyFrequency = value;
                    this.RaisePropertyChanged("DailyFrequency");
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public int DailyInterval
        {
            get
            {
                return this.dailyInterval;
            }
            set
            {
                if (this.dailyInterval != value)
                {
                    this.dailyInterval = value;
                    this.RaisePropertyChanged("DailyInterval");
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of times when the schedule should be thrown
        /// </summary>
        private List<TimeSpan> _Times;
        public List<TimeSpan> Times
        {
            get { return _Times; }
            set
            {
                if (_Times != value)
                {
                    _Times = value;
                    RaisePropertyChanged("Times");
                }
            }
        }

        #endregion

        #region Recurrence settings

        /// <summary>
        /// Gets or sets the day of month.
        /// </summary>
        /// <value>The day of month.</value>
        public int? DayOfMonth
        {
            get
            {
                return this.dayOfMonth;
            }
            set
            {
                if (this.dayOfMonth != value)
                {
                    this.dayOfMonth = value;
                    this.RaisePropertyChanged("DayOfMonth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the day ordinal.
        /// </summary>
        /// <value>The day ordinal.</value>
        public int? DayOrdinal
        {
            get
            {
                return this.dayOrdinal;
            }
            set
            {
                if (this.dayOrdinal != value)
                {
                    this.dayOrdinal = value;
                    this.RaisePropertyChanged("WeekOfMonth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the days of week mask.
        /// </summary>
        /// <value>The days of week mask.</value>
        public RecurrenceDays DaysOfWeekMask
        {
            get
            {
                return this.daysOfWeekMask;
            }
            set
            {
                if (this.daysOfWeekMask != value)
                {
                    this.daysOfWeekMask = value;
                    this.RaisePropertyChanged("DaysOfWeekMask");
                }
            }
        }

        /// <summary>Gets or sets the day on which the week starts.</summary>
        /// <value>
        ///     This property is only meaningful when <see cref="RecurrenceFrequency"/> is set
        ///     to <see cref="RecurrenceFrequency.Weekly"/> and <see cref="Interval"/> is greater than 1.
        /// </value>
        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return this.firstDayOfWeek;
            }
            set
            {
                if (this.firstDayOfWeek != value)
                {
                    this.firstDayOfWeek = value;
                    this.RaisePropertyChanged("FirstDayOfWeek");
                }
            }
        }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public RecurrenceFrequency Frequency
        {
            get
            {
                return this.frequency;
            }
            set
            {
                if (this.frequency != value)
                {
                    this.frequency = value;
                    this.RaisePropertyChanged("Frequency");
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public int Interval
        {
            get
            {
                return this.interval;
            }
            set
            {
                if (this.interval != value)
                {
                    this.interval = value;
                    this.RaisePropertyChanged("Interval");
                }
            }
        }
        #endregion

        #endregion

        private static RecurrenceType GetTypeFromRule(RecurrencePattern pattern)
        {
            switch (pattern.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return RecurrenceType.Daily;
                case RecurrenceFrequency.Weekly:
                    if (pattern.Interval == 1 && pattern.DaysOfWeekMask == RecurrenceDays.WeekDays)
                    {
                        return RecurrenceType.WeekDays;
                    }
                    return RecurrenceType.Weekly;
                case RecurrenceFrequency.Monthly:
                    if (pattern.DayOfMonth > 0)
                    {
                        return RecurrenceType.Monthly;
                    }
                    return RecurrenceType.MonthlyNth;
            }

            return RecurrenceType.Weekly;
        }

        #region Factory Methods

        public static RecurrencePattern Load(string data)
        {
            if (string.IsNullOrWhiteSpace(data) == true)
                throw new ArgumentException("The RecurrencePattern data is not set.");

            return Deserialize(data);
        }

        #endregion

        #region Serialization
        protected static DataContractJsonSerializer GetSerializer()
        {
            return new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(RecurrencePattern));
        }

        protected static RecurrencePattern Deserialize(string data)
        {
            RecurrencePattern o = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            try
            {
                DataContractJsonSerializer ser = GetSerializer();
                o = ser.ReadObject(ms) as RecurrencePattern;
            }
            catch (Exception e)
            {
                throw new RecurrencePatternLoadException(string.Format("Error thrown then read RecurrencePattern: {0}", e.Message));
            }
            finally
            {
                ms.Close();
            }
            return o;
        }

        protected string Serialize()
        {
            MemoryStream ms = new MemoryStream();
            GetSerializer().WriteObject(ms, this);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public string GetSerialized()
        {
            return Serialize();
        }

        #endregion

        /// <summary>
        /// 	<b>Deep</b> copies this instance.
        /// </summary>
        /// <returns>
        /// A <b>deep</b> copy of the current object.
        /// </returns>
        public RecurrencePattern Copy()
        {
            var rule = new RecurrencePattern();
            rule.CopyFrom(this);
            return rule;
        }

        /// <summary>
        /// Copies from.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <exception cref="ArgumentException">Invalid type.</exception>
        public void CopyFrom(RecurrencePattern other)
        {
            if (this.GetType().FullName != other.GetType().FullName)
            {
                throw new ArgumentException("Invalid type");
            }

            this.StartTime = other.StartTime;
            this.StopTime = other.StopTime;
            this.Times = other.Times;
            this.DailyFrequency = other.DailyFrequency;
            this.DailyInterval = other.DailyInterval;
            this.Frequency = other.Frequency;
            this.Interval = other.Interval;
            this.DayOrdinal = other.DayOrdinal;
            this.DayOfMonth = other.DayOfMonth;
            this.DaysOfWeekMask = other.DaysOfWeekMask;
            this.FirstDayOfWeek = other.FirstDayOfWeek;
        }

        /// <summary>
        /// Gets the first occurrence.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public DateTime? GetFirstOccurrence(DateTime start)
        {
            IEnumerator<DateTime> enumerator = this.GetOccurrences(start).GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return null;
        }

        /// <summary>
        /// Gets the first occurrence.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public DateTime? GetFirstOccurrence(DateTime start, TimeSpan interval)
        {
            IEnumerator<DateTime> enumerator = this.GetOccurrences(start, start, start.Add(interval)).GetEnumerator();
            if (enumerator.MoveNext())
            {
                return enumerator.Current;
            }

            return null;
        }

        /// <summary>
        /// Gets the occurrences.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetOccurrences(DateTime start)
        {
            return this.GetOccurrences(start, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// Gets the occurrences.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetOccurrences(DateTime start, DateTime from, DateTime to)
        {
            int candidateIndex = 0;
            int occurrencesCount = 0;

            DateTime endDate = DateTime.MaxValue;

            if (to < endDate)
            {
                endDate = to;
            }
            int maximumOccurrences = int.MaxValue;

            while (occurrencesCount < maximumOccurrences)
            {
                DateTime nextStart = start;
                try
                {
                    nextStart = this.GetNextDate(start, candidateIndex++);
                }
                catch (ArgumentOutOfRangeException)
                {
                    yield break;
                }

                if (endDate < nextStart)
                {
                    yield break;
                }

                if (!this.MatchPattern(start, nextStart))
                {
                    continue;
                }

                occurrencesCount++;

                if (nextStart < from)
                {
                    continue;
                }

                // check times in day
                if (Times.Count > 0)
                {
                    // check in the list of times
                    TimeSpan f = from.TimeOfDay;
                    TimeSpan t = to.TimeOfDay;
                    foreach (TimeSpan ts in Times.Where(x => x >= f && x < t))
                        yield return nextStart.Date.Add(ts);
                }
                else
                {
                    // check in period
                    TimeSpan sTime = this.StartTime > from.TimeOfDay ? this.StartTime : from.TimeOfDay;
                    TimeSpan eTime = this.StopTime > to.TimeOfDay ? to.TimeOfDay : this.StopTime;

                    long r = (long)(sTime.Ticks / new TimeSpan(0, DailyInterval, 0).Ticks);

                    TimeSpan nextTime = new TimeSpan(0, DailyInterval * (int)r, 0);
                    while (nextTime <= eTime)
                    {
                        if (nextTime >= sTime)
                            yield return nextStart.Date.Add(nextTime);

                        nextTime = nextTime.Add(new TimeSpan(0, DailyInterval, 0));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether pattern is valid.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>
        /// 	<c>True</c> if pattern is valid; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsValidPattern(RecurrencePattern pattern)
        {
            if (pattern.Interval <= 0)
            {
                return false;
            }

            switch (pattern.Frequency)
            {
                case RecurrenceFrequency.None:
                    return false;
                case RecurrenceFrequency.Secondly:
                case RecurrenceFrequency.Minutely:
                case RecurrenceFrequency.Hourly:
                    {
                        if (pattern.Interval == 0)
                            return false;
                    }
                    break;
                case RecurrenceFrequency.Daily:
                    {
                        if (pattern.DaysOfWeekMask == RecurrenceDays.None || pattern.DayOfMonth.HasValue ||
                            pattern.DayOrdinal.HasValue)
                        {
                            return false;
                        }
                        break;
                    }
                case RecurrenceFrequency.Weekly:
                    break;
                case RecurrenceFrequency.Monthly:
                    if (pattern.DayOfMonth.HasValue && pattern.DayOfMonth.Value == 0)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        internal int GetMaxDaysDuration()
        {
            switch (this.Frequency)
            {
                case RecurrenceFrequency.None:
                    return 0;
                case RecurrenceFrequency.Secondly:
                case RecurrenceFrequency.Minutely:
                case RecurrenceFrequency.Hourly:
                case RecurrenceFrequency.Daily:
                    return this.Interval;
                case RecurrenceFrequency.Weekly:
                    return this.GetMaxWeeklyDaysDuration();
                case RecurrenceFrequency.Monthly:
                    return this.Interval * MaxMonthlyDays;
                default:
                    return 0;
            }
        }

        internal int GetMaxWeeklyDaysDuration()
        {
            List<DayOfWeek> daysOfWeek = this.DaysOfWeekMask.GetDaysOfWeek().ToList();
            if (daysOfWeek.Count == 0)
            {
                return 0;
            }
            if (daysOfWeek.Count == 1)
            {
                return MaxWeeklyDays * this.Interval;
            }

            int maxWeeklyDays = MaxWeeklyDays;
            for (int i = 1; i < daysOfWeek.Count(); i++)
            {
                int diff = (int)daysOfWeek[i] - (int)daysOfWeek[i - 1];
                if (diff < maxWeeklyDays)
                {
                    maxWeeklyDays = diff;
                }
            }
            if (this.Interval == 1)
            {
                int diffLastFirst = (int)daysOfWeek.First() - (int)daysOfWeek.Last() + MaxWeeklyDays;
                if (diffLastFirst < maxWeeklyDays)
                {
                    maxWeeklyDays = diffLastFirst;
                }
            }

            return maxWeeklyDays;
        }

        /// <summary>
        /// Matches the day of week mask.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="daysOfWeekMask">The days of week mask.</param>
        /// <returns></returns>
        protected static bool MatchDayOfWeekMask(DateTime start, RecurrenceDays daysOfWeekMask)
        {
            if (daysOfWeekMask == RecurrenceDays.None)
            {
                return true;
            }

            return daysOfWeekMask.HasDay(start.DayOfWeek.GetRecurrenceDay());
        }

        /// <summary>
        /// Matches the day ordinal.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="dayOrdinal">The day ordinal.</param>
        /// <param name="daysOfWeekMask">The days of week mask.</param>
        /// <returns></returns>
        protected static bool MatchDayOrdinal(DateTime date, int? dayOrdinal, RecurrenceDays daysOfWeekMask)
        {
            if (!dayOrdinal.HasValue)
            {
                return true;
            }

            return (0 < dayOrdinal)
                       ? MatchDayOrdinalPositive(date, dayOrdinal.Value, daysOfWeekMask)
                       : MatchDayOrdinalNegative(date, dayOrdinal.Value, daysOfWeekMask);
        }

        /// <summary>
        /// Gets the next date when the rule can match.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="index">The index from the start.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid <see cref="RecurrenceFrequency"/>.</exception>
        protected DateTime GetNextDate(DateTime start, int index)
        {
            switch (this.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return start.AddDays(index * this.Interval);
                case RecurrenceFrequency.Weekly:
                    return start.AddDays(index);
                case RecurrenceFrequency.Monthly:
                    return start.AddDays(index);
                default:
                    throw new ArgumentException("Invalid RecurrenceFrequency");
            }
        }

        /// <summary>
        /// Matches the pattern for a date.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="dateTime">The date to match.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Invalid <see cref="RecurrenceFrequency"/>.</exception>
        protected bool MatchPattern(DateTime start, DateTime dateTime)
        {
            switch (this.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return MatchDayOfWeekMask(dateTime, this.DaysOfWeekMask);
                case RecurrenceFrequency.Weekly:
                    {
                        if (this.GetWeekIndex(start, dateTime) % this.Interval != 0)
                        {
                            return false;
                        }

                        if (this.DaysOfWeekMask == RecurrenceDays.None)
                        {
                            return start.DayOfWeek == dateTime.DayOfWeek;
                        }

                        return MatchDayOfWeekMask(dateTime, this.DaysOfWeekMask);
                    }
                case RecurrenceFrequency.Monthly:
                    {
                        if ((GetMonthIndex(start, dateTime) % this.Interval) != 0)
                        {
                            return false;
                        }

                        return this.MatchYearlyPattern(dateTime);
                    }
                default:
                    throw new ArgumentException("Invalid RecurrenceFrequency");
            }
        }



        private static int GetMonthIndex(DateTime start, DateTime dateTime)
        {
            return start.Month - dateTime.Month;
        }

        private static bool MatchDayOrdinalNegative(DateTime date, int dayOrdinal, RecurrenceDays dayOfWeekMask)
        {
            DateTime currentDate = CalendarHelper.GetEndOfMonth(date.Year, date.Month);
            int current = 0;
            while (date < currentDate)
            {
                if (MatchDayOfWeekMask(currentDate, dayOfWeekMask))
                {
                    current--;
                }

                currentDate = currentDate.AddDays(-1);
            }

            return current == dayOrdinal;
        }

        private static bool MatchDayOrdinalPositive(
            DateTime date,
            int dayOrdinal,
            RecurrenceDays dayOfWeekMask)
        {
            DateTime currentDate = CalendarHelper.GetStartOfMonth(date.Year, date.Month);
            int current = 0;
            while (currentDate <= date)
            {
                if (MatchDayOfWeekMask(currentDate, dayOfWeekMask))
                {
                    current++;
                }

                currentDate = currentDate.AddDays(1);
            }

            return current == dayOrdinal;
        }

        private int GetWeekIndex(DateTime start, DateTime current)
        {
            DateTime firstWeekStart = CalendarHelper.GetFirstDayOfWeek(start, this.FirstDayOfWeek);
            TimeSpan fromStart = current.Subtract(firstWeekStart);

            return fromStart.Days / 7;
        }

        private bool MatchYearlyPattern(DateTime dateTime)
        {
            int dayOfMonth1 = this.DayOfMonth ?? 0;
            if (this.DayOfMonth.HasValue && dayOfMonth1 < 0)
            {
                int daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                dayOfMonth1 = daysInMonth + dayOfMonth1 + 1;
                if (dayOfMonth1 <= 0)
                {
                    return false;
                }
            }
            if (dayOfMonth1 > 0 && dateTime.Day != dayOfMonth1)
            {
                return false;
            }

            if (!MatchDayOfWeekMask(dateTime, this.DaysOfWeekMask))
            {
                return false;
            }

            if (!MatchDayOrdinal(dateTime, this.DayOrdinal, this.DaysOfWeekMask))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return the human-friendly textual description of the sschedule pattern
        /// </summary>
        /// <returns></returns>
        public string GetTextualDescription()
        {
            StringBuilder sb = new StringBuilder();
            // daily pattern
            if (Times != null && Times.Count() > 0)
            {
                sb.Append("at ");
                bool fst = false;
                Times.ForEach(x => { sb.Append(string.Format("{0}{1}", fst ? ", " : " ", x.ToString())); fst = true; });
            }
            else
            {
                if (StartTime > startTimeDefault)
                {
                    sb.Append(string.Format("from {0} ", StartTime.ToString()));
                }
                if (StopTime < stopTimeDefault)
                {
                    sb.Append(string.Format("to {0} ", StopTime.ToString()));
                }
                if (DailyInterval > 0 && (DailyFrequency == RecurrenceFrequency.Secondly || DailyFrequency == RecurrenceFrequency.Minutely || DailyFrequency == RecurrenceFrequency.Hourly))
                {
                    sb.Append(string.Format(" every {0} {1}", DailyInterval.ToString(), DailyFrequency == RecurrenceFrequency.Hourly ? "hours" : (DailyFrequency == RecurrenceFrequency.Minutely ? "minutes" : "seconds")));
                }
            }

            // day/week/month pattern

            return sb.ToString();
        }
    }


    public class RecurrencePatternLoadException : Exception
    {
        public RecurrencePatternLoadException() : base() { }
        public RecurrencePatternLoadException(string m) : base(m) { }
    }

}
