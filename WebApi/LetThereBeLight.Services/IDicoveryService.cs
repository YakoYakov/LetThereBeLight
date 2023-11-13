using LetThereBeLight.Devices;
using System.Net.NetworkInformation;

namespace LetThereBeLight.Services
{
    public interface IDicoveryService
    {
        // In memory collection of Devices
        List<ISmartBulb> Devices { get; }

        /// <summary>
        /// Discovers light devices by using SSDP protocol
        /// </summary>
        /// <param name="SSDP_receiveTimeOut"> How much time to wait for SSDP packets in milliseconds</param>
        /// <param name="pingCount"> How many SSDP broadcasts to make before returning</param>
        /// <param name="networkInterface"> The network interface to join to Multicast from. If this parameter is null, best interface for Multicast is used.</param>
        /// <returns>
        /// A List of <see cref="ISmartBulb"/> discovered devices.
        /// </returns>
        List<ISmartBulb> DiscoverDevices(int SSDP_receiveTimeOut, int pingCount = 1, NetworkInterface? networkInterface = null);
    }
}
