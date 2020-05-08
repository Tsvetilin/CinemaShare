using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Database and Identity
            services.AddDbContext<CinemaDbContext>(options => {
                options.UseMySQL( Configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
            });
            services.AddDefaultIdentity<CinemaUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<CinemaRole>()
                .AddEntityFrameworkStores<CinemaDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            
            //Configuration
            services.AddSingleton(this.Configuration);

            //Add service providers from business layer
            services.AddTransient<Business.IEmailSender>(x=>new EmailSender(Configuration.GetSection("EmailSender").Value));
            services.AddTransient<ICinemaBusiness, CinemaBusiness>();
            services.AddTransient<IFilmBusiness, FilmBusiness>();
            services.AddTransient<IFilmDataBusiness, FilmDataBusiness>();
            services.AddTransient<IFilmProjectionBusiness, FilmProjectionBusiness>();
            services.AddTransient<IFilmReviewBusiness, FilmReviewBusiness>();
            services.AddTransient<IProjectionTicketBusiness, ProjectionTicketBusiness>();

            services.AddTransient<IFilmFetchAPI, FilmFetchAPI>();

            services.AddTransient<IMapper, Mapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<CinemaDbContext>();
                context.Database.EnsureCreated();
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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
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
