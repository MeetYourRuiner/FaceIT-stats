using faceitwpf.Views.Enums;
using System;

namespace faceitwpf.Services
{
    interface INavigationService
    {
        void Navigate(ViewTypes destination, object parameter = null);
        void ClearHistory();
        void GoBack(Exception ex = null);
    }
}
