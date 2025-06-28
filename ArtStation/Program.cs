
using ArtStation.Core;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Services.Contract;
using ArtStation.Extensions;
using ArtStation.Helper;
using ArtStation.Middlewares;
using ArtStation.Repository;
using ArtStation.Repository.Data;
using ArtStation.Repository.Repository;
using ArtStation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ArtStation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Services
            builder.Services.AddControllers()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization()
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                //})
                ;
            builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, ValidationAuthorization>();
            ;
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


            //Redis Connection
            builder.Services.AddSingleton<IConnectionMultiplexer>((provider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));
            builder.Services.AddScoped(typeof(IFavouriteRepository), typeof(FavouriteRepository));
            builder.Services.AddScoped(typeof(IAddressRepository), typeof(AddressRepository));
            builder.Services.AddScoped(typeof(IBannerRepository), typeof(BannerRepository));
            builder.Services.AddScoped(typeof(ICartService), typeof(CartService));

            builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));

            builder.Services.AddSwaggerServices();

            #region Localization

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ar" }; // add your languages
                options.SetDefaultCulture("en");
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            });

            #endregion

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            #endregion
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
            builder.Services.AddTransient<ISMSService, SMSService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton(typeof(IVerificationCodeService), typeof(VerificationCodeService));


            // Error Message Appear
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .SelectMany(ms => ms.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var result = new
                    {

                        message = string.Join(" | ", errors)
                    };

                    return new BadRequestObjectResult(result);
                };
            });


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<ArtStationDbContext>();
                await AppSeeding.SeedShippingCost(dbcontext);
            }

            HandlerPhoto.Initialize(app.Environment);
            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            app.Run();
        }
    }
}
