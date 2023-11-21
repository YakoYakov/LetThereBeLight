using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Devices
{
    public interface ISmartBulb
    {
        DeviceProperties DeviceProperties { get; }
        event Action<DeviceProperty>? OnPropertyChanged;

        /// <summary>
        /// Sends command to the smart bulb.
        /// </summary>
        /// <param name="command">The command to be executed on the smart bulb.</param>
        /// <returns>True if the command succeeded otherwise false.</returns>
        bool SendCommand(CommandModel command);

        /// <summary>
        /// Checks if the lamp is on
        /// </summary>
        /// <returns>True if the lamp is on otherwise false.</returns>
        bool IsPoweredOn();
    }
}
