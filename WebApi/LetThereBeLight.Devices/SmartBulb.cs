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
        //Dictionary holds device properties. Can be accessed with an indexer
        private Dictionary<DeviceProperty, dynamic> DeviceValues = new Dictionary<DeviceProperty, dynamic>();
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
            string[] set = data.Trim('\n').Split('\r');
            var propArray = (int[])Enum.GetValues(typeof(DeviceProperty));
            foreach (var i in propArray)
            {
                string val = parseValue(set[i]);
                try
                {
                    DeviceValues.Add((DeviceProperty)i, int.Parse(val));
                }
                catch
                {
                    DeviceValues.Add((DeviceProperty)i, val);
                }
            }
        }

        public bool SendCommand(SmartBulb device, int id, string method, dynamic[] parameters)
        {
            var obj = new { id = id, method = method, @params = parameters };

            //Yeelight requires \r\n delimiters at the end of json data
            string json = JsonSerializer.Serialize(obj) + "\r\n";

            //Full address of yeelight bulb
            string location = (string)device[DeviceProperty.Location];

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
                    //var response = JsonSerializer.Deserialize<dynamic>(memory.Memory.Span.Trim());
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

        //Indexer for Dictionary
        public dynamic this[DeviceProperty dp]
        {
            get
            {
                if (this.DeviceValues.ContainsKey(dp))
                {
                    return this.DeviceValues[dp];
                }

                return string.Empty;
            }

            set
            {
                if (this.DeviceValues.ContainsKey(dp))
                {
                    this.DeviceValues[dp] = value;

                    onPropertyChanged?.Invoke(dp);
                }
            }
        }
        private string parseValue(string raw)
        {
            int startPos = raw.IndexOf(':') + 1;
            return raw.Substring(startPos).Trim();
        }
    }
}