using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace RushHour2
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string text = "";
        private int sel = 0;
        private int selg = 0;
        private int mod = 70;
        private int gridSize = 6;
        private List<XML.Spiel> spiele;
        private IMainWindow main;
        private int moves = 0;

        public ViewModel(IMainWindow _main)
        {
            main = _main;
            Spiele = new List<XML.Spiel>();
            ReloadXML();
        }

        public void FahrzeugeAusString()
        {
            XML.Spiel spiel = new XML.Spiel();
            spiel.Fahrzeug = new List<XML.FahrzeugX>();
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
                foreach (Point p in meinFahrzeug.getPos())
                {
                    if (p.X == 5 && p.Y == 2) return true;
                }
            }
            return false;
        }

        internal void ImportXML()
        {
            var assembly = Assembly.GetExecutingAssembly();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "c:\\";
            fileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            fileDialog.FilterIndex = 2;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == true)
            {
                Spiele = XMLHelper.LoadXML(fileDialog.OpenFile());
                foreach (XML.Spiel spiel in Spiele)
                {
                    spiel.ConvertFahreuge();
                }
            }
        }

        public void forwards()
        {
            Out("Try Forward:");
            if (isMoveAllowed(GetCurrentFahrzeug(), false))
            {
                Out(pointToString(GetCurrentFahrzeug().getPos(), Sel));
                GetCurrentFahrzeug().forward();
                Out(pointToString(GetCurrentFahrzeug().getPos(), Sel));
                Text += "---------------\n";
                moves++;
            }
        }
        public void backwards()
        {
            Out("Try Backwards:");
            if (isMoveAllowed(GetCurrentFahrzeug(), true))
            {
                Out(pointToString(GetCurrentFahrzeug().getPos(), Sel));
                GetCurrentFahrzeug().backwards();
                Out(pointToString(GetCurrentFahrzeug().getPos(), Sel));
                Text += "---------------\n";
                moves++;
            }
        }

        public bool isMoveAllowed(Fahrzeug fahrzeug, bool forwards)
        {
            foreach (Point p in fahrzeug.getPos())
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

        private string pointToString(Point[] koor, int i)
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
            //Spiele[0].Fahrzeuge.Add(new Fahrzeug(new Point(0,0),1,"o"));
        }

        public void SetCurrentFahrzeug(Point p)
        {
            int x = (int)Math.Floor(p.X / Mod);
            int y = (int)Math.Floor(p.Y / Mod);
            int i = 0;
            foreach (Fahrzeug f in GetCurrentFahrzeuge())
            {
                foreach (Point pf in f.getPos())
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

        public void Move(direction dir)
        {
            switch (dir)
            {
                case direction.up:
                    if (GetCurrentFahrzeug().Direction == "u") backwards();
                    if (GetCurrentFahrzeug().Direction == "o") forwards();
                    break;
                case direction.down:
                    if (GetCurrentFahrzeug().Direction == "o") backwards();
                    if (GetCurrentFahrzeug().Direction == "u") forwards();
                    break;
                case direction.right:
                    if (GetCurrentFahrzeug().Direction == "r") forwards();
                    if (GetCurrentFahrzeug().Direction == "l") backwards();
                    break;
                case direction.left:
                    if (GetCurrentFahrzeug().Direction == "l") forwards();
                    if (GetCurrentFahrzeug().Direction == "r") backwards();
                    break;
                default:
                    Console.WriteLine("Move error");
                    break;
            }
        }


        public string Text { get => text; set { text = value; OnPropertyChanged(nameof(Text)); } }
        public int Sel { get => sel; set { sel = value; OnPropertyChanged(nameof(Sel)); } }
        public int Moves { get => moves; set { moves = value; OnPropertyChanged(nameof(Moves)); } }
        public int SelG { get => selg; set { selg = value; OnPropertyChanged(nameof(SelG)); } }
        public List<XML.Spiel> Spiele { get => spiele; set { spiele = value; OnPropertyChanged(nameof(Spiele)); } }
        public int Mod { get => mod; set { mod = value; OnPropertyChanged(nameof(Mod)); main.aktuallisiereSpielfeld(true); } }
        public int GridSize { get => gridSize; set { gridSize = value; OnPropertyChanged(nameof(GridSize)); main.aktuallisiereSpielfeld(); } }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public enum direction
        {
            up,
            down,
            left,
            right
        }
    }
}
