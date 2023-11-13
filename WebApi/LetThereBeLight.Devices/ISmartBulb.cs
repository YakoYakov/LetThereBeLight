namespace LetThereBeLight.Devices
{
    public interface ISmartBulb
    {
        bool SendCommand(SmartBulb device, int id, string method, dynamic[] parameters);
    }
}
