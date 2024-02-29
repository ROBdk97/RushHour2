using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RushHour2
{
    public static class XMLHelper
    {
        public static List<Spiel> LoadXML(Stream path)
        {
            XmlSerializer serializer = new(typeof(RushHourXML));
            using StreamReader reader = new(path);
            RushHourXML xml = (RushHourXML)serializer.Deserialize(reader);
            return xml.Spiele;
        }

        internal static void SaveXML(List<Spiel> spiele, string fileName)
        {
            RushHourXML xml = new() { Spiele = spiele };
            XmlSerializer serializer = new(typeof(RushHourXML));
            using StreamWriter writer = new(fileName);
            serializer.Serialize(writer, xml);
        }
    }
}
