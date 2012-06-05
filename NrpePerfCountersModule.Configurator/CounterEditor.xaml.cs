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

namespace NrpePerfCountersModule.Configurator
{
    /// <summary>
    /// Interaction logic for CounterEditor.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CounterEditor : Window
    {
        public CounterEditor()
        {
            InitializeComponent();
            this.ViewModel = new CounterEditorModel();
        }

        public CounterEditorModel ViewModel
        {
            get
            {
                return (CounterEditorModel)this.DataContext;
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
