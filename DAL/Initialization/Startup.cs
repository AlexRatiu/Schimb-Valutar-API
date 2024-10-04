using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Schimb_Valutar_API.DAL.Repositories;
using Schimb_Valutar_API.DAL.Repositories.Interfaces;
using Schimb_Valutar_API.Services;
using Schimb_Valutar_API.Services.Interfaces;

namespace Schimb_Valutar_API.DAL.Initialization
{
    public class Startup
    {
        public static void Init(IConfiguration configuration, IServiceCollection services)
        {
            var connectionString = configuration.GetConnectionString("Develop");
            services.AddDbContextPool<CurrencyDbContext>(options => options.UseSqlServer(connectionString));
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<ICurrencyService, CurrencyService>();
            services.AddTransient<ICurrencyRateService, CurrencyRateService>();
            services.AddHttpClient<ICurrencyRateService, CurrencyRateService>(client =>
            {
                client.BaseAddress = new Uri("https://www.bnr.ro");
            });
        }
    }
}

