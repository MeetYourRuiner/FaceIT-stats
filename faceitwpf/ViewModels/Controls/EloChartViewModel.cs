using faceitwpf.Models;
using faceitwpf.ViewModels.Abstractions;
using faceitwpf.ViewModels.Commands;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace faceitwpf.ViewModels.Controls
{
    class EloChartViewModel : LoadableViewModel
    {
        private readonly List<Match> matches;

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


        public override async Task LoadedMethod(object obj)
        {
            UpdateSource(matches);
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
