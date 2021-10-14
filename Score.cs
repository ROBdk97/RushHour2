using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RushHour2
{
    public class Score : BaseModel
    {
        private int lvl;
        private string username;
        private DateTime date;
        private int moves;
        private TimeSpan time;


        public int Lvl { get => lvl; set { lvl = value; OnPropertyChanged(); } }
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public DateTime Date { get => date; set { date = value; OnPropertyChanged(); } }
        public int Moves { get => moves; set { moves = value; OnPropertyChanged(); } }
        [XmlIgnore]
        public TimeSpan Time { get => time; set { time = value; OnPropertyChanged(); } }
        [XmlElement]
        public long TimeSp { get => Time.Ticks; set { Time = TimeSpan.FromTicks(value); } }
    }
}
