using Microsoft.EntityFrameworkCore;

namespace TicketWave.Models
{
    [PrimaryKey (nameof(MovieId), nameof(ImagePath))]
    public class MovieSubImages
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
