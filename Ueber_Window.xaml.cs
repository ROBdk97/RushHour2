using System;
using System.Linq;
using System.Windows;

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
