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
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, string rootPath)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");

            services.AddDbContext<CircularSeasContext>(options => 
                                options.UseSqlServer(configuration.GetConnectionString("CircularSeasDBConnection"), 
                                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            services.AddScoped<Adapters.IDbService,CircularSeas.Infrastructure.DB.DbService>();

            services.AddSingleton<ILog, Log>(x => new Log(rootPath));
            services.AddScoped<IGenPDF, PdfGenerator>();
            services.AddScoped<ISlicerCLI, PrusaSlicerCLI>(p => new PrusaSlicerCLI(p.GetRequiredService<ILog>(), configuration.GetSection("AppSettings").GetValue<string>("prusaSlicerPath")));
        }
    
    }
}
