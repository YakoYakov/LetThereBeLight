using LetThereBeLightApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static LetThereBeLightApp.Constants.Constants;

namespace LetThereBeLightApp.ExtrnalCalls
{
    public static class SmartBulbClient
    {
        public async static Task<IEnumerable<DiscoveryResponse>> DiscoverDevices(int timeOut = 5000)
        {
            var discoveryUrl = string.Format(DISCOVERY_ENDPOINT, timeOut);

            using (HttpClient  client = new HttpClient())
            {
                var response = await client.GetAsync(discoveryUrl);
                var json = await response.Content.ReadAsStringAsync();
                var smartBulbsResult = JsonSerializer.Deserialize<List<DiscoveryResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return smartBulbsResult;
            }
        }

        public async static Task<CommandResponse> SendCommandAsync(string commandEndpoint ,object commandPayload)
        {
            var content = new StringContent(JsonSerializer.Serialize(commandPayload), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsync(commandEndpoint, content);
                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CommandResponse>(
                    jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
        }
    }
}
