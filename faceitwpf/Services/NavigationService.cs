using faceitwpf.ViewModels;
using faceitwpf.Views.Enums;
using System;

namespace faceitwpf.Services
{
    class NavigationService : INavigationService
    {
        private readonly INavigationViewModel navigationViewModel;

        public NavigationService(INavigationViewModel navigationViewModel)
        {
            this.navigationViewModel = navigationViewModel;
        }

        public void GoBack(Exception exception = null)
        {
            navigationViewModel.GoBack(exception);
        }

        public void Navigate(ViewTypes destination, object parameter = null)
        {
            navigationViewModel.Navigate(destination, parameter);
        }

        public void ClearHistory()
        {
            navigationViewModel.ClearHistory();
        }
    }
}
