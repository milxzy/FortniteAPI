

namespace FortniteAPI.Models
{
    public class FortnitePlayer
    {
        public long ID { get; set; }
        public string? Name { get; set; } = "";
        
        public int Earnings { get; set; }
        public string? Server { get; set; }
        public int Age { get; set; }
        public int TeamID { get; set; }

        public Team? Team { get; set; }
        


    }
}
