﻿using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using FaceitStats.WPF.Views.Enums;
using System;

namespace FaceitStats.WPF.ViewModels
{
    class SearchViewModel : BaseViewModel
    {
        private readonly IUpdateService _updateService;
        private readonly INotifyService _notifyService;
        private readonly INavigator _navigator;
        private bool _isLoaded = false;

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
            get => _searchCommand ??= new RelayCommand((obj) =>
            {
                if (obj != null)
                {
                    string playerName = (string)obj;
                    _navigator.Navigate(ViewTypes.Data, playerName);
                }
                else if (PlayerName.Length > 0 && PlayerName.Length < 30)
                {
                    SettingsWrapper.LastNickname.Set(PlayerName);
                    _navigator.Navigate(ViewTypes.Data, PlayerName);
                }
            });
        }

        private RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get => _updateCommand ??= new RelayCommand(async (obj) =>
            {
                try
                {
                    IsUpdating = true;
                    await _updateService.UpdateAsync((string percentage) => UpdateProgress = percentage);
                }
                catch (Exception ex)
                {
                    IsUpdating = false;
                    _notifyService.DisplayError(ex);
                }
                finally
                {
                    IsUpdating = false;
                }
            });
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ??= new RelayCommand((obj) =>
            {
                Favorites = SettingsWrapper.Favorites.ListAll();

                if (_isLoaded)
                    return;

                PlayerName = SettingsWrapper.LastNickname.Get();
                IsTextboxFocused = true;
                _isLoaded = true;

                CheckForUpdate();
            });
        }

        private RelayCommand _openFavoritesCommand;
        public RelayCommand OpenFavoritesCommand
        {
            get => _openFavoritesCommand ??= new RelayCommand((obj) =>
            {
                IsFavoritesOpen = true;
            });
        }

        private RelayCommand _closeFavoritesCommand;
        public RelayCommand CloseFavoritesCommand
        {
            get => _closeFavoritesCommand ??= new RelayCommand((obj) =>
            {
                IsFavoritesOpen = false;
            });
        }

        private RelayCommand _removeFromFavoritesCommand;
        public RelayCommand RemoveFromFavoritesCommand
        {
            get => _removeFromFavoritesCommand ??= new RelayCommand((obj) =>
            {
                string playerName = (string)obj;
                SettingsWrapper.Favorites.Remove(playerName);
                Favorites = SettingsWrapper.Favorites.ListAll();
            });
        }

        public SearchViewModel(IUpdateService updateService, INavigator navigator, INotifyService notifyService)
        {
            _updateService = updateService;
            _navigator = navigator;
            _notifyService = notifyService;
        }

        private async void CheckForUpdate()
        {
            try
            {
                IsUpdateAvailable = await _updateService.CheckForUpdate();
            }
            catch { }
        }
    }
}
