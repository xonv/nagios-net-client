using System;
using System.Collections.Generic;
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
using System.ComponentModel.Composition;

namespace NscaWinServicesModule.Configurator
{
    /// <summary>
    /// Interaction logic for ServiceEditor.xaml
    /// </summary>

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ServiceEditor : Window
    {
        public ServiceEditor()
        {
            InitializeComponent();
            this.ViewModel = new ServiceEditorModel();
        }

        public ServiceEditorModel ViewModel
        {
            get
            {
                return (ServiceEditorModel)this.DataContext;
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
