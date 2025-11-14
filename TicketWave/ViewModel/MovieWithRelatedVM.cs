using TicketWave.Models;

namespace TicketWave.ViewModel
{
    public class MovieWithRelatedVM
    {
        public Movie movie { get; set; } = default!;
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();
    }
}
