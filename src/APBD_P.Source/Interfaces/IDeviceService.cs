using APBD_P1;

namespace APBD_P.Source.Interfaces;

public interface IDeviceService
{
    IEnumerable<DeviceDB> GetAllDevices();
    Device? GetDeviceById(string id);
    bool AddDevice(Device device);
    Device? UpdateDevice(string id, Device device);
    bool DeleteDevice(string id);
}