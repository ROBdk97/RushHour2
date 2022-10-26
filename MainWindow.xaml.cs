using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RushHour2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ViewModel viewModel;
        private bool grid = false;

        private SolidColorBrush selectedColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush mainColor = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush targetColor = new SolidColorBrush(Colors.DarkViolet);

        private HelpWindow helpWindow;

        public ViewModel ViewModel { get => viewModel; set { viewModel = value; OnPropertyChanged(); } }

        public MainWindow()
        {
            ViewModel = new ViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
            Window_SizeChanged(this, null);
        }

        private void CanvasGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("X:" + (int)e.GetPosition(CanvasGrid).X / ViewModel.Mod + ",Y:" + (int)e.GetPosition(CanvasGrid).Y / ViewModel.Mod);
            ViewModel.SetCurrentFahrzeug(e.GetPosition(CanvasGrid));
            //Console.WriteLine(ViewModel.GetCurrentFahrzeug());
            RedrawSpielfeld();
        }

        private void GoUp(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(Direction.Up);
            RedrawSpielfeld();
        }

        private void GoDown(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(Direction.Down);
            RedrawSpielfeld();
        }

        private void GoRight(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(Direction.Right);
            RedrawSpielfeld();
        }

        private void GoLeft(object sender, RoutedEventArgs e)
        {
            ViewModel.Move(Direction.Left);
            RedrawSpielfeld();
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            ViewModel.ReloadLevel();
            ViewModel.Moves = 0;
            ViewModel.StartTime();
            RedrawSpielfeld();
        }

        private void RestartGame(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetProperties();
            RedrawSpielfeld(true);
            Window_SizeChanged(this, null);
        }

        private void GameMinus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedGame > 0)
            {
                ViewModel.SelectedGame--;
                ViewModel.SelectedFahrzeug = 0;
                if (ViewModel.Geloest()) ViewModel.SelectedGame++;
                RedrawSpielfeld();
            }
        }

        private void GamePlus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedGame < ViewModel.Spiele.Count - 1)
            {
                ViewModel.SelectedGame++;
                ViewModel.SelectedFahrzeug = 0;
                RedrawSpielfeld();
            }
        }

        public void RedrawSpielfeld(bool newGrid = false)
        {
            if (newGrid) grid = false;
            DrawRectangles(CanvasBackground);
            CanvasGrid.Children.Clear();
            CanvasGrid.Width = ViewModel.Scale * ViewModel.GridSize;
            CanvasGrid.Height = ViewModel.Scale * ViewModel.GridSize;
            int i = 0;
            bool first = true;
            foreach (Fahrzeug fahrzeug in ViewModel.GetCurrentFahrzeuge())
            {
                bool sel = i == ViewModel.SelectedFahrzeug ? true : false;
                GibFahrzeug(fahrzeug, sel, first);
                if (first) first = false;
                i++;
            }
            DrawGrid();
            if (ViewModel.Geloest())
            {
                ViewModel.Finished();
                Erfolg erfolgWindow = new Erfolg
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this,
                    DataContext = ViewModel                  
                };
                erfolgWindow.ShowDialog();
                ViewModel.StartTime();
                ViewModel.Moves = 0;
                GamePlus(null, null);
                ViewModel.Text = "";

                if (viewModel.SelectedGame == viewModel.Spiele.Count - 1)
                {
                    MessageBox.Show("Sie haben das Spiel durchgespielt!", "Gratulation!");
                    RestartGame(this, null);
                }
            }
        }

        private void DrawGrid()
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
            {
                Fill = targetColor,
                Opacity = .75,
                Width = ViewModel.Scale * 1,
                Height = ViewModel.Scale * 1
            };
            Canvas.SetLeft(rect, ViewModel.Scale * 5);
            Canvas.SetTop(rect, ViewModel.Scale * 2);
            CanvasGrid.Children.Add(rect);
        }

        private void GibFahrzeug(Fahrzeug fahrzeug, bool sel, bool first)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image
            {
                Width = ViewModel.Scale * fahrzeug.Width(),
                Height = ViewModel.Scale * fahrzeug.Heigth(),
                Source = new BitmapImage(new Uri("/Ressourcen/" + fahrzeug.GetImageUri(), UriKind.Relative))
            };
            double x = fahrzeug.GetPosition()[0].X;
            double y = fahrzeug.GetPosition()[0].Y;
            if (fahrzeug.Direction == "u" || fahrzeug.Direction == "r")
            {
                x = fahrzeug.GetPosition()[fahrzeug.GetPosition().Length - 1].X;
                y = fahrzeug.GetPosition()[fahrzeug.GetPosition().Length - 1].Y;
            }

            Canvas.SetLeft(image, ViewModel.Scale * x);
            Canvas.SetTop(image, ViewModel.Scale * y);
            CanvasGrid.Children.Add(image);
            if (first)
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                {
                    Fill = mainColor,
                    Opacity = .5,
                    Width = ViewModel.Scale * fahrzeug.Width(),
                    Height = ViewModel.Scale * fahrzeug.Heigth()
                };
                Canvas.SetLeft(rect, ViewModel.Scale * x);
                Canvas.SetTop(rect, ViewModel.Scale * y);
                CanvasGrid.Children.Add(rect);
            }
            if (sel)
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                {
                    Fill = selectedColor,
                    Opacity = .5,
                    Width = ViewModel.Scale * fahrzeug.Width(),
                    Height = ViewModel.Scale * fahrzeug.Heigth()
                };
                Canvas.SetLeft(rect, ViewModel.Scale * x);
                Canvas.SetTop(rect, ViewModel.Scale * y);
                CanvasGrid.Children.Add(rect);
            }
        }

        public void DrawRectangles(Canvas MyCanvas)
        {
            if (!grid)
            {
                MyCanvas.Children.RemoveRange(1, MyCanvas.Children.Count - 1);
                MyCanvas.Width = ViewModel.Scale * ViewModel.GridSize;
                MyCanvas.Height = ViewModel.Scale * ViewModel.GridSize;
                for (int j = 0; j < ViewModel.GridSize; j++)
                {
                    for (int i = 0; i < ViewModel.GridSize; i++)
                    {
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                        {
                            Height = ViewModel.Scale,
                            Width = ViewModel.Scale,
                        };
                        rectangle.Stroke = System.Windows.Media.Brushes.Black;
                        rectangle.Fill = System.Windows.Media.Brushes.LightGray;
                        Canvas.SetLeft(rectangle, i * ViewModel.Scale);
                        Canvas.SetTop(rectangle, j * ViewModel.Scale);
                        MyCanvas.Children.Add(rectangle);
                    }
                }
                grid = true;
            }
        }

        private void ImportXML(object sender, RoutedEventArgs e)
        {
            ViewModel.ImportXML();
        }

        private void MenuItem_Click_AusString(object sender, RoutedEventArgs e)
        {
            ViewModel.FahrzeugeAusString();
            RedrawSpielfeld(true);
        }

        private void MenuItem_Click_Anleitung(object sender, RoutedEventArgs e)
        {
            ShowHelp();
        }

        private void MenuItem_Click_Ueber(object sender, RoutedEventArgs e)
        {
            Ueber_Window ueberWindow = new Ueber_Window
            {
                Owner = this
            };
            ueberWindow.Show();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            CanvasGrid.Children.Remove(StartButton);
            RedrawSpielfeld();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.Scale =(int) (this.ActualHeight * 0.6 / ViewModel.GridSize);
            RedrawSpielfeld(true);
        }

        private void ShowHelp()
        {
            if (helpWindow == null)
            {
                helpWindow = new HelpWindow
                {
                    Owner = this
                };
                helpWindow.Show();
                helpWindow.Closed += HelpClosed;
            }
        }

        private void HelpClosed(object sender, EventArgs e)
        {
            helpWindow = null;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {               
                case Key.W:
                    GoUp(null,null);
                    break;
                case Key.S:
                    GoDown(null, null);
                    break;
                case Key.A:
                    GoLeft(null, null);
                    break;
                case Key.D:
                    GoRight(null, null);
                    break;
                case Key.R:
                    Restart(null, null);
                    break;
                case Key.H:
                    ShowHelp();
                    break;               
            }
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                this.UpButton.Focus();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.SaveScoreBoard();
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion PropertyChanged
    }
}
