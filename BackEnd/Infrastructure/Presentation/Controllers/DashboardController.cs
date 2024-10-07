using Identity.Security;
using MarketData.Services;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IMarketData _marketData;
        public DashboardController(IServiceManager serviceManager, IMarketData marketData)
        {
            _serviceManager = serviceManager;
            _marketData = marketData;
        }

        [HttpGet("GetUserBrokerConfigurationStatus/{brokerId}")]
        public async Task<IActionResult> GetUserBrokerConfigurationStatus(Guid brokerId)
        {
            var claims = new Claims(HttpContext);
            var roles = claims.Role?.Split(',').ToList();
            var userId = Guid.Parse(claims.UserId);
            var result=await _serviceManager.UserBrokerService.GetBrokerDetailsForUpdateAsync(brokerId, userId);
            return Ok(result);
        }
    }
}
