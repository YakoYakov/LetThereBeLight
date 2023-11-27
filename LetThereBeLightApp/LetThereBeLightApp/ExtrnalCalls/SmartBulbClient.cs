using LetThereBeLightApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LetThereBeLightApp.ExtrnalCalls
{
    public static class SmartBulbClient
    {
        public async static Task<IEnumerable<SmartBulb>> DiscoverDevices(int timeOut = 5000)
        {
            var discoveredDevices = new List<SmartBulb>();

            var discoveryUrl = string.Format(Constants.Constants.DISCOVERY_ENDPOINT, timeOut);

            using (HttpClient  client = new HttpClient())
            {
                var response = await client.GetAsync(discoveryUrl);
                var json = await response.Content.ReadAsStringAsync();
               
            }

            return discoveredDevices;
        }
    }
}
