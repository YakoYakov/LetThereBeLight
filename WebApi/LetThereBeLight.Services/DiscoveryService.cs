using LetThereBeLight.Devices;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LetThereBeLight.Services
{
    public class DiscoveryService : IDicoveryService
    {
        // https://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf
        private const string SSDP_searchMessage = "M-SEARCH * HTTP/1.1\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb\r\n"; //Yeelight SSDP search message
        private const string multiCastAddress = "239.255.255.250"; //YeeLight multicast address
        private const int SSDP_port = 1982; //YeeLight SSDP port
        private const int SSDP_timeOut = 10000; //Default time in microseconds to wait for SSDP packet

        public List<ISmartBulb> Devices { get; set; } = new List<ISmartBulb>();

        
        public List<ISmartBulb> DiscoverDevices(int SSDP_receiveTimeOut = SSDP_timeOut, int pingCount = 1, NetworkInterface? networkInterface = null)
        {
            Dictionary<IPAddress, ISmartBulb> devices = new Dictionary<IPAddress, ISmartBulb>();

            using (UdpClient socket = new UdpClient())
            {
                IPAddress multicastAddress = IPAddress.Parse(multiCastAddress);
                IPEndPoint multicastEndpoint = new IPEndPoint(multicastAddress, SSDP_port);

                bool isMulticastJoined = false;

                if (networkInterface == null)
                {
                    foreach (NetworkInterface currentNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var tmpClient = socket.Client;
                        isMulticastJoined = TryJoinMulticastAndSetListener(ref tmpClient, currentNetworkInterface, multicastEndpoint);
                        socket.Client = tmpClient;

                        //Break if this interface joined the Multicast
                        if (isMulticastJoined)
                        {
                            networkInterface = currentNetworkInterface;
                            break;
                        }
                    }
                }
                else
                {
                    var tmpClient = socket.Client;
                    isMulticastJoined = TryJoinMulticastAndSetListener(ref tmpClient, networkInterface, multicastEndpoint);
                    socket.Client = tmpClient;
                }

                if (!isMulticastJoined)
                {
                    throw new Exception("Failed to join Multicast with the network interface.");
                }

                // Disable Multicast packet loop back
                socket.Client.MulticastLoopback = false;

                socket.Client.ReceiveTimeout = SSDP_receiveTimeOut;

                byte[] buffer = Encoding.ASCII.GetBytes(SSDP_searchMessage);

                for (int i = 0; i < pingCount; i++)
                {
                    socket.Send(buffer, buffer.Length, multicastEndpoint);

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
            }
            Devices = devices.Select(n => n.Value).ToList();
            return Devices;
        }

        private static bool IsNicGoodForMulticast(NetworkInterface adapter)
        {
            IPInterfaceProperties ip_properties = adapter.GetIPProperties();
            if (!adapter.GetIPProperties().MulticastAddresses.Any())
                return false; // most of VPN adapters will be skipped
            if (!adapter.SupportsMulticast)
                return false; // Doesn't support Multicast, no point in trying
            if (OperationalStatus.Up != adapter.OperationalStatus)
                return false; // This adapter is off or not connected
            IPv4InterfaceProperties IPv4Props = adapter.GetIPProperties().GetIPv4Properties();
            if (IPv4Props == null)
                return false; // IPv4 is not configured on this adapter

            // IPv4InterfaceProperties.Index's getter can throw if the interface has no index
            try
            {
                _ = IPv4Props.Index;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static bool TryJoinMulticastAndSetListener(ref Socket socket, NetworkInterface networkInterface, IPEndPoint multicastEndpoint)
        {
            if (!IsNicGoodForMulticast(networkInterface))
            {
                return false;
            }

            // Set incoming Multicast packet address, pick the first address that doesn't fail joining to the Multicast group
            foreach (var localAddr in networkInterface.GetIPProperties().UnicastAddresses.Select(x => x.Address))
            {
                try
                {
                    socket.Bind(new IPEndPoint(localAddr, multicastEndpoint.Port));

                    //Index getter shouldn't throw here because IsNicGoodForMulticast checks for that
                    int ifIndex = networkInterface.GetIPProperties().GetIPv4Properties().Index;

                    MulticastOption multicastOptions = new MulticastOption(multicastEndpoint.Address, ifIndex);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, multicastOptions);

                    // Set outgoing Multicast packet interface
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(ifIndex));

                    return true;
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }
    }
}