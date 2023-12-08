using LetThereBeLight.Api.BindingModels;
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
        public ActionResult ToggleOnOff(BaseBindingModel bindingModel) 
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == bindingModel.SmartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ToggleOnOff());
        }

        [HttpPost("brightness")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeBrightness(BrightnessModel bindingModel)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == bindingModel.SmartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeBrightness(bindingModel.Brightness));
        }

        [HttpPost("color-temperature")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeColorTemperature(ColorTempModel bindingModel)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == bindingModel.SmartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeColorTemperature(bindingModel.ColorTemperature));
        }

        [HttpPost("rgb")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeRGB(ColorModel bindingModel)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == bindingModel.SmartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.ChangeRGB(bindingModel.R, bindingModel.G, bindingModel.B));
        }

        [HttpPost("name")]
        [ProducesResponseType(typeof(CommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ChangeName(NameModel bindingModel)
        {
            HasDeviceDiscoveryRun();

            var smartBulb = _discoveryService.Devices.FirstOrDefault(sb => sb.DeviceProperties.Id == bindingModel.SmartBulbId);

            if (smartBulb == null) { return NotFound(); }

            return Ok(smartBulb.SetName(bindingModel.Name));
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
