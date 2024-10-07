using MarketData.Services;
using Microsoft.AspNetCore.SignalR;

namespace AlgoBhagirath.Server
{
    public class StockHub : Hub
    {
        private readonly IMarketData _marketData;
        public StockHub(IMarketData marketData)
        {
            this._marketData=marketData;
        }
        public async Task SendStockData(List<string> stockSymbols)
        {
            //// Use the Breeze API to fetch stock data
            //var marketData= 
            //var tasks = stockSymbols.Select(symbol => _marketData.GetLiveStockDataAsync(symbol)).ToList();
            //var liveStockData = await Task.WhenAll(tasks);

            // Send data to the connected clients
            //await Clients.All.SendAsync("ReceiveStockData", liveStockData);
        }
    }
}
