using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RushHour2.XML
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
        public List<Spiel> Spiel { get; set; }
    }

}

namespace RushHour2
{
    public static class XMLHelper
    {
        public static List<XML.Spiel> LoadXML(Stream path)
        {
            XML.RushHourXML xml = new XML.RushHourXML();
            XmlSerializer serializer = new XmlSerializer(typeof(XML.RushHourXML));
            using (StreamReader reader = new StreamReader(path))
            {
                xml = (XML.RushHourXML)serializer.Deserialize(reader);
            }
            return xml.Spiel;
        }
    }
}
