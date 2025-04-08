namespace APBD_P1;

public interface IDeviceManager : IDeviceModification
{
    List<Device> Devices { get; set; }

    public Device? GetById(string id);
    public bool RemoveDeviceById(string id);
    public bool EditDataById(string id, Device newDevice);
    void SaveData(string filePath);
    string ReturnAllDevices();
}