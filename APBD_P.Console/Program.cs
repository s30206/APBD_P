using APBD_P1;

// Relative paths work in API but not here. No idea why :D
string inputPath = "D:\\СSProjects\\APBD_P\\APBD_P.Source\\TextFiles\\input.txt";
string outputPath = "D:\\СSProjects\\APBD_P\\APBD_P.Source\\TextFiles\\output.txt";

IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(inputPath);
Console.WriteLine(manager.ReturnAllDevices()); // 5 devices because 2 lines in the input file have wrong data

Console.WriteLine("Smartwatch");
manager.TurnOffDevice(manager.Devices[0]);
manager.TurnOnDevice(manager.Devices[0]); // Receive warning about low battery

Console.WriteLine("Embedded device");
manager.TurnOnDevice(manager.Devices[3]); // No errors
manager.TurnOnDevice(manager.Devices[4]); // Errors (No MD Ltd.)

Console.WriteLine("Personal computer");
manager.TurnOnDevice(manager.Devices[1]); // No errors
manager.TurnOnDevice(manager.Devices[2]); // Errors (no OS)

Console.WriteLine($"Before adding a device: {manager.Devices.Count}");
manager.AddDevice(new Smartwatch());
Console.WriteLine($"After adding a device: {manager.Devices.Count}");
manager.RemoveDevice(manager.Devices[manager.Devices.Count - 1]);
Console.WriteLine($"After removing a device: {manager.Devices.Count}");

Console.WriteLine("Deleting all data from the output file");
File.WriteAllText(outputPath, string.Empty);
manager.SaveData(outputPath);
Console.WriteLine($"Saved data:\n{File.ReadAllText(outputPath)}");