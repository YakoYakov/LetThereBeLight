using System.Net;
using System.Net.Sockets;

namespace LetThereBeLight.Services.Helpers
{
    public static class NetworkUtils
    {
        public static string GetLocalIp()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint ??
                    throw new ArgumentNullException("No local Ip found");

                return endPoint.Address.ToString();
            }
        }
    }
}
