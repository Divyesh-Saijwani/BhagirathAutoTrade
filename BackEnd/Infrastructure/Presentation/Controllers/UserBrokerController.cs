using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/UserBroker")]
    public class UserBrokerController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UserBrokerController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("{configId}/UpdateBrokerConfiguration")]
        public async Task<IActionResult> UpdateBrokerConfiguration(Guid configId, [FromQuery] string apisession)
        {
            await _serviceManager.UserBrokerService.UpdateBrokerConfiguration(configId, apisession);

            // Redirect to a webpage (replace with your actual URL)
            return Redirect("https://www.algomoney.in");
        }
    }
}
