namespace LetThereBeLightApp.Constants
{
    public class Constants
    {
        private const string API_SERVICE_BASE_URL = "http://192.168.0.106/api/SmartBulb";

        public const string DISCOVERY_ENDPOINT = API_SERVICE_BASE_URL + "/discover-devices?timeOut={0}";

        public const string TOGGLE_ENDPOINT = API_SERVICE_BASE_URL + "/toggle";

        public const string CHANGE_BRIGHTNESS_ENDPOINT = API_SERVICE_BASE_URL + "/brightness";

        public const string CHANGE_COLOR_TEMPERATURE_ENDPOINT = API_SERVICE_BASE_URL + "/color-temperature";

        public const string CHANGE_RGB_ENDPOINT = API_SERVICE_BASE_URL + "/rgb";

        public const string CHANGE_NAME_ENDPOINT = API_SERVICE_BASE_URL + "/name";
    }
}
