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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace NscaEventLogModule.Configurator
{
    [Export(typeof(EventLogEditorModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EventLogEditorModel : INotifyPropertyChanged
    {
        public EventLogEditorModel()
        {
            EventLogs = new List<string>();
            SelectedLog = null;
        }

        #region Properties

        private List<string> _EventLogs;
        public List<string> EventLogs
        {
            get { return _EventLogs; }
            set
            {
                _EventLogs = value;
                RaisePropertyChanged("EventLogs");
            }
        }


        private string _SelectedLog;
        public string SelectedLog
        {
            get { return _SelectedLog; }
            set
            {
                _SelectedLog = value;
                RaisePropertyChanged("SelectedLog");
            }
        }


        private bool _IsCritical;
        public bool IsCritical
        {
            get { return _IsCritical; }
            set
            {
                if (_IsCritical != value)
                {
                    _IsCritical = value;
                    RaisePropertyChanged("IsCritical");
                }
            }
        }

        private bool _IsWarning;
        public bool IsWarning
        {
            get { return _IsWarning; }
            set
            {
                if (_IsWarning != value)
                {
                    _IsWarning = value;
                    RaisePropertyChanged("IsWarning");
                }
            }
        }

        private bool _IsVerbose;
        public bool IsVerbose
        {
            get { return _IsVerbose; }
            set
            {
                if (_IsVerbose != value)
                {
                    _IsVerbose = value;
                    RaisePropertyChanged("IsVerbose");
                }
            }
        }

        private bool _IsInformation;
        public bool IsInformation
        {
            get { return _IsInformation; }
            set
            {
                if (_IsInformation != value)
                {
                    _IsInformation = value;
                    RaisePropertyChanged("IsInformation");
                }
            }
        }

        private bool _IsError;
        public bool IsError
        {
            get { return _IsError; }
            set
            {
                if (_IsError != value)
                {
                    _IsError = value;
                    RaisePropertyChanged("IsError");
                }
            }
        }


        EventLogSources _LogSources;
        public EventLogSources LogSources
        {
            get { return _LogSources; }
            set
            {
                _LogSources = value;
                SetCheckedLogSources();
                RaisePropertyChanged("LogSources");
            }
        }

        bool _setCheckedLogSources = false;
        private void SetCheckedLogSources()
        {
            if (_setCheckedLogSources == true)
                return;
            _setCheckedLogSources = true;
            if (_LogSources != null && string.IsNullOrWhiteSpace(SelectedLogSources) == false)
            {
                string sl = SelectedLogSources;
                _LogSources.ForEach(x => x.IsSelected = (sl.Contains(x.Title)));
            }
            _setCheckedLogSources = false;
        }


        private string _SelectedLogSources;
        public string SelectedLogSources
        {
            get { return _SelectedLogSources; }
            set
            {
                _SelectedLogSources = value;
                SetCheckedLogSources();
                RaisePropertyChanged("SelectedLogSources");
            }
        }


        private string _EventIds;
        public string EventIds
        {
            get { return _EventIds; }
            set
            {
                if (_EventIds != value)
                {
                    _EventIds = value;
                    RaisePropertyChanged("EventIds");
                }
            }
        }

        private string _Keywords;
        public string Keywords
        {
            get { return _Keywords; }
            set
            {
                if (_Keywords != value)
                {
                    _Keywords = value;
                    RaisePropertyChanged("Keywords");
                }
            }
        }

        // Nagios settings

        private string _NagiosService;
        public string NagiosServiceName
        {
            get { return _NagiosService; }
            set
            {
                if (_NagiosService != value)
                {
                    _NagiosService = value;
                    RaisePropertyChanged("NagiosServiceName");
                }
            }
        }

        private string _NagiosServiceDescription;
        public string NagiosServiceDescription
        {
            get { return _NagiosServiceDescription; }
            set
            {
                if (_NagiosServiceDescription != value)
                {
                    _NagiosServiceDescription = value;
                    RaisePropertyChanged("NagiosServiceDescription");
                }
            }
        }

        private Nagios.Net.Client.Nsca.Level _Level;
        public Nagios.Net.Client.Nsca.Level Level
        {
            get { return _Level; }
            set
            {
                if (_Level != value)
                {
                    _Level = value;
                    RaisePropertyChanged("Level");
                }
            }
        }


        private bool _IsLevelCritical;
        public bool IsLevelCritical
        {
            get { return _IsLevelCritical; }
            set
            {
                if (_IsLevelCritical != value)
                {
                    _IsLevelCritical = value;
                    RaisePropertyChanged("IsLevelCritical");
                }
            }
        }

        private bool _IsLevelWarning;
        public bool IsLevelWarning
        {
            get { return _IsLevelWarning; }
            set
            {
                if (_IsLevelWarning != value)
                {
                    _IsLevelWarning = value;
                    RaisePropertyChanged("IsLevelWarning");
                }
            }
        }

        private bool _IsLevelOk;
        public bool IsLevelOk
        {
            get { return _IsLevelOk; }
            set
            {
                if (_IsLevelOk != value)
                {
                    _IsLevelOk = value;
                    RaisePropertyChanged("IsLevelOk");
                }
            }
        }


        #endregion Properties

        #region Settings Values

        public void SetEventLog(EventLogDescription val)
        {
            SelectedLog = val.EventLogName;

            this.SelectedLogSources = val.LogSources;
            this.Keywords = val.Keywords;
            this.EventIds = val.EventIds;

            this.IsCritical = val.IsCritical;
            this.IsError = val.IsError;
            this.IsWarning = val.IsWarning;
            this.IsInformation = val.IsInformation;
            this.IsVerbose = val.IsVerbose;

            this.NagiosServiceName = val.NagiosServiceName;
            this.Level = val.MessageLevel;
            this.NagiosServiceDescription = val.NagiosServiceDescription;

            this.IsLevelCritical = val.MessageLevel == Nagios.Net.Client.Nsca.Level.Critical;
            this.IsLevelWarning = val.MessageLevel == Nagios.Net.Client.Nsca.Level.Warning;
            this.IsLevelOk = val.MessageLevel == Nagios.Net.Client.Nsca.Level.OK;
        }

        public EventLogDescription GetEventLog()
        {
            EventLogDescription ed = new EventLogDescription();
            ed.EventLogName = this.SelectedLog;
            ed.LogSources = this.SelectedLogSources;
            ed.Keywords = this.Keywords;
            ed.EventIds = this.EventIds;

            ed.IsCritical = this.IsCritical;
            ed.IsError = this.IsError;
            ed.IsWarning = this.IsWarning;
            ed.IsInformation = this.IsInformation;
            ed.IsVerbose = this.IsVerbose;

            ed.NagiosServiceName = this.NagiosServiceName;
            ed.MessageLevel = this.Level;
            ed.NagiosServiceDescription = this.NagiosServiceDescription;
            ed.MessageLevel = this.IsLevelCritical ? Nagios.Net.Client.Nsca.Level.Critical : (this.IsLevelWarning ? Nagios.Net.Client.Nsca.Level.Warning : Nagios.Net.Client.Nsca.Level.OK);
            return ed;
        }

        #endregion Settings Values

        #region System Event Logs

        private const string WetLogName = "WETChannels";

        public void InitListOfLogs()
        {
            System.Diagnostics.Eventing.Reader.EventLogSession session = new System.Diagnostics.Eventing.Reader.EventLogSession();
            EventLogs = session.GetLogNames().ToList(); // the list of channels

            EventLogSources es = new EventLogSources();
            es.AddRange(session.GetProviderNames().Select(x => new EventLogSourceItem { Title = x, IsSelected = false }).OrderBy(x => x.Title));
            LogSources = es;
        }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }

    public class EventLogSources : List<EventLogSourceItem>
    {
        public EventLogSources() : base() { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            this.Where(x => x.IsSelected == true).ToList().ForEach(x => { if (sb.Length > 0) sb.Append(", "); sb.Append(x.Title); });

            return sb.ToString();
        }
    }

    public class EventLogSourceItem : INotifyPropertyChanged
    {
        public EventLogSourceItem()
        {
            Title = string.Empty;
            IsSelected = false;
        }

        public EventLogSourceItem(string logSrc, bool selected)
        {
            Title = logSrc;
            IsSelected = selected;
        }


        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }


        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }


        /// <summary>
        /// Event raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}