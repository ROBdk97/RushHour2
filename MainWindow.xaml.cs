using System;
using System.Collections.Generic;
using System.Drawing;
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
using Color = System.Drawing.Color;

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
            if (viewModel.SelG < viewModel.Spiele.Count-1)
            {
                viewModel.SelG++;
                viewModel.Sel = 0;
                aktuallisiereSpielfeld();
            }
        }

        public void aktuallisiereSpielfeld()
        {
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
                GamePlus(null,null);
            }
        }

        private void drawWinRectangle()
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Fill = targetColor;
            rect.Opacity = .5;
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
                x= fahrzeug.getPos()[fahrzeug.getPos().Length-1].X;
                y = fahrzeug.getPos()[fahrzeug.getPos().Length-1].Y;
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
            anleitung_Window.Show();
        }

        private void MenuItem_Click_Ueber(object sender, RoutedEventArgs e)
        {
            Ueber_Window ueberWindow = new Ueber_Window();
            ueberWindow.Show();
        }
    }
}
