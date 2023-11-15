using LetThereBeLight.Devices;
using LetThereBeLight.Devices.Enums;
using LetThereBeLight.Services.Constants;

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
        public static SmartBulb ToggleOnOff(
            this SmartBulb smartBulb,
            Effect effect = Effect.Smooth,
            int duration = CommandConstants.DEFAULT_DURATION_MILISECONDS)
        {
            var newState = smartBulb.IsPoweredOn() ? Power.Off : Power.On;

            var command = new CommandModel
            {
                Method = Devices.Enums.Method.set_power,
                Params = new List<object>() { newState, effect, duration }
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
        public static SmartBulb ChangeColorTemperature(
            this SmartBulb smartBulb,
            int temperature,
            Effect effect = Effect.Smooth,
            int duration = CommandConstants.DEFAULT_DURATION_MILISECONDS)
        {
            // Cannot make changes on device that is off
            if (!smartBulb.IsPoweredOn()) { return smartBulb; }
            if (temperature < 1700) { temperature = 1700; }
            if (temperature > 6500) { temperature = 6500; }

            var changeColorCommand = new CommandModel
            {
                Method = Method.set_ct_abx,
                Params = new List<object> { temperature, effect, duration }
            };

            var isSuccesful = smartBulb.SendCommand(changeColorCommand);

            if (isSuccesful)
            {
                smartBulb.DeviceProperties.ColorTemperature = temperature;
                smartBulb.DeviceProperties.ColorMode = 2;
            }

            return smartBulb;
        }

        /// <summary>
        /// Changes the RGB setting of the smart bulb.
        /// </summary>
        /// <param name="r">Red color</param>
        /// <param name="g">Green color</param>
        /// <param name="b">blue color</param>
        /// <param name="effect">Shuld the transition be smooth or sudden (default is smooth)</param>
        /// <param name="duration">The duration of the change (applies only id the effect is smooth)</param>
        /// <returns><see cref="SmartBulb"/></returns>
        public static SmartBulb ChangeRGB(
            this SmartBulb smartBulb,
            int r,
            int g,
            int b,
            Effect effect = Effect.Smooth,
            int duration = CommandConstants.DEFAULT_DURATION_MILISECONDS)
        {
            // Cannot make changes on device that is off
            if (!smartBulb.IsPoweredOn()) { return smartBulb; }
            var sumRgb = GetSumRGB(r, g, b);

            var changeRGBCommand = new CommandModel
            {
                Method = Method.set_rgb,
                Params = new List<object> { sumRgb, effect, duration }
            };

            var isSuccesful = smartBulb.SendCommand(changeRGBCommand);

            if (isSuccesful)
            {
                smartBulb.DeviceProperties.RGB = sumRgb;
                smartBulb.DeviceProperties.ColorMode = 1;
            }

            return smartBulb;
        }

        public static SmartBulb ChangeBrightness(
            this SmartBulb smartBulb,
            int brightness,
            Effect effect = Effect.Smooth,
            int duration = CommandConstants.DEFAULT_DURATION_MILISECONDS)
        {
            // Cannot make changes on device that is off
            if (!smartBulb.IsPoweredOn()) { return smartBulb; }

            if (brightness < CommandConstants.MIN_BRIGHTNESS) { brightness = CommandConstants.MIN_BRIGHTNESS; }
            if (brightness > CommandConstants.MAX_BRIGHTNESS) { brightness = CommandConstants.MAX_BRIGHTNESS; }

            var changeBrightnessCommand = new CommandModel
            {
                Method = Method.set_bright,
                Params = new List<object> { brightness, effect, duration }
            };

            var isSuccesful = smartBulb.SendCommand(changeBrightnessCommand);
            if (isSuccesful)
            {
                smartBulb.DeviceProperties.Brightness = brightness;
            }

            return smartBulb;
        }

        private static int GetSumRGB(int r, int g, int b)
        {
            var result = (r * 65536) + (g * 256) + b;

            if (result < CommandConstants.MIN_RGB_VALUE) { result = 0; }
            if (result > CommandConstants.MAX_RGB_VALUE) { result = 16777215; }

            return result;
        }
    }
}
