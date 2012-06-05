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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nagios.Net.Client.Common.Schedule;
using Nagios.Net.Client.Common;
using System.Globalization;

namespace NetClient.Common.Schedule
{
    /// <summary>
    /// Interaction logic for ScheduleView.xaml
    /// </summary>
    public partial class ScheduleView : UserControl
    {
        List<TimeSpan> _dailyTimes;

        public ScheduleView()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            _dailyTimes = new List<TimeSpan>();
            InitializeComponent();
            this.periodicallyStartSelector.SelectionChanged += (s, e) => { this.periodicallyStart.Text = (string)this.periodicallyStartSelector.SelectedItem; };
            this.periodicallyStopSelector.SelectionChanged += (s, e) => { this.periodicallyStop.Text = (string)this.periodicallyStopSelector.SelectedItem; };
            this.monthlyFST.SelectionChanged += (s, e) => { this.monthlyNum.Text = (string)this.monthlyFST.SelectedItem; };
            this.monthlyDWS.SelectionChanged += (s, e) => { this.monthlyNumType.Text = (string)this.monthlyDWS.SelectedItem; };

            this.timeEditPanel.Visibility = Visibility.Collapsed;
        }


        private RecurrencePattern _Pattern;
        public RecurrencePattern Pattern
        {
            get
            {
                GetValues();
                return _Pattern;
            }
            set
            {
                if (_Pattern != value)
                {
                    _Pattern = value;
                    SetValues();
                }
            }
        }

