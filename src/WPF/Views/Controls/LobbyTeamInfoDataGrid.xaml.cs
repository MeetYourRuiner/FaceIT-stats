using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FaceitStats.WPF.Views.Controls
{
    public partial class LobbyTeamInfoDataGrid : UserControl
    {
        public LobbyTeamInfoDataGrid()
        {
            InitializeComponent();
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            double controlHeight = control.ActualHeight;
            Setter marginSetter = (Setter)dgTeam.RowStyle.Setters.Where(s => ((Setter)s).Property == MarginProperty).FirstOrDefault();
            double bottomMargin = ((Thickness)marginSetter.Value).Bottom;
            int players = 5;
            double rowHeight = (controlHeight - players * bottomMargin) / (players + 1);
            this.Resources["RowHeight"] = rowHeight;
        }
    }
}
