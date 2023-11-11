namespace LetThereBeLight.Services
{
    public class NetworkService
    {
        // https://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf
        private const string dgram = "M-SEARCH * HTTP/1.1\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb\r\n"; //Yeelight udp datagram
        private const string MULTICAST_ADDRESS = "239.255.255.250"; //Yeelight multicast address
        private const int DEVICE_PORT = 1982; //Yeelight comm port


    }
}