        private void SetValues()
        {
            if (_Pattern == null)
                return;
            CultureInfo ci = new CultureInfo("en-US");

            // time settings
            this.periodicallyStart.Text = _Pattern.StartTime.ToString("hh\\:mm");
            this.periodicallyStop.Text = _Pattern.StopTime.ToString("hh\\:mm");
            _dailyTimes.Clear();
            _dailyTimes.AddRange(_Pattern.Times);
            this.timesForSchedule.ItemsSource = _dailyTimes;
            this.periodicallyMinutes.Text = _Pattern.DailyInterval.ToString();
            if (_Pattern.Times.Count() == 0)
                this.dailyPeriodically.IsChecked = true;
            else
                this.dailyTimes.IsChecked = true;

            // day/week/month values
            if (_Pattern.Frequency == RecurrenceFrequency.Daily)
            {
                this.dailyPattern.IsChecked = true;
            }
            else if (_Pattern.Frequency == RecurrenceFrequency.Weekly)
            {
                this.weeklyPattern.IsChecked = true;
            }
            else
            {
                this.monthlyPattern.IsChecked = true;
                this.monthlyDays.IsChecked = _Pattern.DayOfMonth > 0;
                this.monthlyNums.IsChecked = _Pattern.DayOfMonth > 0 == false;
            }

            // daily settings
            if (_Pattern.DaysOfWeekMask == RecurrenceDays.WeekDays)
            {
                this.everyWeekDay.IsChecked = true;
            }
            else
            {
                this.everyNdays.IsChecked = true;
                this.daysNumber.Text = _Pattern.Interval.ToString();
            }

            // weekly settings
            this.recurrEveryWeeks.Text = _Pattern.Interval.ToString();
            this.sunday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Sunday) > 0;
            this.monday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Monday) > 0;
            this.tuesday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Tuesday) > 0;
            this.wednesday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Wednesday) > 0;
            this.thursday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Thursday) > 0;
            this.friday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Friday) > 0;
            this.saturday.IsChecked = (_Pattern.DaysOfWeekMask & RecurrenceDays.Saturday) > 0;

            // monthly settings
            this.monthlyDaysNum.Text = _Pattern.Interval.ToString(); ;
            this.monthlyDaysMonths.Text = _Pattern.DayOfMonth.HasValue ? _Pattern.DayOfMonth.ToString() : "1";
            this.monthlyNum.Text = _Pattern.DayOrdinal == 1 ? "first" : (_Pattern.DayOrdinal == 2 ? "second" : (_Pattern.DayOrdinal == 3 ? "third" : (_Pattern.DayOrdinal == 4 ? "fourth" : "last")));

            if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Monday) > 0)
                this.monthlyNumType.Text = "Monday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Tuesday) > 0)
                this.monthlyNumType.Text = "Tuesday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Wednesday) > 0)
                this.monthlyNumType.Text = "Wednesday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Thursday) > 0)
                this.monthlyNumType.Text = "Thursday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Friday) > 0)
                this.monthlyNumType.Text = "Friday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Saturday) > 0)
                this.monthlyNumType.Text = "Saturday";
            else if ((_Pattern.DaysOfWeekMask & RecurrenceDays.Sunday) > 0)
                this.monthlyNumType.Text = "Sunday";

            this.monthlyValue.Text = _Pattern.Interval.ToString();
        }

        private void GetValues()
        {
            _Pattern = new RecurrencePattern();
            CultureInfo ci = new CultureInfo("en-US");

            // time values
            if (this.dailyPeriodically.IsChecked == true)
            {
                TimeSpan tStart = new TimeSpan();
                if (TimeSpan.TryParseExact(this.periodicallyStart.Text, "%h\\:mm", ci, out tStart) == true)
                    _Pattern.StartTime = tStart;

                TimeSpan tStop = new TimeSpan();
                if (TimeSpan.TryParseExact(this.periodicallyStop.Text, "%h\\:mm", ci, out tStop) == true)
                    _Pattern.StopTime = tStop;

                int di = 1;
                int.TryParse(this.periodicallyMinutes.Text, out di); // shoul be checked convertion result for validate the form
                _Pattern.DailyInterval = di;
            }
            else
            {
                _Pattern.Times.AddRange((List<TimeSpan>)timesForSchedule.ItemsSource);
            }

            // day/week/month values

            // daily
            if (this.dailyPattern.IsChecked == true)
            {
                if (this.everyWeekDay.IsChecked == true)
                {
                    _Pattern.Frequency = RecurrenceFrequency.Weekly;
                    _Pattern.DaysOfWeekMask = RecurrenceDays.WeekDays;
                    _Pattern.Interval = 1;
                }
                else
                {
                    int interval = 1;
                    int.TryParse(this.daysNumber.Text, out interval);
                    _Pattern.Interval = interval;
                    _Pattern.DaysOfWeekMask = RecurrenceDays.EveryDay;
                    _Pattern.Frequency = RecurrenceFrequency.Daily;
                }
            }
            // weekly
            else if (this.weeklyPattern.IsChecked == true)
            {
                _Pattern.Frequency = RecurrenceFrequency.Weekly;
                int interval = 1;
                int.TryParse(this.recurrEveryWeeks.Text, out interval);
                _Pattern.Interval = interval;

                RecurrenceDays dayMask = RecurrenceDays.None;
                if (this.sunday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Sunday;
                if (this.monday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Monday;
                if (this.tuesday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Tuesday;
                if (this.wednesday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Wednesday;
                if (this.thursday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Thursday;
                if (this.friday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Friday;
                if (this.saturday.IsChecked == true) dayMask = dayMask | RecurrenceDays.Saturday;

                _Pattern.DaysOfWeekMask = dayMask;

                _Pattern.FirstDayOfWeek = CultureInfo.CurrentUICulture.DateTimeFormat.FirstDayOfWeek;
            }
            else if (this.monthlyPattern.IsChecked == true)
            {
                _Pattern.Frequency = RecurrenceFrequency.Monthly;

                if (this.monthlyDays.IsChecked == true)
                {
                    int interval = 1;
                    int.TryParse(this.monthlyDaysNum.Text, out interval);
                    _Pattern.Interval = interval;

                    int dayOfMonth = 1;
                    int.TryParse(this.monthlyDaysMonths.Text, out dayOfMonth);
                    _Pattern.DayOfMonth = dayOfMonth;
                }
                else if (this.monthlyNums.IsChecked == true)
                {
                    switch (this.monthlyNum.Text)
                    {
                        case "first":
                            _Pattern.DayOrdinal = 1;
                            break;
                        case "second":
                            _Pattern.DayOrdinal = 2;
                            break;
                        case "third":
                            _Pattern.DayOrdinal = 3;
                            break;
                        case "fourth":
                            _Pattern.DayOrdinal = 4;
                            break;
                        case "last":
                            _Pattern.DayOrdinal = 5;
                            break;
                    }

                    if (this.monthlyNumType.Text == "Monday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Monday;
                    else if (this.monthlyNumType.Text == "Tuesday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Tuesday;
                    else if (this.monthlyNumType.Text == "Wednesday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Wednesday;
                    else if (this.monthlyNumType.Text == "Thursday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Thursday;
                    else if (this.monthlyNumType.Text == "Friday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Friday;
                    else if (this.monthlyNumType.Text == "Saturday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Saturday;
                    else if (this.monthlyNumType.Text == "Sunday")
                        _Pattern.DaysOfWeekMask = RecurrenceDays.Sunday;

                    int interval = 1;
                    int.TryParse(this.monthlyValue.Text, out interval);
                    _Pattern.Interval = interval;
                }
            }

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Window w = this.Parent as Window;
            if (w != null)
            {
                w.DialogResult = true;
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window w = this.Parent as Window;
            if (w != null)
            {
                w.DialogResult = false;
            }
        }

        private void btnTimeNew_Click(object sender, RoutedEventArgs e)
        {
            this.editableTime.Text = "";
            BtnTimeDisable();
        }

        private TimeSpan? _oldTimeEdit = null;
        private void btnTimeEdit_Click(object sender, RoutedEventArgs e)
        {
            _oldTimeEdit = (TimeSpan)timesForSchedule.SelectedItem;
            if (_oldTimeEdit.HasValue)
                this.editableTime.Text = _oldTimeEdit.Value.ToString("hh\\:mm");
            BtnTimeDisable();
        }

        private void BtnTimeDisable()
        {
            btnTimeNew.IsEnabled = false;
            btnTimeEdit.IsEnabled = false;
            btnTimeDel.IsEnabled = false;
            timeEditPanel.Visibility = Visibility.Visible;
        }

        private void btnTimeDel_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan delTime = ((TimeSpan)timesForSchedule.SelectedItem);
            timesForSchedule.ItemsSource = ((List<TimeSpan>)timesForSchedule.ItemsSource).Where(x => x != delTime).OrderBy(x => x).ToList();
        }

        private void btnTimeAdd_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = new TimeSpan();
            if (TimeSpan.TryParseExact(this.editableTime.Text, "%h\\:mm", null, out ts) == true)
            {
                List<TimeSpan> vals = (List<TimeSpan>)timesForSchedule.ItemsSource;

                if (_oldTimeEdit.HasValue)
                {
                    vals = ((List<TimeSpan>)timesForSchedule.ItemsSource).Where(x => x != _oldTimeEdit.Value && x != ts).OrderBy(x => x).ToList();
                    _oldTimeEdit = null;
                }

                vals.Add(ts);
                timesForSchedule.ItemsSource = vals.OrderBy(x => x).ToList();
            }
            BtnTimeEnable();
        }

        private void btnTimeCancel_Click(object sender, RoutedEventArgs e)
        {
            BtnTimeEnable();
        }

        private void BtnTimeEnable()
        {
            btnTimeNew.IsEnabled = true;
            btnTimeEdit.IsEnabled = true;
            btnTimeDel.IsEnabled = true;
            timeEditPanel.Visibility = Visibility.Collapsed;
        }
    }
}