using LetThereBeLight.Devices.Enums;
using LetThereBeLight.Services.Helpers;
using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LetThereBeLight.Devices
{
    public class SmartBulb : ISmartBulb
    {
        public DeviceProperties DeviceProperties { get; set; } = new DeviceProperties();
        public event Action<DeviceProperty>? OnPropertyChanged;

        public static SmartBulb Initialize(string data)
        {
            SmartBulb device = new();
            device.SetProperties(data);
            return device;
        }

        public bool SendCommand(CommandModel command)
        {
            var obj = new { id = DeviceProperties.Id, method = command.Method, @params = command.Params };

            var serializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            //Yeelight requires \r\n delimiters at the end of json data
            string json = JsonSerializer.Serialize(obj, serializerOptions) + "\r\n";

            //Full address of yeelight bulb
            string location = DeviceProperties.Location;

            string ip = NetworkHelper.getAddress(location);
            int port = NetworkHelper.getPort(location);

            if (string.IsNullOrEmpty(ip) || port == 0)
                return false;

            try
            {
                TcpClient client = new();

                client.Connect(ip, port);

                if (client.Connected)
                {
                    //Send command
                    byte[] buffer = Encoding.ASCII.GetBytes(json);
                    client.Client.Send(buffer);

                    //Receive response
                    using IMemoryOwner<byte> memory = MemoryPool<byte>.Shared.Rent(1024 * 4);
                    buffer = new byte[128];
                    client.Client.Receive(memory.Memory.Span);

                    client.Close();

                    string responseJSON = Encoding.ASCII.GetString(buffer);
                    return responseJSON.Contains("ok");
                }
                else
                {
                    client.Close();
                    return false;
                }
            }
            catch (SocketException)
            {
                return false;
            }

        }

        public bool IsPoweredOn()
        {
            return DeviceProperties.Power == Power.On;
        }

        //Parses values from udp response and set the DeviceProperties
        private void SetProperties(string data)
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