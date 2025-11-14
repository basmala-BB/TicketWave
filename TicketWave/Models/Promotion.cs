using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TicketWave.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int moviId { get; set; }
        [ValidateNever]
        public Movie movie { get; set; }
        public DateTime PublishAt { get; set; } = DateTime.UtcNow;
        public DateTime ValidTo { get; set; }
        public bool IsValid { get; set; } = true;

        public string Code { get; set; }
        public decimal Discount { get; set; }
    }
}
