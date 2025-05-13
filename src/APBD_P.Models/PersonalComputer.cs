namespace APBD_P1;

public class PersonalComputer : Device
{
    public string OperationSystem { get; set; }
    
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperationSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        IsEnabled = true;
    }

    public PersonalComputer()
    {
        
    }

    public PersonalComputer(int Id, string Name, bool isEnabled, string OperatingSystem) : base(Id, Name, isEnabled)
    {
        OperatingSystem = OperatingSystem;
    }

    public override string ToString() =>
        $"PC: ID: {Id},\nName:{Name},\nOperating system: {OperationSystem},\nIsOn:{IsEnabled}";
}