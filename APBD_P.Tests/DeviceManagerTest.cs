using System.IO;
using JetBrains.Annotations;
using Xunit;

namespace APBD_P1.Tests;

[TestSubject(typeof(DeviceManager))]
public class DeviceManagerTest
{
    private string _inputPath = "inputTest.txt";
    private string _outputPath = "outputTest.txt";
    
    [Fact]
    public void TestConstructorMethod()
    {
        IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(_inputPath);
        
        int expectedDeviceCount = 5;
        Assert.Equal(manager.Devices.Count, expectedDeviceCount);
    }
    
    [Fact]
    public void TestTurnOnOffDeviceMethod()
    {
        IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(_inputPath);
        
        manager.TurnOnDevice(manager.Devices[0]);
        Assert.True(manager.Devices[0].IsOn);
        
        manager.TurnOffDevice(manager.Devices[0]);
        Assert.False(manager.Devices[0].IsOn);
    }
    
    [Fact]
    public void TestAddDeleteDeviceMethod()
    {
        IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(_inputPath);
        
        int beforeCount = manager.Devices.Count;
        manager.AddDevice(manager.Devices[0]);
        Assert.Equal(beforeCount, manager.Devices.Count);
        
        var sw = new Smartwatch();
        manager.AddDevice(sw);
        Assert.NotEqual(beforeCount, manager.Devices.Count);
        
        manager.RemoveDevice(sw);
        Assert.Equal(beforeCount, manager.Devices.Count);
        
        manager.RemoveDevice(sw);
        Assert.Equal(beforeCount, manager.Devices.Count);
    }
    
    [Fact]
    public void TestEditDeviceMethod()
    {
        IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(_inputPath);

        Smartwatch manSw = (Smartwatch) manager.Devices[0];
        
        var sw = new Smartwatch();
        sw.Name = "Watch1";
        sw.IsOn = true;
        sw.BatteryPercentage = 100;

        Assert.NotEqual(sw.Name, manSw.Name);
        Assert.NotEqual(sw.IsOn, manSw.IsOn);
        Assert.NotEqual(sw.BatteryPercentage, manSw.BatteryPercentage);
        
        manager.EditData(manSw, sw);
        
        manSw = (Smartwatch) manager.Devices[0];
        
        Assert.Equal(sw.Name, manSw.Name);
        Assert.Equal(sw.IsOn, manSw.IsOn);
        Assert.Equal(sw.BatteryPercentage, manSw.BatteryPercentage);
    }
    
    [Fact]
    public void TestSaveDataMethod()
    {
        File.WriteAllText(_outputPath, string.Empty);
        
        IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(_inputPath);
        manager.SaveData(_outputPath);
        
        string[] lines = File.ReadAllLines(_outputPath);
        Assert.NotEmpty(lines);
        Assert.Equal(lines.Length, manager.Devices.Count);
    }
}