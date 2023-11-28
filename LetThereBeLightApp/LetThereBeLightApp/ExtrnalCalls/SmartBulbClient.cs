using LetThereBeLightApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LetThereBeLightApp.ExtrnalCalls
{
    public static class SmartBulbClient
    {
        public async static Task<IEnumerable<DiscoveryResponse>> DiscoverDevices(int timeOut = 5000)
        {
            var discoveryUrl = string.Format(Constants.Constants.DISCOVERY_ENDPOINT, timeOut);

            using (HttpClient  client = new HttpClient())
            {
                var response = await client.GetAsync(discoveryUrl);
                var json = await response.Content.ReadAsStringAsync();
                var smartBulbsResult = JsonSerializer.Deserialize<List<DiscoveryResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return smartBulbsResult;
            }
        }
    }
}
