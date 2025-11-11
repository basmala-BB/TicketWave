namespace TicketWave.ViewModel
{
    public record FilterMovieVM(
       string? Title,
       int? CategoryId,
       int? CinemaId,
       int? ActorId,
       bool IsUpcoming,
       bool IsFeatured
   );
}
