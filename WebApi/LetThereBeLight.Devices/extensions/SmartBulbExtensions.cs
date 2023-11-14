using LetThereBeLight.Services.Helpers;
using System.Buffers;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;

namespace LetThereBeLight.Devices.extensions
{
    internal static class SmartBulbExtensions
    {
        public static bool SendCommand(this SmartBulb smartBulb, int id, string method, dynamic[] parameters)
        {
            var obj = new { id = id, method = method, @params = parameters };

            //Yeelight requires \r\n delimiters at the end of json data
            string json = JsonSerializer.Serialize(obj) + "\r\n";

            //Full address of yeelight bulb
            string location = smartBulb.DeviceProperties.Location;

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
                Console.WriteLine("Unable to connect to device.");
                return false;
            }

        }
    }
}
