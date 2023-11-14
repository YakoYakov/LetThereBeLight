using LetThereBeLight.Devices.Enums;

namespace LetThereBeLight.Devices
{
    public class CommandModel
    {
        public Method Method { get; set; }
        public List<object> Params { get; set; } = new List<object>();
    }
}
