using System.ComponentModel.DataAnnotations;
namespace TicketWave.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImagePath { get; set; } = "defaultImg.png";
        public bool Status { get; set; }


    }
}
