using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace RushHour2
{
    public class Score : BaseModel
    {
        private int lvl;
        private string username;
        private DateTime date;
        private int moves;
        private TimeSpan time;
        private int score;


        public int Id { get; set; }
        public int Lvl { get => lvl; set { lvl = value; OnPropertyChanged(); } }
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public DateTime Date { get => date; set { date = value; OnPropertyChanged(); } }
        public int Moves { get => moves; set { moves = value; OnPropertyChanged(); } }
        [JsonIgnore]
        public TimeSpan Time
        {
            get => time;
            set { time = value; OnPropertyChanged(); }
        }
        public long TimeSp { get => Time.Ticks; set { Time = TimeSpan.FromTicks(value); OnPropertyChanged(); } }
        // only calculated by the api
        public int Points { get => score; set { score = value; OnPropertyChanged(); } }
        [JsonIgnore]
        public string DisplayText
        {
            get
            {
                return $"Punkte: {Points} \nBenötigte Zeit: {Time:mm\\:ss\\:fff} \nBenötigte Züge: {Moves}";
            }
        }


    }
}
