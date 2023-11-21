using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Devices
{
    public class DeviceProperties
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public Power Power { get; set; } = Power.Off;
        public int Brightness { get; set; }
        public int ColorMode { get; set; }
        public int ColorTemperature { get; set; }
        public int RGB { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
