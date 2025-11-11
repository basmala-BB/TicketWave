using TicketWave.Models;

namespace TicketWave.ViewModel
{
    public class MovieVM
    {
        public IEnumerable<Actors> actors { get; set; } = new List<Actors>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Cinema> cinemas { get; set; } = new List<Cinema>();
        public Movie movie { get; set; } = new Movie();
    }

}
