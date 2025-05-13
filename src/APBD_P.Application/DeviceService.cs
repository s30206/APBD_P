using APBD_P.Repository;
using APBD_P.Source.Interfaces;
using APBD_P1;

namespace APBD_P.Source.Service;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public List<DeviceDB> GetAllDevices()
    {
        var result = _deviceRepository.GetAllDevices();
        return result;
    }

    public Device? GetDeviceById(string id)
    {
        var devType = id.Split('-')[0] switch
        {
            "ED" => nameof(Embedded),
            "P" => nameof(PersonalComputer),
            "SW" => nameof(Smartwatch),
            _ => null
        };
        
        if (devType == null) return null;
        
        var result = _deviceRepository.GetDeviceById(id, devType);
        return result;
    }

    public bool AddDevice(Device device)
    {
        if (device == null) return false;
        
        var result = _deviceRepository.AddDevice(device);
        return result;
    }

    public bool UpdateDevice(string id, Device device)
    {
        var result = _deviceRepository.UpdateDevice(id, device);
        return result;
    }

    public bool DeleteDevice(string id)
    {
        var devType = id.Split('-')[0] switch
        {
            "ED" => nameof(Embedded),
            "P" => nameof(PersonalComputer),
            "SW" => nameof(Smartwatch),
            _ => null
        };
        
        if (devType == null) return false;

        var result = _deviceRepository.DeleteDevice(id, devType);
        return result;
    }
}