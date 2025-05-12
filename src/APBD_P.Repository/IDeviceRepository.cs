using APBD_P.Source.Parsers;
using APBD_P1;

namespace APBD_P.Repository;

public interface IDeviceRepository
{
    public List<DeviceDB> GetAllDevices();
    public Device? GetDeviceById(string id, string devType);
    public bool AddDevice(Device device);
    public bool UpdateDevice(string id, Device device);
    public bool DeleteDevice(string id, string devType);
}