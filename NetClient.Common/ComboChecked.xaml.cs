﻿using System;
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

namespace Nagios.Net.Client.Common
{
    /// <summary>
    /// Interaction logic for ComboChecked.xaml
    /// The ItemsSource MUST contain the Title and IsSelected public fields for objects in the items collection
    /// </summary>
    public partial class ComboChecked : UserControl
    {
        public ComboChecked()
        {
            InitializeComponent();
            this.Text = this.DefaultText;
        }

        #region Dependency Properties
        /// <summary> 
        ///Gets or sets a collection used to generate the content of the ComboBox 
        /// </summary> 
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(ComboChecked), new UIPropertyMetadata(null, OnItemsSourcePropertyChanged));

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboChecked source = d as ComboChecked;
            source.SetText();
        }

        /// <summary> 
        ///Gets or sets the text displayed in the ComboBox 
        /// </summary> 
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ComboChecked), new UIPropertyMetadata(string.Empty, OnTextPropertyChanged));

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboChecked source = d as ComboChecked;
            string value = (string)e.NewValue;
            if (string.IsNullOrWhiteSpace(value) == true)
            {
                source.Text = source.DefaultText;
            }
        }

        /// <summary> 
        ///Gets or sets the text displayed in the ComboBox if there are no selected items 
        /// </summary> 
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultText.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty DefaultTextProperty =
             DependencyProperty.Register("DefaultText", typeof(string), typeof(ComboChecked), new UIPropertyMetadata(string.Empty));
        #endregion

        /// <summary> 
        ///Whenever a CheckBox is checked, change the text displayed 
        /// </summary> 
        /// <param name="sender"></param> 
        /// <param name="e"></param> 
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SetText();
        }

        /// <summary> 
        ///Set the text property of this control (bound to the ContentPresenter of the ComboBox) 
        /// </summary> 
        private void SetText()
        {
            this.Text = (this.ItemsSource != null) ?
                this.ItemsSource.ToString() : this.DefaultText;

            // set DefaultText if nothing else selected 
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }
    }
}
