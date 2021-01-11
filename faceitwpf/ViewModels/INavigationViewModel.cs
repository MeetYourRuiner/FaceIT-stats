using faceitwpf.Views.Enums;

namespace faceitwpf.ViewModels
{
    interface INavigationViewModel
    {
        BaseViewModel CurrentViewModel { get; set; }
        void ChangeViewModel(ViewTypes destination, object parameter);
    }
}
