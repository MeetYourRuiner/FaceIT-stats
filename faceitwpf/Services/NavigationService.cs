using faceitwpf.ViewModels;
using faceitwpf.Views.Enums;

namespace faceitwpf.Services
{
    class NavigationService : INavigationService
    {
        private readonly INavigationViewModel navigationViewModel;

        public NavigationService(INavigationViewModel navigationViewModel)
        {
            this.navigationViewModel = navigationViewModel;
        }
        public void Navigate(ViewTypes destination, object parameter = null)
        {
            navigationViewModel.ChangeViewModel(destination, parameter);
        }
    }
}
