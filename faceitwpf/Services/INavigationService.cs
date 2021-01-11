using faceitwpf.Views.Enums;

namespace faceitwpf.Services
{
    interface INavigationService
    {
        void Navigate(ViewTypes destination, object parameter = null);
    }
}
