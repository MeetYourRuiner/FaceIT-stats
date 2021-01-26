using faceitwpf.Views.Enums;
using System;

namespace faceitwpf.Services
{
    interface INavigator
    {
        void Navigate(ViewTypes destination, object parameter = null, bool replace = false);
        void ClearHistory();
        void DisplayError(Exception exception);
        void GoBack(Exception ex = null);
    }
}
