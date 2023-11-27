using SQLite;

namespace LetThereBeLightApp.Models
{
    public class SmartBulb
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Power { get; set; } = string.Empty;
        [MaxLength(100)]
        public int Brightness { get; set; }
        public int ColorMode { get; set; }

        //Range between 2700K and 6500K
        public int ColorTemperature { get; set; }
        public int RGB { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
