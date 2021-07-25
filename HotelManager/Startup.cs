using HotelManager.Data;
using HotelManager.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Owin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Owin;



namespace HotelManager
{
    public class Startup
    {
        // Constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }


        public IConfiguration Configuration { get; }
        

        /* This method gets called by the runtime. 
           Use this method to add services to the container. */
        public void ConfigureServices(IServiceCollection services)
            {
                services.AddDbContext<ApplicationDbContext>
                (
                    options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                );

                services.AddIdentity<UserEmployee, IdentityRole>
                (
                    options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 5;
                        options.Password.RequireLowercase = false;
                        options.Password.RequiredUniqueChars = 0;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                    }
                )
                .AddDefaultUI()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

                // services.AddTransient<IEventsService, EventsService>();
                //services.AddTransient<IOrdersService, OrdersService>();

                services.AddControllersWithViews();
                services.AddRazorPages();

            }

            /* This method gets called by the runtime. 
               Use this method to configure the HTTP request pipeline. */
            public  void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<UserEmployee> userManager, RoleManager<IdentityRole> roleManager )
            {
               // AutoMapperConfig.ConfigureMapping();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseAuthorization();
                DefaultAdminAndRoles.SeedData(userManager, roleManager);
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });
            }
        

        }
    }
