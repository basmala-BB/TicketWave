using System.ComponentModel.DataAnnotations;

namespace TicketWave.ViewModel
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOrEmail { get; set; } = String.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RemeberMe { get; set; }
    }
}
