using APBD_P1;

namespace APBD_P.Database;

public interface IDeviceService
{
    IEnumerable<DeviceDatabase> GetAllDevices();
    Device GetDeviceById(string id);
    bool AddDevice(Device device);
    
}