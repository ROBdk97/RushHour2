using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RushHour2
{
    /// <summary>
    /// Interaction logic for UsernameWindow.xaml
    /// </summary>
    public partial class UsernameWindow : Window
    {
        public UsernameWindow()
        {
            InitializeComponent();
        }

        private void UsernameButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public string Username
        {
            get { return UsernameTextBox.Text; }
        }

        // if the user presses the enter key, the username is accepted
        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (UsernameTextBox.Text.Length > 0)
                if (e.Key == Key.Enter)
                {
                    DialogResult = true;
                    Close();
                }
        }
    }
}
