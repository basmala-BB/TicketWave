using TicketWave.Repositories.IRepositories;
using TicketWave.Models;

namespace TicketWave.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationdbContext _context;

        public MovieRepository(ApplicationdbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(movies, cancellationToken);
        }
    }
}
