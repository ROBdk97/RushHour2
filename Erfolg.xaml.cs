using System;
using System.Linq;
using System.Windows;

namespace RushHour2
{
    /// <summary>
    /// Interaktionslogik für Erfolg.xaml
    /// </summary>
    public partial class Erfolg : Window
    {
        public Erfolg()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
