using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для Name.xaml
    /// </summary>
    public partial class Name : Page
    {
        public Name()
        {
            InitializeComponent();
        }
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (text1.Text.Length > 0 && text1.Text.Length < 30)
            {
                try
                {
                    DataPage datapage = new DataPage(text1.Text);
                    this.NavigationService.Navigate(datapage);
                }
                catch(ArgumentException ex)
                {
                    text1.Text = "Wrong nickname";
                }
            }
        }

        private void btn_MouseEnter(object sender, MouseEventArgs e)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 125, 0));
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 85, 0));
        }
    }
}
