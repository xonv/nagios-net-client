using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace NscaLogParserModule.Configurator
{
    [Export(typeof(TextLogEditorModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TextLogEditorModel : INotifyPropertyChanged
    {
        public TextLogEditorModel()
        {
        }

        #region Properties

        // Log Parser settings
        string _Folder;
        public string Folder
        {
            get { return _Folder; }
            set
            {
                _Folder = value;
                RaisePropertyChanged("Folder");
            }
        }

        string _FileTemplate;
        public string FileTemplate
        {
            get { return _FileTemplate; }
            set
            {
                _FileTemplate = value;
                RaisePropertyChanged("FileTemplate");
            }
        }

        string _MessageTemplate;
        public string MessageTemplate
        {
            get { return _MessageTemplate; }
            set
            {
                _MessageTemplate = value;
                RaisePropertyChanged("MessageTemplate");
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

        public void SetValues(LogFile val)
        {
            this.Folder = val.Folder;
            this.FileTemplate = val.FileTemplate;
            this.MessageTemplate = val.MessageTemplate;

            this.NagiosServiceName = val.NagiosServiceName;
            this.Level = val.MessageLevel;
            this.NagiosServiceDescription = val.NagiosServiceDescription;

            this.IsLevelCritical = val.MessageLevel == Nagios.Net.Client.Nsca.Level.Critical;
            this.IsLevelWarning = val.MessageLevel == Nagios.Net.Client.Nsca.Level.Warning;
            this.IsLevelOk = val.MessageLevel == Nagios.Net.Client.Nsca.Level.OK;
        }

        public LogFile GetValues()
        {
            LogFile ed = new LogFile();
            ed.Folder = this.Folder;
            ed.FileTemplate = this.FileTemplate;
            ed.MessageTemplate = this.MessageTemplate;

            ed.NagiosServiceName = this.NagiosServiceName;
            ed.MessageLevel = this.Level;
            ed.NagiosServiceDescription = this.NagiosServiceDescription;
            ed.MessageLevel = this.IsLevelCritical ? Nagios.Net.Client.Nsca.Level.Critical : (this.IsLevelWarning ? Nagios.Net.Client.Nsca.Level.Warning : Nagios.Net.Client.Nsca.Level.OK);
            return ed;
        }

        #endregion Settings Values


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
}
