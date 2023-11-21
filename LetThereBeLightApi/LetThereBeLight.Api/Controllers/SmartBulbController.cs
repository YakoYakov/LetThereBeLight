using LetThereBeLight.Devices;
using LetThereBeLight.Services;
using LetThereBeLight.Services.Extensions;
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
        [ProducesResponseType(typeof(IEnumerable<ISmartBulb>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DiscoverConnectedDevices(int timeOut = DEFAULT_DISCOVERY_TIME_OUT_MILISECONDS)
        {
            var result = _discoveryService.DiscoverDevices(timeOut);

            if (result.Count == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("toggle")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ToggleOnOff(int smartBulbId)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == smartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ToggleOnOff());
        }

        [HttpPost("brightness")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeBrightness(int smartBulbId, int brightness)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == smartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeBrightness(brightness));
        }

        [HttpPost("color-temperature")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeColorTemperature(int smartBulbId, int colorTemperature)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == smartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeColorTemperature(colorTemperature));
        }

        [HttpPost("rgb")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeRGB(int smartBulbId, int r, int g, int b)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == smartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeRGB(r, g, b));
        }

        [HttpPost("name")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeName(int smartBulbId, string name)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == smartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.SetName(name));
        }

        private void HasDeviceDiscoveryRun()
        {
            if (!_discoveryService.Devices.Any())
            {
                _discoveryService.DiscoverDevices(DEFAULT_DISCOVERY_TIME_OUT_MILISECONDS);
            }
        }
    }
}
