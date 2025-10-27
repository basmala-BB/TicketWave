using TicketWave.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TicketWave.ViewModels;

namespace TicketWave.Models;

public class ApplicationdbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationdbContext()
    {
    }

    public ApplicationdbContext(DbContextOptions<ApplicationdbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> categories { get; set; }
    public DbSet<Movie> movies { get; set; }
    public DbSet<Actors> actors { get; set; }
    public DbSet<Movie> cinemas { get; set; }
    public DbSet<MovieSubImages> movieSubImages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial catalog = Filmpass;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

public DbSet<TicketWave.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;

}


