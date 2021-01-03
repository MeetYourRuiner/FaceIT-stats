using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using faceitwpf.Models;
using faceitwpf.Services;
using faceitwpf.ViewModels.Commands;
using faceitwpf.Views;

namespace faceitwpf.ViewModels
{
    class SearchPageViewModel : INotifyPropertyChanged
    {
        private readonly Page page;
        private readonly Action setFocusOnTextbox;

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set
            {
                playerName = value;
                OnPropertyChanged();
            }
        }

        private bool isUpdateAvailable = false;
        public bool IsUpdateAvailable
        {
            get { return isUpdateAvailable; }
            set
            {
                isUpdateAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool isUpdating;
        public bool IsUpdating
        {
            get { return isUpdating; }
            set
            {
                isUpdating = value;
                OnPropertyChanged();
            }
        }

        private string updateProgress = "0";
        public string UpdateProgress
        {
            get { return updateProgress; }
            set
            {
                updateProgress = value;
                OnPropertyChanged();
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand searchCommand;
        public RelayCommand SearchCommand
        {
            get => searchCommand ?? (searchCommand = new RelayCommand(async (obj) =>
            {
                if (PlayerName.Length > 0 && PlayerName.Length < 30)
                {
                    IsLoading = true;
                    try
                    {
                        Player player = await APIService.GetPlayerAsync(PlayerName);
                        APIService.CurrentPlayer = player;
                        DataPage datapage = new DataPage();
                        await datapage.Initialize();
                        Properties.Settings.Default.LastNickname = PlayerName;
                        Properties.Settings.Default.Save();
                        page.NavigationService.Navigate(datapage);
                    }
                    catch (Exception ex)
                    {
                        IsLoading = false;
                        PlayerName = ex.Message;
                        setFocusOnTextbox();
                    }
                    IsLoading = false;
                }
            }));
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get => updateCommand ?? (updateCommand = new RelayCommand(async (obj) =>
            {
                try
                {
                    IsUpdating = true;
                    await UpdateService.Update((string percentage) => UpdateProgress = percentage);
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

        public SearchPageViewModel(Page page, Action setFocusOnTextbox)
        {
            this.page = page;
            this.setFocusOnTextbox = setFocusOnTextbox;
            PlayerName = Properties.Settings.Default.LastNickname;
            CheckUpdates();
        }

        private async void CheckUpdates()
        {
            string latestVersion = await UpdateService.CheckForUpdate();
            if (latestVersion != null)
            {
                IsUpdateAvailable = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
