using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FaceitStats.WPF.Views
{
    /// <summary>
    /// Interaction logic for TeamAnalyzeView.xaml
    /// </summary>
    public partial class TeamAnalyzeView : UserControl
    {
        public TeamAnalyzeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double controlHeight = this.ActualHeight;
            Setter marginSetter = (Setter)dgMaps.RowStyle.Setters.Where(s => ((Setter)s).Property == MarginProperty).FirstOrDefault();
            double bottomMargin = ((Thickness)marginSetter.Value).Bottom;
            int rowsCount = FaceitStats.Core.Constants.FaceitConstants.Maps.Length + 2;
            double rowHeight = (controlHeight - 73 - rowsCount * bottomMargin) / rowsCount;
            this.Resources["RowHeight"] = rowHeight;
        }
    }
}
