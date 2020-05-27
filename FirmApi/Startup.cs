using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FirmApi;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebstoreApi;

namespace GenericApi
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
            services.AddControllers(opt => { opt.Filters.Add(typeof(ValidateModelStateAttribute)); }
                )
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddControllersAsServices();
            var connectionString = Configuration.GetConnectionString("GDb");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<FirmDbContext>(options =>
            {
                options.UseMySql(connectionString);
                /*
                ~M.S~ This needs to be deleted at some point
                ||||||||||||||||||||||||||||||||||||||
                VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV*/
                options.EnableSensitiveDataLogging();
            });
        }
        
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<AutofacModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthorization();
            
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FirmDbContext>();
            context.Database.Migrate();
            
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireCors("CorsPolicy"); });
        }
    }
}