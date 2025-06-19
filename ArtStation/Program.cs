
using ArtStation.Core.Entities.Identity;
using ArtStation.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ArtStation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Services
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ArtStationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("default")));
            builder.Services.AddIdentity<AppUser, AppRole>(
                options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<ArtStationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ar" }; // add your languages
                options.SetDefaultCulture("en");
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion

            var app = builder.Build();

            var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
