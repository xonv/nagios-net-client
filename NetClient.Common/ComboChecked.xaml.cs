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

