using TicketWave.Models;
using Microsoft.EntityFrameworkCore;

namespace TicketWave.Models;

public class ApplicationdbContext : DbContext
{
    public DbSet<Category> categories { get; set; }
    public DbSet<Movie> movies { get; set; }
    public DbSet<Actors> actors { get; set; }
    public DbSet<Cinema> cinemas { get; set; }
    public DbSet<MovieSubImages> movieSubImages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial catalog = Filmpass;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

}


