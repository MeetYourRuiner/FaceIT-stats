﻿using faceitwpf.ViewModels;
using System;
using System.Windows.Controls;

namespace faceitwpf.Views
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            DataContext = new SearchPageViewModel(this, new Action(SetFocusOnTextbox));
        }

        private void nameTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Dispatcher.BeginInvoke(new Action(() => textbox.SelectAll()));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetFocusOnTextbox();
        }

        private void SetFocusOnTextbox()
        {
            nameTextBox.Focus();
        }
    }
}
