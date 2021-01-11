using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views.Enums;
using System.Collections.Generic;

namespace faceitwpf.ViewModels
{
    class DataViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly INavigationService navigationService;

        private const int MATCHES_ON_PAGE = 9;

        private int _pagesCount = 1;
        private int _page;
        private int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged("IsPrevEnabled");
                OnPropertyChanged("IsNextEnabled");
            }
        }

        private enum PageTurn
        {
            Next,
            Previous,
            First
        }

        private Player _currentPlayer;
        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                _currentPlayer = value;
                OnPropertyChanged();
            }
        }

        public List<Match> Matches { get; set; }

        private LastMatchesPerfomance _lastMatchesPerfomance;
        public LastMatchesPerfomance LastMatchesPerfomance
        {
            get { return _lastMatchesPerfomance; }
            set
            {
                _lastMatchesPerfomance = value;
                OnPropertyChanged();
            }
        }

        private List<Match> _sliceOfHistory;
        public List<Match> SliceOfHistory
        {
            get { return _sliceOfHistory; }
            set
            {
                _sliceOfHistory = value;
                OnPropertyChanged();
            }
        }

        private bool _isChartVisible = false;
        public bool IsChartVisible
        {
            get { return _isChartVisible; }
            set
            {
                _isChartVisible = value;
                OnPropertyChanged();
            }
        }

        private string _toggleButtonContent;
        public string ToggleButtonContent
        {
            get { return _toggleButtonContent; }
            set
            {
                _toggleButtonContent = value;
                OnPropertyChanged();
            }
        }

        public bool IsNextEnabled
        {
            get { return Page < _pagesCount - 1; }
        }

        public bool IsPrevEnabled
        {
            get { return Page > 0; }
        }


        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get => _backCommand ?? (_backCommand = new RelayCommand((obj) =>
            {
                navigationService.Navigate(ViewTypes.Search);
            }));
        }

        private RelayCommand _nextPageCommand;
        public RelayCommand NextPageCommand
        {
            get => _nextPageCommand ?? (_nextPageCommand = new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(PageTurn.Next);
                }
                catch { }
            }));
        }

        private RelayCommand _prevPageCommand;
        public RelayCommand PrevPageCommand
        {
            get => _prevPageCommand ?? (_prevPageCommand = new RelayCommand((obj) =>
            {
                try
                {
                    SliceOfHistory = GetPage(PageTurn.Previous);
                }
                catch { }
            }));
        }

        private RelayCommand _toggleModeCommand;
        public RelayCommand ToggleModeCommand
        {
            get => _toggleModeCommand ?? (_toggleModeCommand = new RelayCommand((obj) =>
            {
                IsChartVisible = !IsChartVisible;
                ToggleButtonContent = IsChartVisible ? "Table" : "Chart";
            }));
        }

        public DataViewModel(IStatsRepository statsRepository, INavigationService navigationService, object parameter)
        {
            this.statsRepository = statsRepository;
            this.navigationService = navigationService;

            CurrentPlayer = statsRepository.GetCurrentPlayer();
            Matches = statsRepository.GetMatches();
            _pagesCount = CountPages();
            SliceOfHistory = GetPage(PageTurn.First);
            LastMatchesPerfomance = new LastMatchesPerfomance(Matches);
        }

        private List<Match> GetPage(PageTurn pageTurn)
        {
            switch (pageTurn)
            {
                case PageTurn.Next:
                    ++Page;
                    break;
                case PageTurn.Previous:
                    --Page;
                    break;
                case PageTurn.First:
                    Page = 0;
                    break;
                default:
                    break;
            }
            try
            {
                if (Matches.Count == 0)
                {
                    return Matches;
                }
                int restOfMatches = Matches.Count - Page * MATCHES_ON_PAGE;
                if (restOfMatches < MATCHES_ON_PAGE && restOfMatches > 0)
                    return Matches.GetRange(Page * MATCHES_ON_PAGE, restOfMatches);
                else if (restOfMatches > 0)
                    return Matches.GetRange(Page * MATCHES_ON_PAGE, MATCHES_ON_PAGE);
            }
            catch (System.Exception)
            {
                if (pageTurn != PageTurn.First)
                    throw;
            }
            return Matches;
        }

        private int CountPages()
        {
            int pagesCount = Matches.Count / MATCHES_ON_PAGE;
            if (Matches.Count % MATCHES_ON_PAGE > 0)
                ++pagesCount;
            return pagesCount;
        }
    }
}
