namespace APBD_P1;

public interface IDeviceModification
{
    bool AddDevice(Device device);
    bool RemoveDevice(Device device);
    void TurnOnDevice(Device device);
    void TurnOffDevice(Device device);
    bool EditData(Device device, Device newDevice);
}