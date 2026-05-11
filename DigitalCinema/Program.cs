using DigitalCinema.Models;
using DigitalCinema.Utility;
using DigitalCinema.Utility.DbInitializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace DigitalCinema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new
              InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true; 
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = false;


            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Identity/Account/Login";
                option.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });


            builder.Services.AddScoped<IRepository<Ticket>, Repository<Ticket>>();
            builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();
            builder.Services.AddScoped<IRepository<OrderItem>, Repository<OrderItem>>();
            builder.Services.AddScoped<IRepository<Booking>, Repository<Booking>>();
            builder.Services.AddScoped<IRepository<Hall>, Repository<Hall>>();
            builder.Services.AddScoped<IRepository<ShowSeat>, Repository<ShowSeat>>();
            builder.Services.AddScoped<IRepository<Seat>, Repository<Seat>>();
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IRepository<Show>, Repository<Show>>();
            builder.Services.AddScoped<IRepository<ShowMovieHall>, Repository<ShowMovieHall>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            builder.Services.AddScoped<IRepository<SupImg>, Repository<SupImg>>();
            builder.Services.AddScoped<IRepository<Post>, Repository<Post>>();
            builder.Services.AddScoped<IRepository<PostComment>, Repository<PostComment>>();
            builder.Services.AddScoped<IRepository<PostLike>, Repository<PostLike>>();
            builder.Services.AddScoped<IMovieSubImgRepository, MovieSubImgRepository>();
            builder.Services.AddScoped<IRepository<ActorMovie>, Repository<ActorMovie>>();
            builder.Services.AddScoped<IImgesService, ImgesService>();
            builder.Services.AddScoped<IBockingRepository, BockingRepository>();
            builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();
          
            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDbInitializer>();
            service.Initialize();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
