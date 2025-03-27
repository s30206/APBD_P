namespace APBD_P1;

public interface IDeviceModification
{
    void AddDevice(IDevice device);
    void RemoveDevice(IDevice device);
    void TurnOnDevice(IDevice device);
    void TurnOffDevice(IDevice device);
    void EditData(IDevice device, IDevice newDevice);
}