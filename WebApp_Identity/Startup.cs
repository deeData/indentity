using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp_Identity.Authorization;

namespace WebApp_Identity
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
            //adding/registering cookie handler for authentication- will encrypt and serialize for us
            //to use "MyCookieAuth" cookie scheme for authentication [services.AddAuthentication("MyCookieAuth")]
            services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options => {
                //asp will know which cookie has authentication
                options.Cookie.Name = "MyCookieAuth";
                //specify location of login page- as to redirect for authentication before accessing a page - IF not in default location
                //default location
                //options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                //can specify lifetime of cookie session. Closing the browser will automatically delete the cookie (dependent of browser session-interaction btw browser and server).
                //options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
                //however, can also add a persistent cookie, which will maintain even when browser is closed
                options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
            });

            //create authorization policies
            services.AddAuthorization(options => {
                options.AddPolicy("HrAccess", policy => policy.RequireClaim("Department","HR"));
            });
            services.AddAuthorization(options => {
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
            });
            services.AddAuthorization(options => {
                options.AddPolicy("HRManagerOnly", policy => policy
                .RequireClaim("Department", "HR")
                .RequireClaim("Manager")
                //hardcoded as 3 months
                .Requirements.Add(new HRManagerProbationRequirement(3)));
            });

            services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

            services.AddRazorPages();
            services.AddMvc().AddRazorRuntimeCompilation();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
    
            //always add for user logins
            app.UseAuthentication();
            //all pages by default have authorization metadata
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
