using APBD_P1;

namespace APBD_P.Source.Interfaces;

public interface IDeviceService
{
    List<DeviceDB> GetAllDevices();
    Device? GetDeviceById(string id);
    bool AddDevice(Device device);
    bool UpdateDevice(string id, Device device);
    bool DeleteDevice(string id);
}