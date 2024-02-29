using System;
using System.Diagnostics;
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
            var url = "https://rob-games.zapto.org/";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Button_Click_URL_Github(object sender, RoutedEventArgs e)
        {
            var url = "https://github.com/ROBdk97/RushHour2";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
