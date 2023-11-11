using LetThereBeLight.Devices;

namespace LetThereBeLight.Services
{
    public interface INetworkService
    {
      Task<List<IDevice>> DiscoverDevicesAsync(int timeout = 5000);
    }
}
