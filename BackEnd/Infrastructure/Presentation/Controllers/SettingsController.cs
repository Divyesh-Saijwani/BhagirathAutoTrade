using MarketData.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/Settings")]
    public class SettingsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMarketData _marketData;
        public SettingsController(IMarketData marketData, IServiceManager serviceManager)
        {
            _marketData = marketData;
            _serviceManager = serviceManager;
        }

        [HttpGet("UpdateMarketDataToken")]
        public async Task<ActionResult> Index(string apisession)
        {
            var result =_marketData.CheckTokenStatusToken(apisession);
            if (result)
            {
                await _serviceManager.UserBrokerService.UpdateBrokerConfiguration(new Guid("84987133-D4C4-4A9C-95A1-B25E8ABC1CD0"), apisession);
                return Redirect("https://www.algomoney.in/login");
            }
            return BadRequest($"Invalid Token : {apisession}");
        }
    }
}
