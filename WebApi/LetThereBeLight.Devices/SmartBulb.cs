using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Devices
{
    public class SmartBulb : ISmartBulb
    {
        public DeviceProperties DeviceProperties { get; set; } = new DeviceProperties();
        public event Action<DeviceProperty>? OnPropertyChanged;

        public static SmartBulb Initialize(string data)
        {
            SmartBulb device = new();
            device.GetProperties(data);
            return device;
        }

        //Parses values from udp response and set the DeviceProperties
        public void GetProperties(string data)
        {
            string[] set = data.Trim('\n').Split('\r', StringSplitOptions.RemoveEmptyEntries);
            var propArray = (int[])Enum.GetValues(typeof(DeviceProperty));
            foreach (var i in propArray)
            {
                string val = ParseValue(set[i]);
                try
                {
                    switch ((DeviceProperty)i)
                    {
                        case DeviceProperty.Location:
                            DeviceProperties.Location = val;
                            break;
                        case DeviceProperty.Id:
                            DeviceProperties.Id = Convert.ToInt32(val, 16);
                            break;
                        case DeviceProperty.Saturation:
                            DeviceProperties.Saturation = int.Parse(val);
                            break;
                        case DeviceProperty.RGB:
                            DeviceProperties.RGB = int.Parse(val);
                            break;
                        case DeviceProperty.Power:
                            DeviceProperties.Power = Enum.Parse<Power>(val, true);
                            break;
                        case DeviceProperty.Hue:
                            DeviceProperties.Hue = int.Parse(val);
                            break;
                        case DeviceProperty.ColorTemperature:
                            DeviceProperties.ColorTemperature = int.Parse(val);
                            break;
                        case DeviceProperty.Brightness:
                            DeviceProperties.Brightness = int.Parse(val);
                            break;
                        case DeviceProperty.ColorMode:
                            DeviceProperties.ColorMode = int.Parse(val);
                            break;
                        case DeviceProperty.Name:
                            DeviceProperties.Name = val;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Couldn`t process returned value {val} when initializing device properties", ex);
                }
            }
        }

        private static string ParseValue(string raw)
        {
            int startPos = raw.IndexOf(':') + 1;
            return raw[startPos..].Trim();
        }
    }
}