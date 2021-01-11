using faceitwpf.Models;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace faceitwpf.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для EloChart.xaml
    /// </summary>
    public partial class EloChart : UserControl
    {
        public SeriesCollection MatchSeries { get; set; }
        public Func<Match, string> Formatter { get; set; }

        public List<Match> History
        {
            get { return (List<Match>)GetValue(HistoryProperty); }
            set { SetValue(HistoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Matches.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HistoryProperty =
            DependencyProperty.Register("History", typeof(List<Match>), typeof(EloChart), new PropertyMetadata(null, new PropertyChangedCallback(SourceChanged)));

        private static void SourceChanged(DependencyObject depObj,
           DependencyPropertyChangedEventArgs args)
        {
            EloChart chart = (EloChart)depObj;
            chart.UpdateSource();
        }

        public EloChart()
        {
            InitializeComponent();
            chart.DataContext = this;
        }

        public void UpdateSource()
        {
            var WinBrush = new SolidColorBrush(Colors.Green);
            var LossBrush = new SolidColorBrush(Colors.Red);
            var mapper = Mappers.Xy<Match>() //in this case value is of type <ObservablePoint>
                .X((value, index) => index) //use the X property as X
                .Y((value, index) => value.ELO) //use the Y property as Y
                .Fill(value => value.Result == 'W' ? WinBrush : LossBrush);
            var matchArray = History.ToArray();
            var values = matchArray.Reverse().Where(m => m.ELO != 0).AsChartValues();
            MatchSeries = new SeriesCollection(mapper)
            {
                new LineSeries
                {
                    Title = "ELO",
                    Values = values
                }
            };
            Formatter = value => value.ELO.ToString();
        }
    }
}
