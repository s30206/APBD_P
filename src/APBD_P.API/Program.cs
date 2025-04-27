using APBD_P.Database;
using APBD_P1;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddSingleton<IDeviceService, DeviceService>(s => new DeviceService(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", (IDeviceService deviceService) =>
    Results.Ok(deviceService.GetAllDevices()));

app.MapGet("/api/devices/{id}", (string id, IDeviceService deviceService) =>
{
    Device? dev = deviceService.GetDeviceById(id);
    return dev == null ? Results.NotFound() : Results.Ok(dev);
});

/*app.MapPut("/api/devices/{id}", (string id, [FromBody] DeviceTemplate newDevice) =>
{
    Device d = null;
    switch (newDevice.Type)
    {
        case "SW":
            d = new Smartwatch
            {
                Id = newDevice.Id,
                Name = newDevice.Name,
                IsOn = newDevice.IsOn,
                BatteryPercentage = newDevice.BatteryPercentage,
            };
            break;
        case "P":
            d = new PersonalComputer
            {
                Id = newDevice.Id,
                Name = newDevice.Name,
                IsOn = newDevice.IsOn,
                OperatingSystem = newDevice.OperatingSystem
            };
            break;
        case "ED":
            d = new EmbeddedDevice
            {
                Id = newDevice.Id,
                Name = newDevice.Name,
                IsOn = newDevice.IsOn,
                IpAddress = newDevice.IpAddress,
                NetworkName = newDevice.NetworkName
            };
            break;
    }

    if (d is null)
    {
        return Results.BadRequest();
    }
    
    return manager.EditDataById(id, d)
        ? Results.Ok(d)
        : Results.NotFound();
});*/

app.MapDelete("/api/devices/{id}", (string id, IDeviceService deviceService) => 
    deviceService.DeleteDevice(id) ? Results.Ok() : Results.NotFound());

app.Run();