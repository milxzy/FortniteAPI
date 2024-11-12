namespace FortniteAPI.Models
{
    public class Team
    {
        public int ID { get; set; }
        public string? TeamName { get; set; }
        public ICollection<FortnitePlayer> Players { get; set; } = new List<FortnitePlayer>();
    }
}
