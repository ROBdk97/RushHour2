using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RushHour2
{
    [XmlRoot(ElementName = "Fahrzeug")]
    public class FahrzeugX
    {
        public FahrzeugX()
        {
        }

        public FahrzeugX(double x, double y, int leange, string ausrichtung)
        {
            X = x;
            Y = y;
            Laenge = leange;
            Ausrichtung = ausrichtung;
        }

        [XmlElement(ElementName = "X")]
        public double X { get; set; }

        [XmlElement(ElementName = "Y")]
        public double Y { get; set; }

        [XmlElement(ElementName = "Laenge")]
        public int Laenge { get; set; }

        [XmlElement(ElementName = "Ausrichtung")]
        public string Ausrichtung { get; set; }
    }

    [XmlRoot(ElementName = "spiel")]
    public class Spiel
    {
        [XmlElement(ElementName = "Fahrzeug")]
        public List<FahrzeugX> Fahrzeug { get; set; }

        [XmlIgnore]
        public List<Fahrzeug> Fahrzeuge { get; set; }

        public Spiel()
        {
        }

        public void ConvertFahreuge()
        {
            Fahrzeuge = new List<Fahrzeug>();
            foreach (FahrzeugX x in Fahrzeug)
            {
                Fahrzeuge.Add(new Fahrzeug(new System.Windows.Point(x.X, x.Y), x.Laenge, x.Ausrichtung));
            }
        }

        public Spiel(List<Fahrzeug> fahrzeuge)
        {
            Fahrzeuge = new List<Fahrzeug>();
            foreach (Fahrzeug f in fahrzeuge)
            {
                this.Fahrzeuge.Add(f);
            }
        }
    }

    [XmlRoot(ElementName = "RushHour")]
    public class RushHourXML
    {
        [XmlElement(ElementName = "spiel")]
        public List<Spiel> Spiele { get; set; }
    }
}
