namespace APBD_P1;

public class DeviceManagerFactory
{
    public static IDeviceManager CreateDeviceManager(string filePath) => new DeviceManager(filePath);
}