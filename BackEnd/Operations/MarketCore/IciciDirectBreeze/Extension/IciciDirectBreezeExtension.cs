using IciciDirectBreeze.Models;
using MarketData.Services;
using IciciDirectBreeze.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IciciDirectBreeze.Extension
{
    public static class IciciDirectBreezeExtension
    {
        public static void AddIciciDirectBreezeMarketData(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IMarketData, MarketDataService>();

            var iciciConfig = new IciciDirectBreezeConfiguration();
            configuration.GetSection("IciciDirectBreezeConfig").Bind(iciciConfig);

            serviceCollection.AddSingleton(iciciConfig);
        }

        //public static void AddIciciDirectBreezeTrader(this IServiceCollection serviceCollection, IciciDirectBreezeConfiguration appConfiguration)
        //{
        //    serviceCollection.AddTransient<IMarketData, MarketDataService>();
        //}
    }
}
