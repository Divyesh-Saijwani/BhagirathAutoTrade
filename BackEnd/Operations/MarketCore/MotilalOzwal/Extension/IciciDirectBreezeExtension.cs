using MotilalOzwal.Models;
using MarketData.Services;
//using MotilalOzwal.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MotilalOzwal.Extension
{
    public static class MotilalOzwalExtension
    {
        public static void AddMotilalOzwalMarketData(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //serviceCollection.AddTransient<IMarketData, MarketDataService>();
        }

        //public static void AddMotilalOzwalTrader(this IServiceCollection serviceCollection, MotilalOzwalConfiguration appConfiguration)
        //{
        //    serviceCollection.AddTransient<IMarketData, MarketDataService>();
        //}
    }
}
