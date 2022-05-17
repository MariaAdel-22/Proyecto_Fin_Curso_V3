using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Proyecto_AdoPet.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Core.Extensions;
using Amazon.S3;

namespace Proyecto_AdoPet
{
    public class Startup
    {
        public IConfiguration configuration { get; }

        public Startup(IConfiguration configuration) {

            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string urlApi = this.configuration.GetValue<string>("UrlApi:aws");

            ServiceApiAdopet service = new ServiceApiAdopet(urlApi);

            services.AddTransient<ServiceApiAdopet>(x => service);

            services.AddAWSService<IAmazonS3>();
            services.AddTransient<ServiceS3>();

            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(40);
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            services.AddControllersWithViews(op => op.EnableEndpointRouting = false);

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseMvc(routes =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<ChatHub>("/chathub");
                });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Inicio}/{action=Index}/{id?}");
            });
        }
    }
   
}
