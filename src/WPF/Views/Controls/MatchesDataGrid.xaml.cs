using FaceitStats.WPF.ViewModels.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FaceitStats.WPF.Views.Controls
{
    /// <summary>
    /// Interaction logic for MatchesDataGrid.xaml
    /// </summary>
    public partial class MatchesDataGrid : UserControl
    {
        public MatchesDataGrid()
        {
            InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            double controlHeight = control.ActualHeight;
            Setter marginProperty = (Setter)dataGrid.RowStyle.Setters.Where(s => ((Setter)s).Property == MarginProperty).FirstOrDefault();
            double bottomMargin = ((Thickness)marginProperty.Value).Bottom;
            int matchesOnPage = ((MatchesViewModel)DataContext).MatchesOnPage;
            double rowHeight = (controlHeight - (2 * matchesOnPage)) / (matchesOnPage + 1);
            dataGrid.ColumnHeaderHeight = rowHeight;
            dataGrid.RowHeight = rowHeight;
        }
    }
}
