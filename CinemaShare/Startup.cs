using Data;
using Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Business;
using CinemaShare.Common.Mapping;

namespace CinemaShare
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Database and Identity
            services.AddDbContext<CinemaDbContext>(options => {
                options.UseMySQL( Configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
            });
            services.AddDefaultIdentity<CinemaUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<CinemaRole>()
                .AddEntityFrameworkStores<CinemaDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            
            // Configuration
            services.AddSingleton(this.Configuration);

            // Service providers for business layer
            services.AddTransient<ICinemaBusiness, CinemaBusiness>();
            services.AddTransient<IFilmBusiness, FilmBusiness>();
            services.AddTransient<IFilmDataBusiness, FilmDataBusiness>();
            services.AddTransient<IFilmProjectionBusiness, FilmProjectionBusiness>();
            services.AddTransient<IFilmReviewBusiness, FilmReviewBusiness>();
            services.AddTransient<IProjectionTicketBusiness, ProjectionTicketBusiness>();

            services.AddTransient<IFilmFetchAPI>(x=>
                new FilmFetchAPI(Configuration.GetSection("OMDb")["APIKey"]));
            services.AddTransient<ICloudinaryAPI>(x =>
                new CloudinaryAPI(Configuration.GetSection("Cloudinary")["CloudName"],
                                  Configuration.GetSection("Cloudinary")["APIKey"],
                                  Configuration.GetSection("Cloudinary")["APISecret"]));
            services.AddTransient<IEmailSender>(x=>
                new EmailSender(Configuration.GetSection("SendGridEmailSender")["APIKey"],
                                Configuration.GetSection("SendGridEmailSender")["Sender"],
                                Configuration.GetSection("SendGridEmailSender")["SenderName"]));

            services.AddTransient<IMapper, Mapper>();
        }

        // Configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<CinemaDbContext>();
                new CinemaDbContextSeeder(context).SeedAsync().GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithRedirects("/Home/StatusError?code={0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "filmsList",
                    pattern: "{controller=Films}/{action=Index}/{sort=All}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
