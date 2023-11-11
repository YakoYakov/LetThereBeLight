using LetThereBeLight.Devices;
using System.Net.Sockets;
using System.Net;
using System.Text;
using LetThereBeLight.Services.Helpers;

namespace LetThereBeLight.Services
{
    public class NetworkService : INetworkService
    {
        // https://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf
        private const string dgram = "M-SEARCH * HTTP/1.1\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb\r\n"; //Yeelight udp datagram
        private const string MULTICAST_ADDRESS = "239.255.255.250"; //Yeelight multicast address
        private const int DEVICE_PORT = 1982; //Yeelight comm port

        public async Task<List<IDevice>> DiscoverDevicesAsync(int timeout = 5000)
        {

            Dictionary<string, IDevice> devices = new Dictionary<string, IDevice>();

            using (UdpClient socket = new UdpClient())
            {
                socket.Client.ReceiveTimeout = 1000;

                IPAddress multicastAddress = IPAddress.Parse(MULTICAST_ADDRESS);

                IPEndPoint remoteEndPoint = new IPEndPoint(multicastAddress, DEVICE_PORT);
                IPEndPoint anyEndPoint = new IPEndPoint(IPAddress.Any, DEVICE_PORT);

                socket.JoinMulticastGroup(multicastAddress);

                byte[] buffer = Encoding.ASCII.GetBytes(dgram);
                string localIp = NetworkUtils.GetLocalIp();

                while (true)
                {
                    var send = await socket.Send(buffer.AsMemory(), remoteEndPoint);
                    await Task.Delay(50);

                    try
                    {

                        var response = socket.Receive(ref anyEndPoint);

                        //var deviceIp = response.Address.ToString();

                        //if (deviceIp == localIp || devices.ContainsKey(deviceIp))
                        //    continue;

                        //var deviceInfo = Encoding.ASCII.GetString(response.Buffer);
                        ////var device = Device.Initialize(deviceInfo);

                        //devices.Add(deviceIp, null);

                        await Task.Delay(200);
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }

            return devices.Select(n => n.Value).ToList();
        }
    }
}