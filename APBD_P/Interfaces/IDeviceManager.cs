namespace APBD_P1;

public interface IDeviceManager : IDeviceModification
{
    List<IDevice> Devices { get; set; }
    
    void SaveData(string filePath);
    void ShowAllDevices();
}