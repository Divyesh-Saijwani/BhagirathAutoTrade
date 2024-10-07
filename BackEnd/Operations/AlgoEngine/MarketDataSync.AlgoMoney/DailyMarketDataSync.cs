using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MarketDataSync.AlgoMoney
{
    public class DailyMarketDataSync
    {
        [FunctionName("DailyMarketDataSync")]
        public void Run([TimerTrigger("0 17 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
