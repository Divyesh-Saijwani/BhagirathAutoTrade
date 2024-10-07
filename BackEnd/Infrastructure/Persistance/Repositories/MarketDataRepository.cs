using AlgoBhagirath.Common;
using BhagirathFincareClassLibrary.Models;
using Dapper;
using Domain.Entities.Broker;
using Domain.Entities.Calculation;
using Domain.Entities.UserAlgo;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class MarketDataRepository : IMarketDataRepository
    {
        private readonly RepositoryDbContext _repositoryDbContext;

        public MarketDataRepository(RepositoryDbContext repositoryDbContext) => _repositoryDbContext = repositoryDbContext;
        public async Task<List<CalculationData>> GetTodayCalculationsAsync()
        {
            using var connection = _repositoryDbContext.CreateConnection();
            var customers = await connection.QueryAsync<CalculationData>(DatabaseConstants.GetTodayCalculations, commandType: System.Data.CommandType.StoredProcedure);
            return customers.ToList();
        }

        public async Task<bool> InsertTodayCalculationsAsync(IEnumerable<CalculationData> calculations)
        {
            try
            {
                foreach (var item in calculations)
                {
                    item.CalculationDateTime = DateTime.Now;
                    item.IsActive = true;
                }
                var parameters = new DynamicParameters();

                // Prepare DataTable for StockCalculations
                var table = new DataTable();
                table.Columns.Add("Symbol", typeof(string));
                table.Columns.Add("Exchange", typeof(string));
                table.Columns.Add("Type", typeof(string));
                table.Columns.Add("Instrument", typeof(string));
                table.Columns.Add("OptionType", typeof(string));
                table.Columns.Add("StrikePrice", typeof(decimal));
                table.Columns.Add("Expiry", typeof(DateTime));
                table.Columns.Add("Trend", typeof(string));
                table.Columns.Add("LowPoint", typeof(decimal));
                table.Columns.Add("AveragePoint", typeof(decimal));
                table.Columns.Add("MaxPoint", typeof(decimal));
                table.Columns.Add("Direction", typeof(string));
                table.Columns.Add("EntryTime", typeof(DateTime));
                table.Columns.Add("StopLoss", typeof(int));
                table.Columns.Add("Target", typeof(decimal));
                table.Columns.Add("CalculationDateTime", typeof(DateTime));
                table.Columns.Add("IsActive", typeof(bool));

                foreach (var calc in calculations)
                {
                    table.Rows.Add(calc.Symbol, calc.Exchange, calc.Type, calc.Instrument, calc.OptionType,
                                   calc.StrikePrice, calc.Expiry, calc.Trend, calc.LowPoint, calc.AveragePoint,
                                   calc.MaxPoint, calc.Direction, calc.EntryTime, calc.StopLoss, calc.Target,
                                   calc.CalculationDateTime, calc.IsActive);
                }

                parameters.Add("@StockCalculations", table.AsTableValuedParameter("dbo.StockCalculationType"));

                using var connection = _repositoryDbContext.CreateConnection(); // Adjust to your context method
                var result = await connection.ExecuteAsync("dbo.InsertStockCalculations", parameters, commandType: CommandType.StoredProcedure);

                return result > 0;

            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> InsertMarketPricesAsync(IEnumerable<MarketPrice> marketPrices)
        {
            var parameters = new DynamicParameters();

            // Create DataTable for MarketPrice records
            var table = new DataTable();
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("ExpiryDate", typeof(DateTime));
            table.Columns.Add("StrikePrice", typeof(string));
            table.Columns.Add("OptionType", typeof(string));
            table.Columns.Add("Open", typeof(string));
            table.Columns.Add("High", typeof(string));
            table.Columns.Add("Low", typeof(string));
            table.Columns.Add("Close", typeof(string));
            table.Columns.Add("UploadDate", typeof(DateTime));
            table.Columns.Add("FileType", typeof(int));
            table.Columns.Add("ScriptCode", typeof(string));
            table.Columns.Add("EMA13", typeof(string));
            table.Columns.Add("EMA34", typeof(string));
            table.Columns.Add("TotalTradeValue", typeof(string));

            foreach (var price in marketPrices)
            {
                table.Rows.Add(price.Symbol, price.ExpiryDate, price.StrikePrice, price.OptionType,
                               price.Open, price.High, price.Low, price.Close, price.UploadDate,
                               price.FileType, price.ScriptCode, price.EMA13, price.EMA34, price.TotalTradeValue);
            }

            parameters.Add("@MarketPrices", table.AsTableValuedParameter("dbo.MarketPriceType"));

            using var connection = _repositoryDbContext.CreateConnection(); // Adjust to your context method
            var result = await connection.ExecuteAsync("dbo.InsertMarketPrices", parameters, commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<List<UserAlgoData>> GetAlgoConfigurations()
        {
            using var connection = _repositoryDbContext.CreateConnection();
            var customers = await connection.QueryAsync<UserAlgoData>("dbo.GetAllUserAlgoData", commandType: System.Data.CommandType.StoredProcedure);
            return customers.ToList();
        }

        public async Task<bool> InsertAlgoConfigurationsAsync(UserAlgoData userAlgoData)
        {
            // Define parameters for the stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Guid.NewGuid());
            parameters.Add("@WorkingDate", (object)userAlgoData.WorkingDate ?? DBNull.Value);
            parameters.Add("@Symbol", userAlgoData.Symbol);
            parameters.Add("@Exchange", userAlgoData.Exchange);
            parameters.Add("@Type", (object)userAlgoData.Type ?? DBNull.Value);
            parameters.Add("@Instrument", (object)userAlgoData.Instrument ?? DBNull.Value);
            parameters.Add("@OptionType", (object)userAlgoData.OptionType ?? DBNull.Value);
            parameters.Add("@ExpiryDate", (object)userAlgoData.ExpiryDate ?? DBNull.Value);
            parameters.Add("@CallStrikePrice", userAlgoData.CallStrikePrice);
            parameters.Add("@PutStrikePrice", userAlgoData.PutStrikePrice);
            parameters.Add("@Direction", userAlgoData.Direction);
            parameters.Add("@NoOfLots", userAlgoData.NoOfLots);
            parameters.Add("@Multiple", userAlgoData.Multiple);
            parameters.Add("@Status", userAlgoData.Status);

            using (var connection = _repositoryDbContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync("dbo.InsertUserAlgoData", parameters, commandType: CommandType.StoredProcedure);
                return result > 0; // Return true if the operation affected one or more rows
            }
        }
    }
}
