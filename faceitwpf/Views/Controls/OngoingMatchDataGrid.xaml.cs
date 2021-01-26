using faceitwpf.Models;
using faceitwpf.Views.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace faceitwpf.Views.Controls
{
    public partial class OngoingMatchDataGrid : UserControl
    {
        public OngoingMatchTeamInfo TeamSource
        {
            get { return (OngoingMatchTeamInfo)GetValue(TeamSourceProperty); }
            set { SetValue(TeamSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TeamSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TeamSourceProperty =
            DependencyProperty.Register("TeamSource", typeof(OngoingMatchTeamInfo), typeof(OngoingMatchDataGrid), new PropertyMetadata(null));

        public OngoingMatchDataGrid()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory tbFactory = new FrameworkElementFactory(typeof(TextBlock));
            FrameworkElementFactory elFactory = new FrameworkElementFactory(typeof(Ellipse));

            Binding textBinding = new Binding("Nickname");
            tbFactory.SetBinding(TextBlock.TextProperty, textBinding);

            Binding fillBinding = new Binding("PartyIndex") { Converter = new PartyColorConverter() };
            elFactory.SetValue(Ellipse.WidthProperty, 10.0);
            elFactory.SetValue(Ellipse.HeightProperty, 10.0);
            elFactory.SetBinding(Ellipse.FillProperty, fillBinding);

            spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            if (this.HorizontalContentAlignment == HorizontalAlignment.Right)
            {
                DockPanel.SetDock(spAverageElo, Dock.Left);
                DockPanel.SetDock(spAverageLevel, Dock.Left);
                DockPanel.SetDock(lTeamName, Dock.Right);
                lTeamName.HorizontalContentAlignment = HorizontalAlignment.Right;


                colNickname.DisplayIndex = 0;
                colLevel.DisplayIndex = 1;
                colAvatar.DisplayIndex = 2;

                tbFactory.SetValue(TextBlock.MarginProperty, new Thickness(10, 0, 0, 0));

                spFactory.SetValue(StackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Right);

                spFactory.AppendChild(elFactory);
                spFactory.AppendChild(tbFactory);
            }
            else
            {
                tbFactory.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 10, 0));

                spFactory.AppendChild(tbFactory);
                spFactory.AppendChild(elFactory);
            }
            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = spFactory;
            colNickname.CellTemplate = cellTemplate;
        }
    }
}
