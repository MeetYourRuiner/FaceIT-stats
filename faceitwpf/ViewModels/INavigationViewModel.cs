using faceitwpf.Views.Enums;
using System;
using System.Collections.Generic;

namespace faceitwpf.ViewModels
{
    interface INavigationViewModel
    {
        BaseViewModel CurrentViewModel { get; set; }
        Stack<BaseViewModel> History { get; }
        void Navigate(ViewTypes destination, object parameter);
        void GoBack(Exception exception);
        void ClearHistory();
    }
}
