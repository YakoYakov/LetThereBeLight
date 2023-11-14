using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Devices
{
    public interface ISmartBulb
    {
        DeviceProperties DeviceProperties { get; }
        event Action<DeviceProperty>? OnPropertyChanged;
    }
}
