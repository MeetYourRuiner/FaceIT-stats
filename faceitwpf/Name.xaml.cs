﻿using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class Name : Page
    {
        public Name()
        {
            InitializeComponent();
        }
        private async void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nameTextBox.Text.Length > 0 && nameTextBox.Text.Length < 30)
            {
                btn.IsEnabled = false;
                nameTextBox.IsEnabled = false;
                Cursor = Cursors.Wait;
                try
                {
                    DataPage datapage = new DataPage();                   
                    API api = API.GetInstance();
                    API.Player player = await api.AsyncGetPlayerInfo(nameTextBox.Text);
                    api.CurrentPlayer = player;
                    API.MatchHistory matchHistory = await api.AsyncGetHistory(player.PlayerID);
                    for (int i = 0; i < matchHistory.Match.Length; i++)
                    {
                        matchHistory.Match[i].Stats = await api.AsyncGetStats(matchHistory.Match[i].Id, player.Nickname);
                        matchHistory.Match[i].Date = DateTimeOffset.FromUnixTimeSeconds(matchHistory.Match[i]._Date).ToLocalTime();
                    }

                    datapage.matchgrid.ItemsSource = matchHistory.Match;
                    datapage.NickLabel.Content = player.Nickname;
                    datapage.LevelLabel.Content = player.Level + " Level " + player.Elo + " Elo";

                    try 
                    { 
                        datapage.avatar.Source = new BitmapImage(new Uri(player.Avatar)); 
                    }
                    catch (System.UriFormatException)
                    {
                        datapage.avatar.Source = new BitmapImage(new Uri("/faceitwpf;component/icon-pheasant-preview-2-268x151.png", UriKind.Relative));
                        datapage.avatar.Stretch = Stretch.Uniform;
                    }                    
                    NavigationService.Navigate(datapage);
                }
                catch(Exception ex)
                {
                    btn.IsEnabled = true;
                    nameTextBox.IsEnabled = true;
                    Cursor = Cursors.Arrow;
                    nameTextBox.Text = ex.Message;
                }
            }
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 125, 0));
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 85, 0));
        }
    }
}
