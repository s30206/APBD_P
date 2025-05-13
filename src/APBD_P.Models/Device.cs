namespace APBD_P1;

public abstract class Device
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    public Device()
    {
        
    }
    
    public Device(int Id, string Name, bool isEnabled)
    {
        this.Id = Id;
        this.Name = Name;
        this.IsEnabled = isEnabled;
    }

    public abstract void TurnOn();
    public void TurnOff() => IsEnabled = false;

    public abstract override string ToString();
}