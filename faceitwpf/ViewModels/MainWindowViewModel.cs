using faceitwpf.Classes;
using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.Views.Enums;

namespace faceitwpf.ViewModels
{
    class MainWindowViewModel : BaseViewModel, INavigationViewModel
    {
        private readonly VMStore _vmStore;
        private readonly IAPIService _apiService;
        private readonly IUpdateService _updateService;
        private readonly INavigationService _navigationService;
        private readonly IStatsRepository _statsRepository;

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            _apiService = new APIService();
            _updateService = new UpdateService();
            _navigationService = new NavigationService(this);
            _statsRepository = new StatsRepository(_apiService);

            _vmStore = new VMStore();
            _vmStore.Add<SearchViewModel>((parameter) => new SearchViewModel(_statsRepository, _updateService, _navigationService, parameter));
            _vmStore.Add<DataViewModel>((parameter) => new DataViewModel(_statsRepository, _navigationService, parameter));

            _navigationService.Navigate(ViewTypes.Search);
        }

        public void ChangeViewModel(ViewTypes destination, object parameter)
        {
            switch (destination)
            {
                case ViewTypes.Data:
                    CurrentViewModel = _vmStore.Get<DataViewModel>(parameter);
                    break;
                case ViewTypes.Search:
                    CurrentViewModel = _vmStore.Get<SearchViewModel>(parameter);
                    break;
                default:
                    break;
            }
        }
    }
}
