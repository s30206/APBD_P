namespace APBD_P1;

public class PersonalComputer : Device
{
    public string OperatingSystem { get; set; }
    
    public override void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        IsOn = true;
    }

    public override string ToString() =>
        $"PC: ID: {Id},\nName:{Name},\nOperating system: {OperatingSystem},\nIsOn:{IsOn}";
}