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
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace NscaLogParserModule.Configurator
{
    /// <summary>
    /// Interaction logic for TextLogEditor.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TextLogEditor : Window
    {
        public TextLogEditor()
        {
            InitializeComponent();
        }

        public TextLogEditorModel ViewModel
        {
            get
            {
                return (TextLogEditorModel)this.DataContext;
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

        private void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the directory in which you want to watch log files.";
            folderBrowserDialog.ShowNewFolderButton = false;
            
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ViewModel.Folder = folderBrowserDialog.SelectedPath;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true; 
        }
    }
}
