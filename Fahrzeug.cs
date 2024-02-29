using System.Windows;

namespace RushHour2
{
    public class Fahrzeug
    {
        private Point[] koord;
        private readonly string direction;

        public Fahrzeug(Point pos, int laenge, string _string)
        {
            direction = _string;
            koord = new Point[laenge];
            for (int i = 0; i < laenge; i++)
            {
                switch (Direction)
                {
                    case "o":
                        koord[i] = new Point(pos.X, pos.Y + i);
                        break;
                    case "u":
                        koord[i] = new Point(pos.X, pos.Y - i);
                        break;
                    case "r":
                        koord[i] = new Point(pos.X - i, pos.Y);
                        break;
                    case "l":
                        koord[i] = new Point(pos.X + i, pos.Y);
                        break;
                }
            }
        }
        public Point[] GetPosition()
        {
            return koord;
        }
        public void Forwards()
        {
            for (int i = 0; i < koord.Length; i++)
            {
                switch (Direction)
                {
                    case "o":
                        koord[i].Y--;
                        break;
                    case "u":
                        koord[i].Y++;
                        break;
                    case "r":
                        koord[i].X++;
                        break;
                    case "l":
                        koord[i].X--;
                        break;
                }
            }
        }

        public void Backwards()
        {
            for (int i = 0; i < koord.Length; i++)
            {
                switch (Direction)
                {
                    case "o":
                        koord[i].Y++;
                        break;
                    case "u":
                        koord[i].Y--;
                        break;
                    case "r":
                        koord[i].X--;
                        break;
                    case "l":
                        koord[i].X++;
                        break;
                }
            }
        }

        public bool isField(Point point)
        {
            foreach (Point koordPoint in koord)
            {
                if (point == koordPoint)
                    return true;
            }
            return false;
        }

        public string GetImageUri()
        {
            string type = koord.Length <= 2 ? "pkw_" : "lkw_";
            switch (Direction)
            {
                case "o":
                    return type + "down.bmp";
                case "u":
                    return type + "up.bmp";
                case "r":
                    return type + "left.bmp";
                case "l":
                    return type + "right.bmp";
                default:
                    return string.Empty;
            }
        }

        public double Width()
        {
            if (Direction == "o" || Direction == "u") return 1;
            else
            {
                return koord.Length;
            }
        }
        public double Heigth()
        {
            if (Direction == "r" || Direction == "l") return 1;
            else
            {
                return koord.Length;
            }
        }

        public override string ToString()
        {
            string s = "Fahrzeug:\nFahrtrichtung:" + Direction + "\n";
            foreach (Point point in koord)
            {
                s += point.X + "," + point.Y + "\n";
            }
            //s.Length -= 2;
            return s;
        }

        public string Direction { get => direction; }
    }
}
