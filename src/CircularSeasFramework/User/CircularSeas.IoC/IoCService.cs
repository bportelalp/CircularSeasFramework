using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircularSeas.Adapters;
using CircularSeas.Infrastructure.DB.Context;
using CircularSeas.Infrastructure.GenPDF;
using CircularSeas.Infrastructure.Logger;
using CircularSeas.Infrastructure.PrusaSlicerCLI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace CircularSeas.IoC
{
    public static class IoCService
    {
        /// <summary>
        /// Inyecta las dependencias de la infraestructura en la aplicación
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="rootPath"></param>
        public static void ConfigureIoC(this IServiceCollection services, IConfiguration configuration, string rootPath)
        {
            // Contexto de base de datos e infraestructura para acceso a base de datos
            services.AddDbContext<CircularSeasContext>(options => 
                                options.UseSqlServer(configuration.GetConnectionString("CircularSeasDBConnection"), 
                                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            services.AddScoped<Adapters.IDbService,CircularSeas.Infrastructure.DB.DbService>();

            //Custom loger
            services.AddSingleton<ILog, Log>(x => new Log(rootPath));

            // Infraestructura de generación de PDF con QR
            services.AddScoped<IGenPDF, PdfGenerator>();

            // Infraestructura de acceso a programa de slicing, con ruta especificada en appsettings
            services.AddSingleton<ISlicerCLI, PrusaSlicerCLI>(p => new PrusaSlicerCLI(p.GetRequiredService<ILog>(), configuration.GetSection("AppSettings").GetValue<string>("prusaSlicerPath")));
        }
    
    }
}
