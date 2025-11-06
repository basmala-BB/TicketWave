using TicketWave.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TicketWave.ViewModels;
using TicketWave.ViewModel;

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
    public DbSet<Cinema> cinemas { get; set; }
    public DbSet<MovieSubImage> movieSubImages { get; set; }
    public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial catalog = Filmpass;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

public DbSet<TicketWave.ViewModel.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;

public DbSet<TicketWave.ViewModel.NewPasswordVM> NewPasswordVM { get; set; } = default!;


}


