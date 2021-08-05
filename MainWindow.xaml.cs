using System;
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
    public partial class MainWindow : Window, IMainWindow
    {
        private ViewModel viewModel;
        private bool grid = false;

        private SolidColorBrush selectedColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush mainColor = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush targetColor = new SolidColorBrush(Colors.DarkViolet);


        public MainWindow()
        {
            viewModel = new ViewModel(this);
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void CanvasGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("X:" + (int)e.GetPosition(CanvasGrid).X / viewModel.Mod + ",Y:" + (int)e.GetPosition(CanvasGrid).Y / viewModel.Mod);
            viewModel.SetCurrentFahrzeug(e.GetPosition(CanvasGrid));
            Console.WriteLine(viewModel.GetCurrentFahrzeug());
            aktuallisiereSpielfeld();
        }

        private void GoUp(object sender, RoutedEventArgs e)
        {
            viewModel.Move(ViewModel.direction.up);
            aktuallisiereSpielfeld();
        }

        private void GoDown(object sender, RoutedEventArgs e)
        {
            viewModel.Move(ViewModel.direction.down);
            aktuallisiereSpielfeld();
        }

        private void GoRight(object sender, RoutedEventArgs e)
        {
            viewModel.Move(ViewModel.direction.right);
            aktuallisiereSpielfeld();
        }

        private void GoLeft(object sender, RoutedEventArgs e)
        {
            viewModel.Move(ViewModel.direction.left);
            aktuallisiereSpielfeld();
        }

        private void Restart(object sender, RoutedEventArgs e)
        {
            viewModel.ReloadXML();
            viewModel.Moves = 0;
            aktuallisiereSpielfeld();
        }

        private void GameMinus(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelG > 0)
            {
                viewModel.SelG--;
                viewModel.Sel = 0;
                aktuallisiereSpielfeld();
            }
        }

        private void GamePlus(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelG < viewModel.Spiele.Count - 1)
            {
                viewModel.SelG++;
                viewModel.Sel = 0;
                aktuallisiereSpielfeld();
            }
        }

        public void aktuallisiereSpielfeld(bool newGrid = false)
        {
            if (newGrid) grid = false;
            DrawRectangles(CanvasBackground);
            CanvasGrid.Children.Clear();
            CanvasGrid.Width = viewModel.Mod * viewModel.GridSize;
            CanvasGrid.Height = viewModel.Mod * viewModel.GridSize;
            int i = 0;
            bool first = true;
            foreach (Fahrzeug fahrzeug in viewModel.GetCurrentFahrzeuge())
            {
                bool sel = i == viewModel.Sel ? true : false;
                gibFahrzeug(fahrzeug, sel, first);
                if (first) first = false;
                i++;
            }
            drawWinRectangle();
            if (viewModel.Geloest())
            {
                Erfolg erfolgWindow = new Erfolg();
                erfolgWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                erfolgWindow.Owner = this;
                erfolgWindow.DataContext = viewModel;
                erfolgWindow.ShowDialog();
                viewModel.Moves = 0;
                GamePlus(null, null);
                viewModel.Text = "";
            }
        }

        private void drawWinRectangle()
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Fill = targetColor;
            rect.Opacity = .75;
            rect.Width = viewModel.Mod * 1;
            rect.Height = viewModel.Mod * 1;
            Canvas.SetLeft(rect, viewModel.Mod * 5);
            Canvas.SetTop(rect, viewModel.Mod * 2);
            CanvasGrid.Children.Add(rect);
        }

        private void gibFahrzeug(Fahrzeug fahrzeug, bool sel, bool first)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image
            {
                Width = viewModel.Mod * fahrzeug.Width(),
                Height = viewModel.Mod * fahrzeug.Heigth(),
                Source = new BitmapImage(new Uri("/Ressourcen/" + fahrzeug.GetImageUri(), UriKind.Relative))
            };
            double x = fahrzeug.getPos()[0].X;
            double y = fahrzeug.getPos()[0].Y;
            if (fahrzeug.Direction == "u" || fahrzeug.Direction == "r")
            {
                x = fahrzeug.getPos()[fahrzeug.getPos().Length - 1].X;
                y = fahrzeug.getPos()[fahrzeug.getPos().Length - 1].Y;
            }

            Canvas.SetLeft(image, viewModel.Mod * x);
            Canvas.SetTop(image, viewModel.Mod * y);
            CanvasGrid.Children.Add(image);
            if (first)
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                rect.Fill = mainColor;
                rect.Opacity = .5;
                rect.Width = viewModel.Mod * fahrzeug.Width();
                rect.Height = viewModel.Mod * fahrzeug.Heigth();
                Canvas.SetLeft(rect, viewModel.Mod * x);
                Canvas.SetTop(rect, viewModel.Mod * y);
                CanvasGrid.Children.Add(rect);
            }
            if (sel)
            {
                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                rect.Fill = selectedColor;
                rect.Opacity = .5;
                rect.Width = viewModel.Mod * fahrzeug.Width();
                rect.Height = viewModel.Mod * fahrzeug.Heigth();
                Canvas.SetLeft(rect, viewModel.Mod * x);
                Canvas.SetTop(rect, viewModel.Mod * y);
                CanvasGrid.Children.Add(rect);
            }
        }

        public void DrawRectangles(Canvas MyCanvas)
        {
            if (!grid)
            {
                MyCanvas.Children.RemoveRange(1, MyCanvas.Children.Count - 1);
                MyCanvas.Width = viewModel.Mod * viewModel.GridSize;
                MyCanvas.Height = viewModel.Mod * viewModel.GridSize;
                for (int j = 0; j < viewModel.GridSize; j++)
                {
                    for (int i = 0; i < viewModel.GridSize; i++)
                    {
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle
                        {
                            Height = viewModel.Mod,
                            Width = viewModel.Mod,
                        };
                        rectangle.Stroke = System.Windows.Media.Brushes.Black;
                        rectangle.Fill = System.Windows.Media.Brushes.LightGray;
                        Canvas.SetLeft(rectangle, i * viewModel.Mod);
                        Canvas.SetTop(rectangle, j * viewModel.Mod);
                        MyCanvas.Children.Add(rectangle);
                    }
                }
                grid = true;
            }
        }

        private void ImportXML(object sender, RoutedEventArgs e)
        {
            viewModel.ImportXML();
        }

        private void MenuItem_Click_AusString(object sender, RoutedEventArgs e)
        {
            viewModel.FahrzeugeAusString();
        }

        private void MenuItem_Click_Anleitung(object sender, RoutedEventArgs e)
        {
            Anleitung_Window anleitung_Window = new Anleitung_Window();
            anleitung_Window.Owner = this;
            anleitung_Window.Show();
        }

        private void MenuItem_Click_Ueber(object sender, RoutedEventArgs e)
        {
            Ueber_Window ueberWindow = new Ueber_Window();
            ueberWindow.Owner = this;
            ueberWindow.Show();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            CanvasGrid.Children.Remove(StartButton);
            aktuallisiereSpielfeld();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewModel.Mod =(int) (this.ActualHeight * 0.6 / viewModel.GridSize);
            //Console.WriteLine(this.ActualWidth * 0.71428571428f / viewModel.GridSize);
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
            }
        }
    }
}
