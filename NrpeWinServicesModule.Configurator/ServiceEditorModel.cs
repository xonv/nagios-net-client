using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceProcess;
using System.Management;
using Nagios.Net.Client;

namespace NscaWinServicesModule.Configurator
{
    [Export(typeof(ServiceEditorModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ServiceEditorModel : INotifyPropertyChanged
    {
        public ServiceEditorModel() 
        {
            Services = ServiceController.GetServices().OrderBy(x=>x.ServiceName).ToList();
        }

        #region Properties

        private List<ServiceController> _Services;
        public List<ServiceController> Services
        {
            get { return _Services; }
            set
            {
                if (_Services != value)
                {
                    _Services = value;
                    RaisePropertyChanged("Services");
                }
            }
        }


        private ServiceController _SelectedService;
        public ServiceController SelectedService
        {
            get { return _SelectedService; }
            set
            {
                if (_SelectedService != value)
                {
                    _SelectedService = value;
                    RaisePropertyChanged("SelectedService");

                    if (_SelectedService != null)
                    {
                        DisplayName = _SelectedService.DisplayName;
                        ServiceName = _SelectedService.ServiceName;
                        Status = _SelectedService.Status.ToString();
                    }
                }
            }
        }

        private string _ServiceName;
        public string ServiceName
        {
            get { return _ServiceName; }
            set
            {
                if (_ServiceName != value)
                {
                    _ServiceName = value;
                    RaisePropertyChanged("ServiceName");
                }
            }
        }

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                if (_DisplayName != value)
                {
                    _DisplayName = value;
                    RaisePropertyChanged("DisplayName");
                }
            }
        }

        private string _Status;
        public string Status
        {
            get
            {

                return _Status;
            }
            set
            {
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }

        private ServiceControllerStatus _CheckStatus;
        public ServiceControllerStatus CheckStatus
        {
            get { return _CheckStatus; }
            set
            {
                if (_CheckStatus != value)
                {
                    _CheckStatus = value;
                    RaisePropertyChanged("CheckStatus");
                }
            }
        }

        private ServiceControllerStatus _PendingStatus;
        public ServiceControllerStatus PendingStatus
        {
            get { return _PendingStatus; }
            set
            {
                if (_PendingStatus != value)
                {
                    _PendingStatus = value;
                    RaisePropertyChanged("PendingStatus");
                }
            }
        }


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

        private string _NagiosServiceName;
        public string NagiosServiceName
        {
            get { return _NagiosServiceName; }
            set
            {
                if (_NagiosServiceName != value)
                {
                    _NagiosServiceName = value;
                    RaisePropertyChanged("NagiosServiceName");
                }
            }
        }


        #endregion

        #region Settings Value

        public void SetService(ServiceDescription val)
        {
            Duration = val.Duration;
            CheckStatus = val.CheckedStatus;
            PendingStatus = val.PendingStatus;
            DisplayName = val.DisplayName;
            NagiosServiceName = val.NagiosServiceName;
            ServiceName = val.ServiceName;

            SetService(val.ServiceName);
        }
        private void SetService(string sName)
        {
            if (Services != null && Services.Any(x=>x.ServiceName == sName))
            {
                ServiceController sc = Services.Single(x => x.ServiceName == sName);
                SelectedService = sc;
                //DisplayName = _SelectedService.DisplayName;
                //ServiceName = _SelectedService.ServiceName;
                //Status = _SelectedService.Status.ToString();
            }
        }

        public ServiceDescription GetService()
        {
            ServiceDescription pc = new ServiceDescription
            {
                Duration = this.Duration,
                CheckedStatus = this.CheckStatus,
                PendingStatus = this.PendingStatus,
                DisplayName = this.DisplayName,
                NagiosServiceName = this.NagiosServiceName,
                ServiceName = this.ServiceName
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
}
