namespace APBD_P1;

public interface IDevice
{
    int Id { get; set; }
    string Name { get; set; }
    bool IsOn { get; set; }
    
    void TurnOn();
    void TurnOff();
}