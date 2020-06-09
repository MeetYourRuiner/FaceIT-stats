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

namespace faceitwpf.Controls
{
    /// <summary>
    /// Логика взаимодействия для EloChart.xaml
    /// </summary>
    public partial class EloChart : UserControl, INotifyPropertyChanged
    {
        private int _to;
        private int _from;
        private int itemsCount;

        public SeriesCollection MatchSeries { get; set; }
        public Func<Match, string> Formatter { get; set; }

        public int From
        {
            get { return _from; }
            set
            {
                if (value <= 0)
                {
                    value = 0;
                }
                _from = value;
                OnPropertyChanged("From");
            }
        }

        public int To
        {
            get { return _to; }
            set
            {
                if (value >= itemsCount)
                {
                    value = itemsCount;
                    From = itemsCount - 20;
                }
                _to = value;
                OnPropertyChanged("To");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NextOnClick(object sender, RoutedEventArgs e)
        {
            To += 20;
            if (To != itemsCount)
                From += 20;
        }

        private void PrevOnClick(object sender, RoutedEventArgs e)
        {
            From -= 20;
            if (From != 0)
                To -= 20;
        }

        public EloChart()
        {
            InitializeComponent();
        }

        public void SetSource(Match[] matches)
        {
            var mapper = Mappers.Xy<Match>() //in this case value is of type <ObservablePoint>
                .X((value, index) => index) //use the X property as X
                .Y((value, index) => value.ELO); //use the Y property as Y
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
            itemsCount = values.Count - 1;
            To = itemsCount;
        }
    }
}
