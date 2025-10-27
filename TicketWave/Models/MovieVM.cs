namespace TicketWave.Models
{
    public class MovieVM
    {
        public Movie Movie { get; set; } = new Movie();
        public IEnumerable<Category> categories { get; set; } = new List<Category>();
        public IEnumerable<Movie> cinemas { get; set; } = new List<Movie>();
        public IEnumerable<Actors> actors { get; set; } = new List<Actors>();

        public List<int> ActorsIds { get; set; } = new List<int>();
    }

}
