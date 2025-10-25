using System.ComponentModel.DataAnnotations;

namespace TicketWave.ViewModel
{
    public class CreateCinemaVm
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool Status { get; set; }
        public IFormFile ImagePath  { get; set; } = default!;
    }
}
