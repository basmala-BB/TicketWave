using TicketWave.Repositories.IRepositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TicketWave.Models;

namespace TicketWave.Repositories.IRepositories
{

    public interface IMovieRepository : IRepository<Movie>
    {
        Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default);
    }
}


