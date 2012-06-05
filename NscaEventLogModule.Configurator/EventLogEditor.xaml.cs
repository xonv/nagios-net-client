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
using System.Windows.Shapes;

namespace NscaEventLogModule.Configurator
{
    /// <summary>
    /// Interaction logic for EventLogEditor.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class EventLogEditor : Window
    {
        public EventLogEditor()
        {
            InitializeComponent();
            this.ViewModel = new EventLogEditorModel();
            this.Loaded += (s, e) => { this.ViewModel.InitListOfLogs(); };
        }

        public EventLogEditorModel ViewModel
        {
            get
            {
                return (EventLogEditorModel)this.DataContext;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData() == true)
                DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool SaveData()
        {
            return true;
        }
    }
}