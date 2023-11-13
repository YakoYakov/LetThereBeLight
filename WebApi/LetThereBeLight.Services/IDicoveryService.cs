using LetThereBeLight.Devices;
using System.Net.NetworkInformation;

namespace LetThereBeLight.Services
{
    public interface IDicoveryService
    {
        List<ISmartBulb> DiscoverDevices(int SSDP_receiveTimeOut, int pingCount = 1, NetworkInterface? networkInterface = null);
    }
}
