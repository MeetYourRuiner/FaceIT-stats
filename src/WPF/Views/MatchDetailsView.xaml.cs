using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FaceitStats.WPF.Views
{
    /// <summary>
    /// Interaction logic for MatchView.xaml
    /// </summary>
    public partial class MatchDetailsView : UserControl
    {
        public MatchDetailsView()
        {
            InitializeComponent();
        }

        private void control_Loaded(object sender, RoutedEventArgs e)
        {
            double dataGridsHeight = dataGrids.ActualHeight;
            Style rowStyle = (Style)this.Resources["MatchDetailsDGRowStyle"];
            Style baseRowStyle = rowStyle.BasedOn;
            Style columnHeaderStyle = (Style)this.Resources["MatchDetailsDGColumnHeaderStyle"];
            Style baseColumnHeaderStyle = columnHeaderStyle.BasedOn;
            Setter marginSetter = (Setter)baseRowStyle.Setters.Where(s => ((Setter)s).Property == MarginProperty).FirstOrDefault();
            double bottomMargin = ((Thickness)marginSetter.Value).Bottom;
            double rowHeight = (dataGridsHeight - 10 * bottomMargin) / 12;

            Style newRowStyle = new Style
            {
                BasedOn = baseRowStyle,
                TargetType = typeof(DataGridRow)
            };
            newRowStyle.Setters.Add(new Setter(DataGridRow.HeightProperty, rowHeight));
            Style newColumnHeaderStyle = new Style
            {
                BasedOn = baseColumnHeaderStyle,
                TargetType = typeof(DataGridColumnHeader)
            };
            newColumnHeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HeightProperty, rowHeight));

            this.Resources["MatchDetailsDGRowStyle"] = newRowStyle;
            this.Resources["MatchDetailsDGColumnHeaderStyle"] = newColumnHeaderStyle;
        }
    }
}
