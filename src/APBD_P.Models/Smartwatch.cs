namespace APBD_P1;

public class Smartwatch : Device, IPowerNotifier
{
    private int _batteryPercentage;
    public int BatteryPercentage
    {
        get => _batteryPercentage;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("Battery Percentage must be between 0 and 100.");
            _batteryPercentage = value;
            if (_batteryPercentage < 20)
                NotifyPower();
        }
    }
    
    public void NotifyPower()
    {
        Console.WriteLine($"Warning, SW-{Id} {Name} has low battery percentage: {_batteryPercentage}");
    }

    public override void TurnOn()
    {
        if (BatteryPercentage < 11)
            throw new Exception("EmptyBatteryException: Battery too low to turn on");
        if (IsEnabled)
            return;
        BatteryPercentage -= 10;
        IsEnabled = true;
    }

    public Smartwatch()
    {
        
    }
    
    public Smartwatch(int Id, string Name, bool isEnabled, int batteryPercentage) : base(Id, Name, isEnabled)
    {
        BatteryPercentage = batteryPercentage;
    }

    public override string ToString() => $"Smartwatch: ID: {Id},\nName:{Name},\nBattery: {BatteryPercentage},\nIsOn:{IsEnabled}";
}