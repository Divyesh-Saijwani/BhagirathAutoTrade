using BhagirathFincareClassLibrary.Models;
using Contracts;
using Contracts.Calculation;
using Domain.Entities.Calculation;
using Domain.Entities.UserAlgo;
using Domain.Repositories;
using Identity.Services;
using Mapster;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MarketDataService :IMarketDataService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IUserService _userService;
        public MarketDataService(IRepositoryManager repositoryManager, IUserService userService)
        {
            _repositoryManager = repositoryManager;
            _userService = userService;
        }

        public async Task<List<CalculationDataDto>> GetTodayCalculationsAsync()
        {
            var result= await _repositoryManager.MarketDataRepository.GetTodayCalculationsAsync();
            return result.Adapt<List<CalculationDataDto>>();
        }

        public async Task<bool> InsertTodayCalculationsAsync(IEnumerable<CalculationDataDto> calculations)
        {
            var result = await _repositoryManager.MarketDataRepository.InsertTodayCalculationsAsync(calculations.Adapt<List<CalculationData>>());
            return result;
        }

        public async Task<bool> InsertMarketPricesAsync(IEnumerable<MarketPrice> marketPrices)
        {
            var result = await _repositoryManager.MarketDataRepository.InsertMarketPricesAsync(marketPrices);
            return result;
        }

        public async Task<List<UserAlgoDataDto>> GetAlgoConfigurations()
        {
            var result = await _repositoryManager.MarketDataRepository.GetAlgoConfigurations();
            return result.Adapt<List<UserAlgoDataDto>>();
        }

        public async Task<bool> InsertAlgoConfigurationsAsync(UserAlgoDataDto userAlgo)
        {
            var result = await _repositoryManager.MarketDataRepository.InsertAlgoConfigurationsAsync(userAlgo.Adapt<UserAlgoData>());
            return result;
        }
    }
}
