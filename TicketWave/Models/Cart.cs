namespace TicketWave.Models
{
    public class Cart
    {

        public int MovieId { get; set; }
        public Movie movie { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
