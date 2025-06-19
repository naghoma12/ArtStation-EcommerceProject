using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtStation_Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Services    
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ArtStationDbContext>(
              options => options.UseSqlServer(builder.Configuration.GetConnectionString("default")));
            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
