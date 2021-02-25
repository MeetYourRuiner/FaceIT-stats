using faceitwpf.Services;
using faceitwpf.ViewModels.Abstractions;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views.Enums;
using System;
using System.Collections.Specialized;

namespace faceitwpf.ViewModels
{
    class SearchViewModel : BaseViewModel
    {
        private readonly IUpdateService updateService;
        private readonly INavigator navigator;
        private string _playerName;
        private bool _isLoaded = false;
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        private string[] _favorites;
        public string[] Favorites
        {
            get => _favorites;
            set
            {
                _favorites = value;
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

        private bool _isFavoritesOpen;
        public bool IsFavoritesOpen
        {
            get { return _isFavoritesOpen; }
            set
            {
                _isFavoritesOpen = value;
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
            get => _searchCommand ?? (_searchCommand = new RelayCommand((obj) =>
            {
                if (obj != null)
                {
                    string playerName = (string)obj;
                    navigator.Navigate(ViewTypes.Data, playerName);
                }
                else if (PlayerName.Length > 0 && PlayerName.Length < 30)
                {
                    Properties.Settings.Default.LastNickname = PlayerName;
                    Properties.Settings.Default.Save();
                    navigator.Navigate(ViewTypes.Data, PlayerName);
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
                    navigator.DisplayError(ex);
                }
                finally
                {
                    IsUpdating = false;
                }
            }));
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ?? (_loadedCommand = new RelayCommand((obj) =>
            {
                StringCollection favoritesCollection = Properties.Settings.Default.Favorites;
                string[] favorites = new string[favoritesCollection.Count];
                favoritesCollection.CopyTo(favorites, 0);
                Favorites = favorites;

                if (_isLoaded)
                    return;

                PlayerName = Properties.Settings.Default.LastNickname;

                IsTextboxFocused = true;
                _isLoaded = true;
                navigator.DisplayError(new Exception("Даже не дебажил)0)"));
                CheckForUpdate();
            }));
        }

        private RelayCommand _openFavoritesCommand;
        public RelayCommand OpenFavoritesCommand
        {
            get => _openFavoritesCommand ?? (_openFavoritesCommand = new RelayCommand((obj) =>
            {
                IsFavoritesOpen = true;
            }));
        }

        private RelayCommand _closeFavoritesCommand;
        public RelayCommand CloseFavoritesCommand
        {
            get => _closeFavoritesCommand ?? (_closeFavoritesCommand = new RelayCommand((obj) =>
            {
                IsFavoritesOpen = false;
            }));
        }

        private RelayCommand _removeFromFavoritesCommand;
        public RelayCommand RemoveFromFavoritesCommand
        {
            get => _removeFromFavoritesCommand ?? (_removeFromFavoritesCommand = new RelayCommand((obj) =>
            {
                string playerName = (string)obj;
                StringCollection favoritesCollection = Properties.Settings.Default.Favorites;
                favoritesCollection.Remove(playerName);
                Properties.Settings.Default.Favorites = favoritesCollection;
                Properties.Settings.Default.Save();

                string[] favorites = new string[favoritesCollection.Count];
                favoritesCollection.CopyTo(favorites, 0);
                Favorites = favorites;
            }));
        }

        public SearchViewModel(IUpdateService updateService, INavigator navigator, object parameter)
        {
            this.updateService = updateService;
            this.navigator = navigator;
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
