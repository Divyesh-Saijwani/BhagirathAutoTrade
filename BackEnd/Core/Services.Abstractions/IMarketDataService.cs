using BhagirathFincareClassLibrary.Models;
using Contracts;
using Contracts.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IMarketDataService
    {
        public Task<List<CalculationDataDto>> GetTodayCalculationsAsync();
        Task<bool> InsertTodayCalculationsAsync(IEnumerable<CalculationDataDto> calculations);
        Task<bool> InsertMarketPricesAsync(IEnumerable<MarketPrice> marketPrices);
        Task<List<UserAlgoDataDto>> GetAlgoConfigurations();
        Task<bool> InsertAlgoConfigurationsAsync(UserAlgoDataDto userAlgo);
    }
}
