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
using System.Windows.Shapes;

namespace RushHour2
{
    /// <summary>
    /// Interaktionslogik für Ueber_Window.xaml
    /// </summary>
    public partial class Ueber_Window : Window
    {
        public Ueber_Window()
        {
            InitializeComponent();
        }

        private void Button_Click_URL_ROB(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://rob-games.zapto.org/");
        }

        private void Button_Click_URL_Schule(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.berufliche-schulen-bretten.de/");
        }
    }
}
