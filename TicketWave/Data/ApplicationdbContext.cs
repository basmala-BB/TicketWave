using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketWave.DataAccess.EntityConfigurations;
using TicketWave.Models;
using TicketWave.ViewModel;
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
    public DbSet<Cinema> cinemas { get; set; }
    public DbSet<MovieSubImage> movieSubImages { get; set; }
    public DbSet<ApplicationUserOTP> applicationUserOTPs { get; set; }
    public DbSet<Cart> carts { get; set; }
    public DbSet<Promotion> promotions { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer("Data Source=.;Initial catalog = Filmpass;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

public DbSet<TicketWave.ViewModel.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;

public DbSet<TicketWave.ViewModel.NewPasswordVM> NewPasswordVM { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // المفتاح المركب ل Cart
        modelBuilder.ApplyConfiguration(new CartEntityTypeConfiguration());

        // المفتاح المركب للجدول الوسيط
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        // علاقة Movie ↔ MovieActor
        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActor) // خلي اسم Collection في Movie: MovieActors
            .HasForeignKey(ma => ma.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // علاقة Actor ↔ MovieActor
        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .OnDelete(DeleteBehavior.Cascade);
    }


}


