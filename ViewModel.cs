using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2
{
    public class ViewModel : BaseModel
    {
        public ViewModel()
        {
            LoadSettings();
            Spiele = new List<Spiel>();
            Scores = new ObservableCollection<Score>();
            ReloadXML();
        }

        private static ObservableCollection<Score> GetAllScores()
        {
            try
            {
                using HttpClient client = new();
                var scores = client.GetFromJsonAsync<ObservableCollection<Score>>(getScores);
                return scores.Result;
            }
            catch (Exception)
            {
                return [];
            }
        }

        private void GetUserName()
        {
            if (!string.IsNullOrWhiteSpace(Username))
                return;
            // open Window to ask for Username
            UsernameWindow usernameWindow = new();
            usernameWindow.ShowDialog();
            if (usernameWindow.DialogResult == true)
                if (!string.IsNullOrWhiteSpace(usernameWindow.Username))
                    Username = usernameWindow.Username;
                else
                    Username = "Anonym";
        }

        private void LoadSettings()
        {
            XmlSerializer xmlSerializer = new(typeof(Settings));
            if (File.Exists(path))
            {
                using StreamReader reader = new(path);
                settings = (Settings)xmlSerializer.Deserialize(reader);
            }
            else
                settings = new Settings();
            Username = settings.Username;
            SelectedGame = settings.Level;
            GetUserName();
        }

        private string PointToString(Point[] koor, int i)
        {
            string s = "Pos Fahrzeug" + i + ":\n";
            foreach (Point point in koor)
            {
                s += point.X + "," + point.Y + "\n";
            }
            //s.Length -= 2;
            return s;
        }

        internal static Score AddScore(Score score)
        {
            try
            {
                using HttpClient client = new();
                var response = client.PostAsJsonAsync(postScores, score);
                var result = response.Result.Content.ReadFromJsonAsync<Score>();
                return result.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void ExportXML()
        {
            SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = "c:\\",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                XMLHelper.SaveXML(Spiele, saveFileDialog.FileName);
            }
        }

        internal static ObservableCollection<Score> GetScoreForLevel(int lvl)
        {
            try
            {
                using HttpClient client = new();
                var scores = client.GetFromJsonAsync<ObservableCollection<Score>>(getLvl + lvl);
                return scores.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal void ImportXML()
        {
            var assembly = Assembly.GetExecutingAssembly();
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (fileDialog.ShowDialog() == true)
            {
                Spiele = XMLHelper.LoadXML(fileDialog.OpenFile());
                foreach (Spiel spiel in Spiele)
                {
                    spiel.ConvertFahreuge();
                }
            }
        }

        internal void LoadScoresForLevel()
        {
            // get scores for level
            var scores = GetScoreForLevel(SelectedGame);
            Scores = scores ?? [];
            OnPropertyChanged(nameof(Scores));
        }

        public void Add()
        {
            if ((SelectedFahrzeug + 1) <= GetCurrentFahrzeuge().Count - 1)
                SelectedFahrzeug++;
        }

        public void Backwards()
        {
            //Out("Try Backwards:");
            if (isMoveAllowed(GetCurrentFahrzeug(), true))
            {
                //Out(PointToString(GetCurrentFahrzeug().GetPosition(), SelF));
                GetCurrentFahrzeug().Backwards();
                //Out(PointToString(GetCurrentFahrzeug().GetPosition(), SelF));
                Text += "---------------\n";
                Moves++;
            }
        }

        public void FahrzeugeAusString()
        {
            Spiel spiel = new Spiel { Fahrzeug = new List<FahrzeugX>() };
            string[] txt = new string[6];
            txt[0] = "<Fahrzeug>";
            txt[1] = "<X>2</X>";
            txt[2] = "<Y>2</Y>";
            txt[3] = "<Laenge>2</Laenge>";
            txt[4] = "<Ausrichtung>r</Ausrichtung>";
            txt[5] = "</Fahrzeug>";

            string s = "<?xml version=\"1.0\" ?>" + string.Join(string.Empty, txt);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(s);
            XmlSerializer xs = new XmlSerializer(typeof(FahrzeugX));
            FahrzeugX fahrzeug = (FahrzeugX)xs.Deserialize(new StringReader(doc.OuterXml));
            spiel.Fahrzeug.Add(fahrzeug);
            spiel.ConvertFahreuge();
            Spiele.Add(spiel);
            SelectedGame = Spiele.Count - 1;
        }

        public void Finished()
        {
            Score score = new Score()
            {
                Date = DateTime.Today,
                Lvl = SelectedGame,
                Moves = Moves,
                Time = DateTime.Now - startTime,
                Username = Username
            };
            var result = AddScore(score);
            if (result != null)
            {
                Scores.Add(result);
                OnPropertyChanged(nameof(Scores));
                Erfolg erfolgWindow = new Erfolg
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow,
                    DataContext = result
                };
                erfolgWindow.ShowDialog();
                Moves = 0;
            }
        }

        public void Forwards()
        {
            //Out("Try Forward:");
            if (isMoveAllowed(GetCurrentFahrzeug(), false))
            {
                //Out(PointToString(GetCurrentFahrzeug().GetPosition(), SelF));
                GetCurrentFahrzeug().Forwards();
                //Out(PointToString(GetCurrentFahrzeug().GetPosition(), SelF));
                Text += "---------------\n";
                Moves++;
            }
        }

        public bool Geloest()
        {
            if (SelectedFahrzeug == 0)
            {
                Fahrzeug meinFahrzeug = GetCurrentFahrzeug();
                foreach (Point p in meinFahrzeug.GetPosition())
                {
                    if (p.X == 5 && p.Y == 2)
                        return true;
                }
            }
            return false;
        }

        public Fahrzeug GetCurrentFahrzeug() { return GetCurrentFahrzeuge()[SelectedFahrzeug]; }

        public List<Fahrzeug> GetCurrentFahrzeuge()
        {
            if (SelectedGame > Spiele.Count - 1)
                SelectedGame = Spiele.Count - 1;
            return Spiele[SelectedGame].Fahrzeuge;
        }

        public bool isMoveAllowed(Fahrzeug fahrzeug, bool forwards)
        {
            foreach (Point p in fahrzeug.GetPosition())
            {
                if (forwards)
                {
                    switch (fahrzeug.Direction)
                    {
                        case "o":
                            if (!isSpace(new Point(p.X, p.Y + 1), fahrzeug))
                                return false;
                            break;
                        case "u":
                            if (!isSpace(new Point(p.X, p.Y - 1), fahrzeug))
                                return false;
                            break;
                        case "l":
                            if (!isSpace(new Point(p.X + 1, p.Y), fahrzeug))
                                return false;
                            break;
                        case "r":
                            if (!isSpace(new Point(p.X - 1, p.Y), fahrzeug))
                                return false;
                            break;
                    }
                }
                else
                {
                    switch (fahrzeug.Direction)
                    {
                        case "o":
                            if (!isSpace(new Point(p.X, p.Y - 1), fahrzeug))
                                return false;
                            break;
                        case "u":
                            if (!isSpace(new Point(p.X, p.Y + 1), fahrzeug))
                                return false;
                            break;
                        case "l":
                            if (!isSpace(new Point(p.X - 1, p.Y), fahrzeug))
                                return false;
                            break;
                        case "r":
                            if (!isSpace(new Point(p.X + 1, p.Y), fahrzeug))
                                return false;
                            break;
                    }
                }
            }
            return true;
        }

        public bool isSpace(Point p, Fahrzeug thisFahrzeug)
        {
            foreach (Fahrzeug otherFahrzeug in GetCurrentFahrzeuge())
            {
                if (otherFahrzeug != thisFahrzeug)
                {
                    if (otherFahrzeug.isField(p))
                        return false;
                }
            }
            if (p.X < 0 || p.X > GridSize - 1)
                return false;
            if (p.Y < 0 || p.Y > GridSize - 1)
                return false;
            return true;
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    if (GetCurrentFahrzeug().Direction == "u")
                        Backwards();
                    if (GetCurrentFahrzeug().Direction == "o")
                        Forwards();
                    break;
                case Direction.Down:
                    if (GetCurrentFahrzeug().Direction == "o")
                        Backwards();
                    if (GetCurrentFahrzeug().Direction == "u")
                        Forwards();
                    break;
                case Direction.Right:
                    if (GetCurrentFahrzeug().Direction == "r")
                        Forwards();
                    if (GetCurrentFahrzeug().Direction == "l")
                        Backwards();
                    break;
                case Direction.Left:
                    if (GetCurrentFahrzeug().Direction == "l")
                        Forwards();
                    if (GetCurrentFahrzeug().Direction == "r")
                        Backwards();
                    break;
                default:
                    Console.WriteLine("Move error");
                    break;
            }
        }

        public void Out(string text) { Text += text + "\n"; }

        public void ReloadLevel()
        {
            List<Spiel> games;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(
                assembly.GetManifestResourceNames().Single(file => file.EndsWith("spiel.xml"))))
                games = XMLHelper.LoadXML(stream);
            foreach (Spiel spiel in games)
            {
                spiel.ConvertFahreuge();
            }
            // replace level
            Spiele[SelectedGame] = games[SelectedGame];
        }


        public void ReloadXML()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(
                assembly.GetManifestResourceNames().Single(file => file.EndsWith("spiel.xml"))))
                Spiele = XMLHelper.LoadXML(stream);
            foreach (Spiel spiel in Spiele)
            {
                spiel.ConvertFahreuge();
            }
            Scores = GetScoreForLevel(SelectedGame);
        }

        public void ResetProperties()
        {
            ViewModel _viewModel = new();
            SelectedFahrzeug = 0;
            Moves = 0;
            SelectedGame = 0;
            Spiele = _viewModel.Spiele;
        }

        public void SaveSettings()
        {
            XmlSerializer xmlSerializer = new(typeof(Settings));
            if (!File.Exists(path))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            using StreamWriter writer = new(path);
            settings.Username = Username;
            settings.Level = SelectedGame;
            xmlSerializer.Serialize(writer, settings);
        }

        public void SetCurrentFahrzeug(Point p)
        {
            int x = (int)Math.Floor(p.X / Scale);
            int y = (int)Math.Floor(p.Y / Scale);
            int i = 0;
            foreach (Fahrzeug f in GetCurrentFahrzeuge())
            {
                foreach (Point pf in f.GetPosition())
                {
                    if (pf.X == x && pf.Y == y)
                    {
                        SelectedFahrzeug = i;
                        //Console.WriteLine("Sel: " + Sel);
                        return;
                    }
                }
                i++;
            }
        }

        public void StartTime() { startTime = DateTime.Now; }

        public void Sub()
        {
            if ((SelectedFahrzeug - 1) >= 0)
                SelectedFahrzeug--;
        }

        #region private
        private string text = string.Empty;
        private string username = "user";
        private int selectedFahrzeug = 0;
        private int selectedGame = 0;
        private int scale = 70;
        private int gridSize = 6;
        private List<Spiel> spiele;
        private int moves = 0;
        private DateTime startTime;
        public ObservableCollection<Score> scores;
        private Settings settings;
        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
            "\\ROBdk97\\RushHour\\Settings.xml";
        private const string getScores = "https://rob-games.zapto.org:4447/api/RushHour/scores";
        private const string postScores = "https://rob-games.zapto.org:4447/api/RushHour/score";
        private const string getLvl = "https://rob-games.zapto.org:4447/api/RushHour/level?level=";
        #endregion private

        #region properties
        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public int SelectedFahrzeug
        {
            get => selectedFahrzeug;
            set
            {
                selectedFahrzeug = value;
                OnPropertyChanged();
            }
        }

        public int Moves
        {
            get => moves;
            set
            {
                moves = value;
                OnPropertyChanged();
                if (moves == 1)
                {
                    StartTime();
                }
            }
        }

        public int SelectedGame
        {
            get => selectedGame;
            set
            {
                selectedGame = value;
                OnPropertyChanged();
                LoadScoresForLevel();
            }
        }

        public List<Spiel> Spiele
        {
            get => spiele;
            set
            {
                spiele = value;
                OnPropertyChanged();
            }
        }

        public int Scale { get => scale; set { scale = value; } }

        public int GridSize
        {
            get => gridSize;
            set
            {
                gridSize = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Score> Scores
        {
            get => scores;
            set
            {
                scores = value;
                OnPropertyChanged();
            }
        }
        #endregion properties

    }
}
