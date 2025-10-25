using System.ComponentModel.DataAnnotations;

namespace TicketWave.ViewModel
{
    public class UpdateCinemaVM
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? ImagePath { get; set; }
        public IFormFile? NewImagePath { get; set; } 
    }
}
