using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace LetThereBeLight.Services.Helpers
{
    public static class NetworkHelper
    {
        // Returns local ip from a Socket
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

        //Return local ip address from full address
        public static string getAddress(string fullAddress)
        {
            Regex regex = new Regex(@"\d{3}.\d{3}.\d{1,3}.\d{1,3}");

            Match match = regex.Match(fullAddress);

            if (match.Success)
            {
                return match.Value;
            }

            return String.Empty;
        }

        //Return port number from full address
        public static int getPort(string fullAddress)
        {
            Regex regex = new Regex(@":(\d{1,5})");

            Match match = regex.Match(fullAddress);

            try
            {
                if (match.Success)
                    return int.Parse(match.Groups[1].Value);

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}
