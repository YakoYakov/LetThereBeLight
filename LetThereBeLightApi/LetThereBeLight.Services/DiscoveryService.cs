using LetThereBeLight.Devices;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LetThereBeLight.Services
{
    public class DiscoveryService : IDiscoveryService
    {
        // https://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf
        private const string SSDP_searchMessage = "M-SEARCH * HTTP/1.1\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb\r\n"; //Yeelight SSDP search message
        private const string multiCastAddress = "239.255.255.250"; //YeeLight multicast address
        private const int SSDP_port = 1982; //YeeLight SSDP port
        private const int SSDP_timeOut = 10000; //Default time in microseconds to wait for SSDP packet

        public List<SmartBulb> Devices { get; set; } = new List<SmartBulb>();

        public List<SmartBulb> DiscoverDevices(int SSDP_receiveTimeOut = SSDP_timeOut, int pingCount = 1, NetworkInterface? networkInterface = null)
        {
            Dictionary<IPAddress, SmartBulb> devices = new();


            using (var socket = new UdpClient())
            {
                socket.Client.ReceiveTimeout = 1000;

                IPAddress multicastAddress = IPAddress.Parse(multiCastAddress);

                var remoteEndPoint = new IPEndPoint(multicastAddress, SSDP_port);
                var anyEndPoint = new IPEndPoint(IPAddress.Any, SSDP_port);

                socket.JoinMulticastGroup(multicastAddress);

                byte[] buffer = Encoding.ASCII.GetBytes(SSDP_searchMessage);
                socket.Send(buffer, buffer.Length, remoteEndPoint);

                while (true)
                {
                    try
                    {
                        IPEndPoint? sourceEndPoint = default;
                        byte[] message = socket.Receive(ref sourceEndPoint);
                        IPAddress deviceIp = sourceEndPoint.Address;

                        lock (devices)
                        {
                            if (devices.ContainsKey(deviceIp))
                            {
                                continue;
                            }
                            var deviceInfo = Encoding.ASCII.GetString(message);
                            var device = SmartBulb.Initialize(deviceInfo);
                            devices.Add(deviceIp, device);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            Devices = devices.Select(n => n.Value).ToList();
            return Devices;
        }
    }
}