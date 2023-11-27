namespace LetThereBeLightApp.Models
{
    public class DiscoveryResponse
    {
        public DeviceProperties deviceProperties { get; set; }
    }
    public class DeviceProperties
    {
        public int id { get; set; }
        public string location { get; set; }
        public int power { get; set; }
        public int brightness { get; set; }
        public int colorMode { get; set; }
        public int colorTemperature { get; set; }
        public int rgb { get; set; }
        public int hue { get; set; }
        public int saturation { get; set; }
        public string name { get; set; }
    }

}
