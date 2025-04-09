using APBD_P1;

namespace WebApplication1;

public class DeviceTemplate : Device
{
    public string Type { get; set; }
    public string IpAddress { get; set; }
    public string NetworkName { get; set; }
    public string OperatingSystem { get; set; }
    public int BatteryPercentage { get; set; }

    public DeviceTemplate(string Type, int Id, string Name, bool IsOn, string IpAddress, 
        string NetworkName, string OperatingSystem, int BatteryPercentage) : base(Id , Name, IsOn)
    {
        this.Type = Type;
        this.IpAddress = IpAddress;
        this.NetworkName = NetworkName;
        this.OperatingSystem = OperatingSystem;
        this.BatteryPercentage = BatteryPercentage;
    }
}