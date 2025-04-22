namespace APBD_P1;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOn { get; set; }

    public Device()
    {
        
    }
    
    public Device(int Id, string Name, bool IsOn)
    {
        this.Id = Id;
        this.Name = Name;
        this.IsOn = IsOn;
    }

    public void TurnOn() => IsOn = true;
    public void TurnOff() => IsOn = false;

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, IsOn: {IsOn}";
    }
}