using APBD_P1;

namespace APBD_P.Database;

public interface IDeviceService
{
    IEnumerable<Device> GetAllDevices();
    bool AddDevice(Device device);
    
}