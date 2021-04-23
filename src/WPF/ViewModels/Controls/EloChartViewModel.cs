using FaceitStats.Core.Models;
using FaceitStats.WPF.ViewModels.Abstractions;
using FaceitStats.WPF.ViewModels.Commands;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace FaceitStats.WPF.ViewModels.Controls
{
    class EloChartViewModel : BaseViewModel
    {
        private readonly List<Match> _matches;

        private SeriesCollection _matchSeries;
        public SeriesCollection MatchSeries
        {
            get { return _matchSeries; }
            set
            {
                _matchSeries = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get => _loadedCommand ??= new RelayCommand((obj) =>
            {
                UpdateSource(_matches);
            });
        }

        public EloChartViewModel(List<Match> matches)
        {
            _matches = matches;
        }

        private void UpdateSource(List<Match> matches)
        {
            var WinBrush = new SolidColorBrush(Colors.Green);
            var LossBrush = new SolidColorBrush(Colors.Red);
            var mapper = Mappers.Xy<Match>()
                .X((value, index) => index) //use the X property as X
                .Y((value, index) => value.ELO) //use the Y property as Y
                .Fill(value => value.PlayerStats.Result == 'W' ? WinBrush : LossBrush);
            var matchArray = matches.ToArray();
            var values = matchArray.Reverse().Where(m => m.ELO != 0).AsChartValues();
            MatchSeries = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    Title = "ELO",
                    Values = values
                }
            };
        }
    }
}
