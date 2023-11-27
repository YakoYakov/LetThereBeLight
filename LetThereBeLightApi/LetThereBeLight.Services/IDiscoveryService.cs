using LetThereBeLight.Devices;
using System.Net.NetworkInformation;

namespace LetThereBeLight.Services
{
    public interface IDiscoveryService
    {
        // In memory collection of Devices
        List<SmartBulb> Devices { get; }

        /// <summary>
        /// Discovers light devices by using SSDP protocol
        /// </summary>
        /// <param name="SSDP_receiveTimeOut"> How much time to wait for SSDP packets in milliseconds</param>
        /// <returns>
        /// A List of <see cref="SmartBulb"/> discovered devices.
        /// </returns>
        List<SmartBulb> DiscoverDevices(int SSDP_receiveTimeOut);
    }
}
