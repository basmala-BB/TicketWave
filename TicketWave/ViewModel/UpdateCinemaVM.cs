namespace TicketWave.ViewModel
{
    public class UpdateCinemaVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public IFormFile? NewImagePath { get; set; }
    }
}
