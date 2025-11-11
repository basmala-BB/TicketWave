using System.ComponentModel.DataAnnotations.Schema;
using TicketWave.Areas.Admin.Controllers;

namespace TicketWave.Models
{
    [Table("movies")]
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }

        public DateTime DateTime { get; set; }
        public bool status { get; set; }
        public string MainImg { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;

        public List<MovieSubImage> movieSubImages { get; set; } = new List<MovieSubImage>();
        public ICollection<MovieActor> MovieActor { get; set; } = new List<MovieActor>();
    }

}
