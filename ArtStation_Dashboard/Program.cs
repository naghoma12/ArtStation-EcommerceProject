using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using ArtStation.Repository;
using ArtStation.Repository.Data;
using ArtStation.Repository.Repository;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ArtStation_Dashboard.Helper;
using Microsoft.AspNetCore.Mvc.Razor;
using ArtStation.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using ArtStation.Core.Services.Contract;
using ArtStation.Services;
using StackExchange.Redis;
using ArtStation.Core.Entities;
using Microsoft.AspNetCore.Http.Features;

namespace ArtStation_Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            #region Services    
            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization();


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
            builder.Services.AddSingleton<IConnectionMultiplexer>((provider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            builder.Services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));
            builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            builder.Services.AddScoped(typeof(ICartRepository), typeof(CartRepository));
            builder.Services.AddScoped(typeof(ICartService), typeof(CartService));
            builder.Services.AddScoped(typeof(IAddressRepository), typeof(AddressRepository));
            builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            builder.Services.AddScoped(typeof(IBannerRepository), typeof(BannerRepository));
            builder.Services.AddScoped(typeof(IProductTypeRepository<ProductSize>), typeof(SizeRepository));
            builder.Services.AddScoped(typeof(IProductTypeRepository<ProductColor>), typeof(ColorRepository));
            builder.Services.AddScoped(typeof(IProductTypeRepository<ProductFlavour>), typeof(FlavourRepository));
            builder.Services.AddScoped(typeof(IProductTypeRepository<ProductForWhom>), typeof(ForWhomRepository));
            builder.Services.AddScoped(typeof(IProductTypeRepository<ProductPhotos>), typeof(PhotoRepository));
            builder.Services.AddScoped(typeof(IForWhomRepository), typeof(ForWhomRepository));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(UserHelper), typeof(UserHelper));
            builder.Services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // If using file uploads
                x.ValueLengthLimit = int.MaxValue;
            });
            #region Localization


            builder.Services.AddLocalization(options => options.ResourcesPath = "Resource");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ar" };
                options.SetDefaultCulture("en")
                       .AddSupportedCultures(supportedCultures)
                       .AddSupportedUICultures(supportedCultures);

                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });
            #endregion


            #region Cookie Configurations
            builder.Services.AddAuthentication("Cookies")
                       .AddCookie(options =>
                       {
                           options.LoginPath = "/Auth/Signin";
                           // options.AccessDeniedPath = "/Account/AccessDenied"; 
                           options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                       });
            builder.Services.ConfigureApplicationCookie(conf =>
            {
                conf.LoginPath = "/Auth/Signin";
            });
            builder.Services.AddAuthorization();
            #endregion
            #endregion
            builder.Services.AddHttpClient<IPaymentService, PaymentService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

           
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
