using TicketWave;
using TicketWave.Configurations;
using TicketWave.Models;
using TicketWave.Utitlies.DBInitilizer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.RegisterConfig(connectionString);
        builder.Services.RegisterMapsterConfig();

        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetService<IDBInitializer>();
        service!.Initialize();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Admin}/{controller=Admin}/{action=Index}/{id?}");

        app.Run();
    }
}




