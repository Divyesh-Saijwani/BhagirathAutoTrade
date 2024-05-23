using BhagirathAutoTrade.Server.Models;
using BhagirathAutoTrade.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BhagirathAutoTrade.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquityController : Controller
    {
        private readonly IEquityService _equityService;

        public EquityController(IEquityService equityService)
        {
            _equityService = equityService;
        }

        [HttpPost("AutoCompleteCompanyForEquity")]
        public async Task<ActionResult<IEnumerable<string>>> AutoCompleteCompanyForEquity([FromBody] EquityRequestModel model)
        {
            var data = await _equityService.AutoCompleteCompanyForEquityAsync(model.Exchange,model.Type,model.Instrument,model.OptionType);
            return Ok(data);
        }

        [HttpPost("GetCalculateDataForEQ")]
        public async Task<ActionResult<EquityData>> GetCalculateDataForEQ([FromBody] EquityRequestModel model)
        {
            var data =await _equityService.GetCalculateDataForEQAsync(model);
            return Ok(data);
        }

        [HttpPost("GetOpenData")]
        public async  Task<ActionResult<string>> GetOpenData([FromBody] EquityRequestModel model)
        {
            var data =await _equityService.GetOpenDataAsync(model.WorkingDate,model.ExpiryDate,model.Exchange,model.Instrument,model.OptionType,model.Type,model.StrickPrice,model.Symbole);
            return Ok(data);
        }

        [HttpPost("GetCloseData")]
        public async Task<ActionResult<string>> GetCloseData([FromBody] EquityRequestModel model)
        {
            var data = await _equityService.GetCloseDataAsync(model.WorkingDate, model.ExpiryDate, model.Exchange, model.Instrument, model.OptionType, model.Type, model.StrickPrice, model.Symbole);
            return Ok(data);
        }

        [HttpGet("GetExpiryDate")]
        public async Task<ActionResult<string>> GetExpiryDate(string symbol, string optionType)
        {
            var data = await _equityService.GetExpiryDateAsync(symbol, optionType);
            return Ok(data);
        }

        [HttpPost("GetStrikePrice")]
        public async Task<ActionResult<List<string>>> GetStrikePrice([FromBody] EquityRequestModel model)
        {
            var data = await _equityService.GetStrikePriceAsync(model.Exchange, model.Type, model.Symbole, model.ExpiryDate);
            return Ok(data);
        }
    }
}
