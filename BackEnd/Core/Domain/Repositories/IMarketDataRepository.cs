using BhagirathFincareClassLibrary.Models;
using Domain.Entities.Calculation;
using Domain.Entities.UserAlgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IMarketDataRepository
    {
        Task<List<CalculationData>> GetTodayCalculationsAsync();
        Task<bool> InsertTodayCalculationsAsync(IEnumerable<CalculationData> calculations);
        Task<bool> InsertMarketPricesAsync(IEnumerable<MarketPrice> marketPrices);
        Task<List<UserAlgoData>> GetAlgoConfigurations();
        Task<bool> InsertAlgoConfigurationsAsync(UserAlgoData algoData);
    }
}
