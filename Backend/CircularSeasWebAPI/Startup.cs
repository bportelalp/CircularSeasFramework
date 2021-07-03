using CircularSeasWebAPI.Helpers;
using CircularSeasWebAPI.Models;
using CircularSeasWebAPI.SlicerEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CircularSeasWebAPI {
    public class Startup {
        
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
            AddSwagger(services);

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddDbContext<CircularSeasContext>(options => options.UseSqlServer(appSettings.DBConnectionString));

            //Servicio Singleton to LOG
            services.AddSingleton<Log>();
            services.AddSingleton<Tools>();
            services.AddScoped<ISlicerCLI, PrusaSlicerCLI>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Foo API V1");
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        private void AddSwagger(IServiceCollection services) {
            services.AddSwaggerGen(options => {
                var groupName = "v1";
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                var commentsFile = Path.Combine(baseDirectory, commentsFileName);
                options.SwaggerDoc(groupName, new OpenApiInfo {
                    Title = $"CloudService API {groupName}",
                    Version = groupName,
                    Description = "CircularSeas CloudService API",
                    Contact = new OpenApiContact {
                        Name = "CircularSeas",
                        Email = string.Empty,
                        Url = new Uri("https://circularseas.com"),
                    }
                });
                //options.IncludeXmlComments(commentsFile, true);
            });
        }
    }
}
