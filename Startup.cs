using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZennoLab.Models;
using ZennoLab.Services;
using ZennoLab.Services.Validation;

namespace ZennoLab
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            if (Environment.IsDevelopment())
                services.AddScoped<IMetadataStorage, FileMetadataMock>();
            else {
                services.AddDbContext<FileMetadataDbContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionMsSql")));
                services.AddScoped<IMetadataStorage, FileMetadataDbContext>();
            }

            services.AddScoped<IValidationService, DatasetFileValidationService>(); // change your validation logic here
            services.AddScoped<IDatasetStorageService, DatasetStorageService>(); // change the way you store dataset
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
