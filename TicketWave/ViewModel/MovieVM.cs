using TicketWave.Models;

namespace TicketWave.ViewModel
{
    public class MovieVM
    {
        public Movie Movie { get; set; } = new Movie();
        public IEnumerable<Category> categories { get; set; } = new List<Category>();
        public IEnumerable<Cinema> cinemas { get; set; } = new List<Cinema>();
        public IEnumerable<Actors> actors { get; set; } = new List<Actors>();

        public List<int> ActorsIds { get; set; } = new List<int>();
    }

}
