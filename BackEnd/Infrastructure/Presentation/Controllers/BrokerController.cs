using Contracts.Broker;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/Broker")]
    public class BrokerController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BrokerController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet()]
        public async Task<IActionResult> GetBrokers()
        {
            var brokers = await _serviceManager.BrokerService.GetAllAsync();
            return Ok(brokers);
        }

        [HttpGet("{brokerId}")]
        public async Task<IActionResult> GetByBrokerId(Guid brokerId)
        {
            var broker = await _serviceManager.BrokerService.GetByIdAsync(brokerId);
            return Ok(broker);
        }

        [HttpPost]
        public async Task<IActionResult> AddBroker(BrokerDto broker)
        {
            try
            {
                if (broker.BrokerId == null || broker.BrokerId == Guid.Empty)
                {
                    broker.BrokerId = Guid.NewGuid();
                }
                var result = await _serviceManager.BrokerService.InsertOrUpdateAsync(broker);
                return Ok(await _serviceManager.BrokerService.GetByIdAsync(broker.BrokerId.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{brokerId}")]
        public async Task<IActionResult> DeleteBroker(Guid brokerId)
        {
            await _serviceManager.BrokerService.DeleteAsync(brokerId);
            return Ok();
        }
    }
}
