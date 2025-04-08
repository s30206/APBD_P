using System.Text;

namespace APBD_P1;

public class DeviceManager : IDeviceManager
{
    public List<Device> Devices { get; set; }
    public const int MaxDevices = 15;

    public DeviceManager(string FilePath) 
    {
        if (!File.Exists(FilePath))
            throw new FileNotFoundException("File not found", FilePath);
        
        Devices = new List<Device>();
        
        foreach (var line in File.ReadLines(FilePath))
        {
            try
            {
                var parts = line.Split(',');
                var device = parts[0].Split('-');
                if (device.Length != 2 || Devices.Count >= MaxDevices) continue;
                if (device[0] == "SW")
                {
                    if (parts.Length == 4)
                    {
                        var sw = new Smartwatch();
                        sw.Id = int.Parse(device[1]);
                        sw.Name = parts[1];
                        sw.IsOn = bool.Parse(parts[2]);
                        sw.BatteryPercentage = int.Parse(parts[3].Remove(parts[3].Length - 1));
                        Devices.Add(sw);
                    }
                }
                else if (device[0] == "P")
                {
                    if (parts.Length >= 3)
                    {
                        var p = new PersonalComputer();
                        p.Id = int.Parse(device[1]);
                        p.Name = parts[1];
                        p.IsOn = bool.Parse(parts[2]);
                        p.OperatingSystem = parts.Length == 3 ? "" : parts[3];
                        Devices.Add(p);
                    }
                }
                else if (device[0] == "ED")
                {
                    if (parts.Length == 4)
                    {
                        var ed = new EmbeddedDevice();
                        ed.Id = int.Parse(device[1]);
                        ed.Name = parts[1];
                        ed.IpAddress = parts[2];
                        ed.NetworkName = parts[3];
                        Devices.Add(ed);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
            }
        }
    }

    public bool AddDevice(Device device)
    {
        if (Devices.Contains(device) || Devices.Count >= MaxDevices) return false;
        
        try
        {
            Devices.Add(device);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when adding a device: {e.Message}");
            return false;
        }
    }

    public bool RemoveDevice(Device device)
    {
        try
        {
            Devices.Remove(device);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when deleting a device: {e.Message}");
            return false;
        }
    }

    public void TurnOnDevice(Device device)
    {
        try
        {
            device.TurnOn();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when turning on a device: {e.Message}");
        }
    }

    public void TurnOffDevice(Device device)
    {
        try
        {
            device.TurnOff();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when turning off a device: {e.Message}");
        }
    }

    public string ReturnAllDevices()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var device in Devices) 
            sb.Append(device);
        return sb.ToString();
    }

    public bool EditData(Device device, Device newDevice)
    {
        try
        {
            if (device is Smartwatch sw1 && newDevice is Smartwatch sw2)
            {
                sw1.Name = sw2.Name;
                sw1.IsOn = sw2.IsOn;
                sw1.BatteryPercentage = sw2.BatteryPercentage;
                return true;
            }
            if (device is PersonalComputer p1 && newDevice is PersonalComputer p2)
            {
                p1.Name = p2.Name;
                p1.IsOn = p2.IsOn;
                p1.OperatingSystem = p2.OperatingSystem;
                return true;
            }
            if (device is EmbeddedDevice ed1 && newDevice is EmbeddedDevice ed2)
            {
                ed1.Name = ed2.Name;
                ed1.IpAddress = ed2.IpAddress;
                ed1.NetworkName = ed2.NetworkName;
                return true;
            }
            
            throw new ArgumentException("The device type is not the same as the new device");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when editing a device: {e.Message}");
            return false;
        }
    }

    public void SaveData(string filePath)
    {
        try
        {
            var lines = new List<string>();
                
            foreach (var device in Devices)
            {
                if (device is Smartwatch sw)
                {
                    lines.Add($"SW-{sw.Id},{sw.Name},{sw.IsOn},{sw.BatteryPercentage}%");
                }
                else if (device is PersonalComputer pc)
                {
                    string pcData = string.IsNullOrEmpty(pc.OperatingSystem) 
                        ? $"P-{pc.Id},{pc.Name},{pc.IsOn}"
                        : $"P-{pc.Id},{pc.Name},{pc.IsOn},{pc.OperatingSystem}";
                    lines.Add(pcData);
                }
                else if (device is EmbeddedDevice ed)
                {
                    lines.Add($"ED-{ed.Id},{ed.Name},{ed.IpAddress},{ed.NetworkName}");
                }
            }
                
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error saving data: {e.Message}");
        }
    }

    public void test()
    {
        foreach (var d in Devices)
        {
            Console.WriteLine(d.GetType().Name);
        }
    }

    public Device? GetById(string id)
    {
        foreach (var d in Devices)
        {
            var split = id.Split('-');
            var dType = getDeviceShortName(d.GetType().Name);
            
            if (dType.Equals(split[0]) && d.Id == Int32.Parse(split[1]))
            {
                return d;
            }
        }

        return null;
    }

    public bool RemoveDeviceById(string id)
    {
        var device = GetById(id);
        if (device == null) return false;
        
        Devices.Remove(device);
        return true;
    }

    public bool EditDataById(string id, Device newDevice)
    {
        var device = GetById(id);
        if (device == null) return false;
        
        EditData(newDevice, device);
        return true;
    }

    private string? getDeviceShortName(string deviceName)
    {
        switch (deviceName)
        {
            case "Smartwatch":
                return "SW";
            case "PersonalComputer":
                return "P";
            case "EmbeddedDevice":
                return "ED";
            default:
                return null;
        }
    }
}