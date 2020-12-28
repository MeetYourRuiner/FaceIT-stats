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

        private string updateBtnContent;
        public string UpdateBtnContent
        {
            get { return updateBtnContent; }
            set
            {
                updateBtnContent = value;
                OnPropertyChanged();
            }
        }

        private Visibility updateBtnVisibility = Visibility.Hidden;
        public Visibility UpdateBtnVisibility
        {
            get { return updateBtnVisibility; }
            set
            {
                updateBtnVisibility = value;
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
                        page.NavigationService.Navigate(datapage);
                    }
                    catch (Exception ex)
                    {
                        IsLoading = false;
                        PlayerName = ex.Message;
                        //await nameTextBox.Dispatcher.BeginInvoke(new Action(() => nameTextBox.SelectAll()));
                        //nameTextBox.Focus();
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
                    isUpdating = true;
                    await UpdateService.Update();
                }
                catch (Exception ex)
                {
                    isUpdating = false;
                    PlayerName = ex.Message;
                }
                finally
                {
                    isUpdating = false;
                }
            }));
        }

        public SearchPageViewModel(Page page)
        {
            this.page = page;
            CheckUpdates();
        }

        private async void CheckUpdates()
        {
            string latestVersion = await UpdateService.CheckForUpdate();
            if (latestVersion != null)
            {
                UpdateBtnContent = "Update to\n" + latestVersion;
                UpdateBtnVisibility = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
