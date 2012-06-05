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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace NrpePerfCountersModule.Configurator
{
    [Export(typeof(CounterEditorModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CounterEditorModel : INotifyPropertyChanged
    {
        public CounterEditorModel()
        {

        }

        #region Properties

        #region Categories

        public List<PerformanceCounterCategory> Categories
        {
            get { return PerformanceCounterCategory.GetCategories().OrderBy(x => x.CategoryName).ToList(); }
        }


        private PerformanceCounterCategory _SelectedCategory;
        public PerformanceCounterCategory SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (_SelectedCategory != value)
                {
                    _SelectedCategory = value;
                    RaisePropertyChanged("SelectedCategory");
                    UpdateInstances();
                }
            }
        }


        #endregion

        #region Counters
        private List<string> _Instances;
        public List<string> Instances
        {
            get { return _Instances; }
            set
            {
                if (_Instances != value)
                {
                    _Instances = value;
                    RaisePropertyChanged("Instances");
                }
            }
        }


        private string _SelectedInstance;
        public string SelectedInstance
        {
            get { return _SelectedInstance; }
            set
            {
                if (_SelectedInstance != value)
                {
                    _SelectedInstance = value;
                    RaisePropertyChanged("SelectedInstance");
                    UpdateCounters();
                }
            }
        }

        void UpdateInstances()
        {
            if (_SelectedCategory == null)
                return;
            if (_SelectedCategory.CategoryType == PerformanceCounterCategoryType.MultiInstance)
            {
                Instances = _SelectedCategory.GetInstanceNames().OrderBy(x => x).ToList();
                if (Instances.Count == 0)
                    Instances = new List<string> { "Default" };
            }
            else
                Instances = new List<string> { "Default" };

            SelectedInstance = Instances.FirstOrDefault();
            UpdateCounters();
        }

        void UpdateCounters()
        {
            if (_SelectedCategory == null || string.IsNullOrWhiteSpace(_SelectedInstance) == true)
            {
                _Counters = null;
                SelectedCounter = null;
                return;
            }
            else
            {
                if (_SelectedCategory.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                {
                    _Counters = _SelectedCategory.GetCounters(SelectedInstance != "Default" ? SelectedInstance : "").OrderBy(z => z.CounterName).ToList();
                }
                else
                {
                    _Counters = _SelectedCategory.GetCounters().OrderBy(z => z.CounterName).ToList();
                }
            }
            RaisePropertyChanged("Counters");
        }

        List<PerformanceCounter> _Counters;
        public List<PerformanceCounter> Counters
        {
            get
            {
                return _Counters;
            }
        }

        PerformanceCounter _selectedCounter;
        public PerformanceCounter SelectedCounter
        {
            get
            {
                return _selectedCounter;
            }
            set
            {
                _selectedCounter = value;
                RaisePropertyChanged("SelectedCounter");
                RaisePropertyChanged("CounterHelp");
            }
        }

        public string CounterHelp
        {
            get
            {
                string cntr = _selectedCounter != null ? _selectedCounter.CounterHelp : "";
                return cntr;
            }
        }
        #endregion

        #region Counter Settings


        private int _Duration;
        public int Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration != value)
                {
                    _Duration = value;
                    RaisePropertyChanged("Duration");
                }
            }
        }

        private int _CalcMethod;
        public int CalcMethod
        {
            get { return _CalcMethod; }
            set
            {
                if (_CalcMethod != value)
                {
                    _CalcMethod = value;
                    RaisePropertyChanged("CalcMethod");
                }
            }
        }

        private string _Command;
        public string Command
        {
            get { return _Command; }
            set
            {
                if (_Command != value)
                {
                    _Command = value;
                    RaisePropertyChanged("Command");
                }
            }
        }

        private int _CriticalMethod;
        public int CriticalMethod
        {
            get { return _CriticalMethod; }
            set
            {
                if (_CriticalMethod != value)
                {
                    _CriticalMethod = value;
                    RaisePropertyChanged("CriticalMethod");
                }
            }
        }

        private float _CriticalValue;
        public float CriticalValue
        {
            get { return _CriticalValue; }
            set
            {
                if (_CriticalValue != value)
                {
                    _CriticalValue = value;
                    RaisePropertyChanged("CriticalValue");
                }
            }
        }

        private int _WarningMethod;
        public int WarningMethod
        {
            get { return _WarningMethod; }
            set
            {
                if (_WarningMethod != value)
                {
                    _WarningMethod = value;
                    RaisePropertyChanged("WarningMethod");
                }
            }
        }

        private float _WarningValue;
        public float WarningValue
        {
            get { return _WarningValue; }
            set
            {
                if (_WarningValue != value)
                {
                    _WarningValue = value;
                    RaisePropertyChanged("WarningValue");
                }
            }
        }

        #endregion

        #endregion

        #region Settings Value

        public void SetPerfCounter(PerfCounter val)
        {
            if (string.IsNullOrEmpty(val.Category) == false)
                this.SelectedCategory = Categories.Single(x => x.CategoryName == val.Category);
            if (string.IsNullOrEmpty(val.Instance) == false)
                this.SelectedInstance = val.Instance;
            if (string.IsNullOrEmpty(val.Counter) == false)
                this.SelectedCounter = Counters.Single(x => x.CounterName == val.Counter);
            this.Duration = val.Duration;
            this.CalcMethod = val.CalcMethod;
            this.Command = val.Command;
            this.CriticalMethod = val.CriticalMethod;
            this.CriticalValue = val.CriticalValue;
            this.WarningMethod = val.WarningMethod;
            this.WarningValue = val.WarningValue;
        }

        public PerfCounter GetPerfCounter()
        {
            PerfCounter pc = new PerfCounter
            {
                Duration = this.Duration,
                CalcMethod = this.CalcMethod,
                Command = this.Command,
                CriticalMethod = this.CriticalMethod,
                CriticalValue = this.CriticalValue,
                WarningMethod = this.WarningMethod,
                WarningValue = this.WarningValue,
                Category = SelectedCounter != null ? SelectedCounter.CategoryName : "",
                Instance = SelectedCounter != null ? SelectedCounter.InstanceName : "",
                Counter = SelectedCounter != null ? SelectedCounter.CounterName : ""
            };
            return pc;
        }

        #endregion

        #region INotifyPropertyChanged

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

        #endregion
    }

    public struct CounterInstance
    {
        public string Instance;
        public PerformanceCounter Counter;
    }
}
