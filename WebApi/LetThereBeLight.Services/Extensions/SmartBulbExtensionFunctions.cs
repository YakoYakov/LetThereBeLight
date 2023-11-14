using LetThereBeLight.Devices;
using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Services.Extensions
{
    public static class SmartBulbExtensionFunctions
    {
        /// <summary>
        /// Toggles the power of the smart lamb
        /// </summary>
        /// <param name="effect">Shuld the transition be smooth or sudden (default is smooth)</param>
        /// <param name="duration">The duration of the change (applies only id the effect is smooth)</param>
        /// <returns><see cref="SmartBulb"/></returns>
        public static SmartBulb ToggleOnOff(this SmartBulb smartBulb, Effect effect = Effect.Smooth, int duration = 500)
        {
            var newState = smartBulb.IsPoweredOn() ? Power.Off : Power.On;

            var command = new CommandModel
            {
                Method = Devices.Enums.Method.set_power,
                Params = new List<object>() { nameof(newState).ToLower(), nameof(effect).ToLower(), duration }
            };

            var isSuccesful = smartBulb.SendCommand(command);
            if (isSuccesful)
            {
                smartBulb.DeviceProperties.Power = newState;
            }

            return smartBulb;
        }

        /// <summary>
        /// Changes the color temperature of the Smart bulb.
        /// </summary>
        /// <param name="temperature">The new color temperature (range is 1700 ~ 6500)</param>
        /// <param name="effect">Shuld the transition be smooth or sudden (default is smooth)</param>
        /// <param name="duration">The duration of the change (applies only id the effect is smooth)</param>
        /// <returns><see cref="SmartBulb"/></returns>
        public static SmartBulb ChangeColorTemperature(this SmartBulb smartBulb, int temperature, Effect effect = Effect.Smooth, int duration = 500) 
        {
            if (temperature < 1700) { temperature = 1700; }
            if (temperature > 6500) { temperature = 6500; }

            var changeColorCommand = new CommandModel
            {
                Method = Method.set_ct_abx,
                Params = new List<object> { temperature, nameof(effect).ToLower(), duration }
            };

            var isSuccesful = smartBulb.SendCommand(changeColorCommand); 

            if (isSuccesful) 
            {
                smartBulb.DeviceProperties.ColorTemperature = temperature;
                smartBulb.DeviceProperties.ColorMode = 2;
            }

            return smartBulb;
        }
    }
}
