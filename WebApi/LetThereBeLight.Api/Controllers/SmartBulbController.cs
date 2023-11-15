using LetThereBeLight.Devices;
using LetThereBeLight.Services;
using Microsoft.AspNetCore.Mvc;

namespace LetThereBeLight.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartBulbController : ControllerBase
    {
        private const int DEFAULT_DISCOVERY_TIME_OUT_MILISECONDS = 5000;

        private readonly IDiscoveryService _discoveryService;

        public SmartBulbController(IDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        [HttpGet("discover-devices")]
        public IEnumerable<SmartBulb> DiscoverConnectedDevices(int timeOut = DEFAULT_DISCOVERY_TIME_OUT_MILISECONDS)
        {
            return _discoveryService.DiscoverDevices(timeOut);
        }
    }
}
