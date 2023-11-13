using LetThereBeLight.Devices.Enums;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Buffers;
using LetThereBeLight.Services.Helpers;

namespace LetThereBeLight.Devices
{
    public class SmartBulb : ISmartBulb
    {
        public DeviceProperties DeviceProperties { get; set; } = new DeviceProperties();
        public event Action<DeviceProperty>? onPropertyChanged;

        public static SmartBulb Initialize(string data)
        {
            SmartBulb device = new SmartBulb();
            device.GetProperties(data);
            return device;
        }

        //Parses values from udp response and fills dictionary
        public void GetProperties(string data)
        {
            string[] set = data.Trim('\n').Split('\r', StringSplitOptions.RemoveEmptyEntries);
            var propArray = (int[])Enum.GetValues(typeof(DeviceProperty));
            foreach (var i in propArray)
            {
                string val = parseValue(set[i]);
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
                            DeviceProperties.Power = val;
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

        public bool SendCommand(SmartBulb device, int id, string method, dynamic[] parameters)
        {
            var obj = new { id = id, method = method, @params = parameters };

            //Yeelight requires \r\n delimiters at the end of json data
            string json = JsonSerializer.Serialize(obj) + "\r\n";

            //Full address of yeelight bulb
            string location = device.DeviceProperties.Location;

            string ip = NetworkHelper.getAddress(location);
            int port = NetworkHelper.getPort(location);

            if (string.IsNullOrEmpty(ip) || port == 0)
                return false;

            try
            {
                TcpClient client = new TcpClient();

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
                Console.WriteLine("Unable to connect to device.");
                return false;
            }

        }

        private string parseValue(string raw)
        {
            int startPos = raw.IndexOf(':') + 1;
            return raw.Substring(startPos).Trim();
        }
    }
}