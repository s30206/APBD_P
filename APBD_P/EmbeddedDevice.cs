using System.Text.RegularExpressions;

namespace APBD_P1;

public class EmbeddedDevice : Device
{
    public static Regex IpRegex = new Regex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$");
    
    private string _ipAddress;
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (!IpRegex.IsMatch(value))
                throw new ArgumentException("Invalid IP address");
            _ipAddress = value;
        }
    }
    
    public string NetworkName { get; set; }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new Exception("ConnectionException: Network Name must be MD Ltd.");
    }

    public override void TurnOn()
    {
        if (IsOn)
            return;
        Connect();
        IsOn = true;
    }

    public override string ToString() => 
        $"Embedded device: ID: {Id},\nName:{Name},\nIP address: {IpAddress},\nNetwork name: {NetworkName},\nIsOn:{IsOn}";
}

