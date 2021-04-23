using FaceitStats.WPF.Services;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.Views.Enums;
using System;
using System.Collections.Generic;

namespace FaceitStats.WPF.Interfaces
{
    interface INavigator
    {
        event EventHandler<NavigatedEventArgs> Navigated;
        Stack<BaseViewModel> History { get; }
        void Navigate(ViewTypes destination, object parameter = null, bool replace = false);
        void ClearHistory();
        void GoBack(Exception exception = null);
    }
}
