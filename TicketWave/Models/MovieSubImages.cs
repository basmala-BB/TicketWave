using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketWave.Models
{
    public class MovieSubImage
    {
        [Key]
        public int Id { get; set; }

        public string ImagePath { get; set; } = string.Empty;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}

