﻿namespace APBD_P1;

public class PersonalComputer : Device
{
    public string OperatingSystem { get; set; }
    
    public void TurnOn()
    {
        if (string.IsNullOrEmpty(OperatingSystem))
            throw new Exception("EmptySystemException: No OS installed.");
        IsOn = true;
    }

    public PersonalComputer()
    {
        
    }

    public PersonalComputer(int Id, string Name, bool IsOn, string OperatingSystem) : base(Id, Name, IsOn)
    {
        OperatingSystem = OperatingSystem;
    }

    public override string ToString() =>
        $"PC: ID: {Id},\nName:{Name},\nOperating system: {OperatingSystem},\nIsOn:{IsOn}";
}