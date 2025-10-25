namespace TicketWave.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public  decimal price { get; set; } 
        public DateTime DateTime { get; set; }
        public bool status { get; set; }
        public string MainImg { get; set; } = string.Empty;

        public int CategoryId {  get; set; }
        public Category Category { get; set; } = null!;
        public int CinemaId { get; set; } 
        public Cinema Cinema { get; set; } = null!;
        public List<Actors> Actors { get; set; } = new List<Actors>();


    }
}
