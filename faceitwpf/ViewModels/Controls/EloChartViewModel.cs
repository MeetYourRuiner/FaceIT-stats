using faceitwpf.Models;
using faceitwpf.ViewModels.Commands;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace faceitwpf.ViewModels.Controls
{
    class EloChartViewModel : BaseViewModel
    {
        private readonly List<Match> matches;
        private bool _isLoaded;

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
            get => _loadedCommand ?? (_loadedCommand = new RelayCommand((obj) =>
            {
                if (_isLoaded)
                    return;

                IsLoading = true;
                UpdateSource(matches);
                IsLoading = false;

                _isLoaded = true;
            }));
        }

        public EloChartViewModel(List<Match> matches)
        {
            this.matches = matches;
        }

        private void UpdateSource(List<Match> matches)
        {
            var WinBrush = new SolidColorBrush(Colors.Green);
            var LossBrush = new SolidColorBrush(Colors.Red);
            var mapper = Mappers.Xy<Match>() //in this case value is of type <ObservablePoint>
                .X((value, index) => index) //use the X property as X
                .Y((value, index) => value.ELO) //use the Y property as Y
                .Fill(value => value.Stats.Result == 'W' ? WinBrush : LossBrush);
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
