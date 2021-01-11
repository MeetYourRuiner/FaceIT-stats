using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows.Controls;

namespace faceitwpf.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChartToolTip.xaml
    /// </summary>
    public partial class ChartToolTip : UserControl, IChartTooltip
    {
        private TooltipData _data;

        public ChartToolTip()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
