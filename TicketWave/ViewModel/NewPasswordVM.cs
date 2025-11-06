using System.ComponentModel.DataAnnotations;

namespace TicketWave.ViewModel
{
    public class NewPasswordVM
    {
        public int Id { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;

    }
}
