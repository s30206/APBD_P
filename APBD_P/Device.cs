namespace APBD_P1;

public abstract class Device
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOn { get; set; }

    public abstract void TurnOn();
    public void TurnOff() => IsOn = false;
    public abstract override string ToString();
}