using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views.Enums;
using System;

namespace faceitwpf.ViewModels
{
    class SearchViewModel : BaseViewModel
    {
        private readonly IStatsRepository statsRepository;
        private readonly IUpdateService updateService;
        private readonly INavigationService navigationService;
        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdateAvailable = false;
        public bool IsUpdateAvailable
        {
            get { return _isUpdateAvailable; }
            set
            {
                _isUpdateAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpdating;
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                _isUpdating = value;
                OnPropertyChanged();
            }
        }

        private string _updateProgress = "0";
        public string UpdateProgress
        {
            get { return _updateProgress; }
            set
            {
                _updateProgress = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool? _isTextboxFocused;
        public bool? IsTextboxFocused
        {
            get { return _isTextboxFocused; }
            set
            {
                _isTextboxFocused = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get => _searchCommand ?? (_searchCommand = new RelayCommand(async (obj) =>
            {
                if (PlayerName.Length > 0 && PlayerName.Length < 30)
                {
                    IsLoading = true;
                    try
                    {
                        await statsRepository.TryToLoadStatsAsync(PlayerName);
                        Properties.Settings.Default.LastNickname = PlayerName;
                        Properties.Settings.Default.Save();
                        navigationService.Navigate(ViewTypes.Data);
                    }
                    catch (Exception ex)
                    {
                        IsLoading = false;
                        PlayerName = ex.Message;
                        IsTextboxFocused = true;
                    }
                    IsLoading = false;
                }
            }));
        }

        private RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get => _updateCommand ?? (_updateCommand = new RelayCommand(async (obj) =>
            {
                try
                {
                    IsUpdating = true;
                    await updateService.UpdateAsync((string percentage) => UpdateProgress = percentage);
                }
                catch (Exception ex)
                {
                    IsUpdating = false;
                    PlayerName = ex.Message;
                }
                finally
                {
                    IsUpdating = false;
                }
            }));
        }

        public SearchViewModel(IStatsRepository statsRepository, IUpdateService updateService, INavigationService navigationService, object parameter)
        {
            this.statsRepository = statsRepository;
            this.updateService = updateService;
            this.navigationService = navigationService;
            PlayerName = Properties.Settings.Default.LastNickname;
            IsTextboxFocused = true;
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            try
            {
                IsUpdateAvailable = await updateService.CheckForUpdate();
            }
            catch { }
        }
    }
}
