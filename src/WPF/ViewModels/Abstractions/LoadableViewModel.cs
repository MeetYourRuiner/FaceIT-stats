using FaceitStats.WPF.ViewModels.Commands;
using System.Threading.Tasks;

namespace FaceitStats.WPF.ViewModels.Abstractions
{
    public abstract class LoadableViewModel : BaseViewModel
    {
        public bool isLoaded { get; set; }

        public bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        protected LoadedCommand _loadedCommand;
        public LoadedCommand LoadedCommand
        {
            get => _loadedCommand ??= new LoadedCommand(async (obj) =>
            {
                await LoadMethod(obj);
            }, this);
        }

        public abstract Task LoadMethod(object obj);
    }
}
