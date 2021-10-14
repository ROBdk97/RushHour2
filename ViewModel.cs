using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2
{
    public class ViewModel : BaseModel
    {
        #region private
        private string text = "";
        private string username = "user";
        private int sel = 0;
        private int selg = 0;
        private int mod = 70;
        private int gridSize = 6;
        private List<XML.Spiel> spiele;
        private IMainWindow main;
        private int moves = 0;
        private DateTime startTime;
        public ObservableCollection<Score> scores;
        private readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\ROBdk97\\RushHour\\ScoreBoard.xml";
        #endregion private

        #region properties
        public string Text { get => text; set { text = value; OnPropertyChanged(); } }
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public int Sel { get => sel; set { sel = value; OnPropertyChanged(); } }
        public int Moves { get => moves; set { moves = value; OnPropertyChanged(); } }
        public int SelG { get => selg; set { selg = value; OnPropertyChanged(); OnPropertyChanged(nameof(Scores)); } }
        public List<XML.Spiel> Spiele { get => spiele; set { spiele = value; OnPropertyChanged(); } }
        public int Mod { get => mod; set { mod = value; OnPropertyChanged(); main.aktuallisiereSpielfeld(true); } }
        public int GridSize { get => gridSize; set { gridSize = value; OnPropertyChanged(); main.aktuallisiereSpielfeld(); } }
        public ObservableCollection<Score> Scores { get => new ObservableCollection<Score>(scores.Where(x => x.Lvl==SelG).OrderBy(z=> z.Moves)); set { scores = value; OnPropertyChanged(); } }
        #endregion properties

        public ViewModel(IMainWindow _main)
        {
            main = _main;
            Spiele = new List<XML.Spiel>();
            Scores = LoadScoreBoard();
            ReloadXML();
        }

        private ObservableCollection<Score> LoadScoreBoard()
        {
            if (!File.Exists(path)) return new ObservableCollection<Score>();
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Score>));
            using (StreamReader reader = new StreamReader(path))
            {
                return (ObservableCollection<Score>)serializer.Deserialize(reader);
            }
        }
        public void SaveScoreBoard()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<Score>));
            if (!File.Exists(path)) Directory.CreateDirectory(Path.GetDirectoryName(path));
            using(StreamWriter writer = new StreamWriter(path))
            {
                xmlSerializer.Serialize(writer, scores);
            }
        }

        public void FahrzeugeAusString()
        {
            XML.Spiel spiel = new XML.Spiel
            {
                Fahrzeug = new List<XML.FahrzeugX>()
            };
            string[] txt = new string[6];
            txt[0] = "<Fahrzeug>";
            txt[1] = "<X>2</X>";
            txt[2] = "<Y>2</Y>";
            txt[3] = "<Laenge>2</Laenge>";
            txt[4] = "<Ausrichtung>r</Ausrichtung>";
            txt[5] = "</Fahrzeug>";

            string s = "<?xml version=\"1.0\" ?>" + string.Join("", txt);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(s);
            XmlSerializer xs = new XmlSerializer(typeof(XML.FahrzeugX));
            XML.FahrzeugX fahrzeug = (XML.FahrzeugX)xs.Deserialize(new StringReader(doc.OuterXml));
            spiel.Fahrzeug.Add(fahrzeug);
            spiel.ConvertFahreuge();
            Spiele.Add(spiel);
            SelG = Spiele.Count - 1;
            main.aktuallisiereSpielfeld();
        }

        public bool Geloest()
        {
            if (Sel == 0)
            {
                Fahrzeug meinFahrzeug = GetCurrentFahrzeug();
                foreach (Point p in meinFahrzeug.GetPosition())
                {
                    if (p.X == 5 && p.Y == 2) return true;
                }
            }
            return false;
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
                foreach (XML.Spiel spiel in Spiele)
                {
                    spiel.ConvertFahreuge();
                }
            }
        }

        public void Finished()
        {
            Score score = new Score()
            {
                Date = DateTime.Today,
                Lvl = SelG,
                Moves = Moves,
                Time = DateTime.Now - startTime,
                Username = Username
            };
            scores.Add(score);
        }

        public void StartTime()
        {
            startTime = DateTime.Now;
        }

        public void Forwards()
        {
            Out("Try Forward:");
            if (isMoveAllowed(GetCurrentFahrzeug(), false))
            {
                Out(PointToString(GetCurrentFahrzeug().GetPosition(), Sel));
                GetCurrentFahrzeug().Forwards();
                Out(PointToString(GetCurrentFahrzeug().GetPosition(), Sel));
                Text += "---------------\n";
                moves++;
            }
        }
        public void Backwards()
        {
            Out("Try Backwards:");
            if (isMoveAllowed(GetCurrentFahrzeug(), true))
            {
                Out(PointToString(GetCurrentFahrzeug().GetPosition(), Sel));
                GetCurrentFahrzeug().Backwards();
                Out(PointToString(GetCurrentFahrzeug().GetPosition(), Sel));
                Text += "---------------\n";
                moves++;
            }
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
                    if (otherFahrzeug.isField(p)) return false;
                }
            }
            if (p.X < 0 || p.X > GridSize - 1) return false;
            if (p.Y < 0 || p.Y > GridSize - 1) return false;
            return true;
        }

        public List<Fahrzeug> GetCurrentFahrzeuge()
        {
            if (SelG > Spiele.Count - 1)
                SelG = Spiele.Count - 1;
            return Spiele[SelG].Fahrzeuge;
        }
        public Fahrzeug GetCurrentFahrzeug()
        {
            return GetCurrentFahrzeuge()[Sel];
        }

        public void Add()
        {
            if ((Sel + 1) <= GetCurrentFahrzeuge().Count - 1)
                Sel++;
        }

        public void Sub()
        {
            if ((Sel - 1) >= 0)
                Sel--;
        }

        public void Out(string text)
        {
            Text += text + "\n";
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

        public void ReloadXML()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().Single(file => file.EndsWith("spiel.xml"))))
                Spiele = XMLHelper.LoadXML(stream);
            foreach (XML.Spiel spiel in Spiele)
            {
                spiel.ConvertFahreuge();
            }
            startTime = DateTime.Now;
        }

        public void SetCurrentFahrzeug(Point p)
        {
            int x = (int)Math.Floor(p.X / Mod);
            int y = (int)Math.Floor(p.Y / Mod);
            int i = 0;
            foreach (Fahrzeug f in GetCurrentFahrzeuge())
            {
                foreach (Point pf in f.GetPosition())
                {
                    if (pf.X == x && pf.Y == y)
                    {
                        Sel = i;
                        //Console.WriteLine("Sel: " + Sel);
                        return;
                    }
                }
                i++;
            }
        }

        public void Move(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    if (GetCurrentFahrzeug().Direction == "u") Backwards();
                    if (GetCurrentFahrzeug().Direction == "o") Forwards();
                    break;
                case Direction.Down:
                    if (GetCurrentFahrzeug().Direction == "o") Backwards();
                    if (GetCurrentFahrzeug().Direction == "u") Forwards();
                    break;
                case Direction.Right:
                    if (GetCurrentFahrzeug().Direction == "r") Forwards();
                    if (GetCurrentFahrzeug().Direction == "l") Backwards();
                    break;
                case Direction.Left:
                    if (GetCurrentFahrzeug().Direction == "l") Forwards();
                    if (GetCurrentFahrzeug().Direction == "r") Backwards();
                    break;
                default:
                    Console.WriteLine("Move error");
                    break;
            }
        }
    }
}
