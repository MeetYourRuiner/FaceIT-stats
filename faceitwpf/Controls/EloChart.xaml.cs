using faceitwpf.Models;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace faceitwpf.Controls
{
    /// <summary>
    /// Логика взаимодействия для EloChart.xaml
    /// </summary>
    public partial class EloChart : UserControl
    {
        public SeriesCollection MatchSeries { get; set; }
        public Func<Match, string> Formatter { get; set; }

        public EloChart()
        {
            InitializeComponent();
        }

        public void SetSource(Match[] matches)
        {
            var WinBrush = new SolidColorBrush(Colors.Green);
            var LossBrush = new SolidColorBrush(Colors.Red);
            var mapper = Mappers.Xy<Match>() //in this case value is of type <ObservablePoint>
                .X((value, index) => index) //use the X property as X
                .Y((value, index) => value.ELO) //use the Y property as Y
                .Fill(value => value.Result == 'W' ? WinBrush : LossBrush);
            var values = matches.Reverse().Where(m => m.ELO != 0).AsChartValues();
            MatchSeries = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    Title = "ELO",
                    Values = values
                }
            };
            DataContext = this;
            Formatter = value => value.ELO.ToString();
        }
    }
}
