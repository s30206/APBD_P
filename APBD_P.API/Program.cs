using APBD_P1;
using Microsoft.AspNetCore.Mvc;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string inputPath = "../APBD_P.Source/TextFiles/input.txt";

IDeviceManager manager = DeviceManagerFactory.CreateDeviceManager(inputPath);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", () =>
    Results.Ok(manager.ReturnAllDevices()));

app.MapGet("/api/devices/{id}", (string id) =>
{
    var device = manager.GetById(id);
    return device is null ? Results.NotFound() : Results.Ok(device);
});

app.MapPost("/api/devices", ([FromBody] DeviceTemplate device) =>
{
    try
    {
        Device d = null;
        switch (device.Type)
        {
            case "SW":
                d = new Smartwatch
                {
                    Id = device.Id,
                    Name = device.Name,
                    IsOn = device.IsOn,
                    BatteryPercentage = device.BatteryPercentage,
                };
                break;
            case "P":
                d = new PersonalComputer
                {
                    Id = device.Id,
                    Name = device.Name,
                    IsOn = device.IsOn,
                    OperatingSystem = device.OperatingSystem
                };
                break;
            case "ED":
                d = new EmbeddedDevice
                {
                    Id = device.Id,
                    Name = device.Name,
                    IsOn = device.IsOn,
                    IpAddress = device.IpAddress,
                    NetworkName = device.NetworkName
                };
                break;
        }

        if (d is null)
        {
            return Results.BadRequest();
        }
        
        manager.AddDevice(device);
        return Results.Created($"/api/devices/{device.Id}", d);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapPut("/api/devices/{id}", (string id, [FromBody] DeviceTemplate newDevice) =>
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
});

app.MapDelete("/api/devices/{id}", (string id) => manager.RemoveDeviceById(id)
    ? Results.Ok()
    : Results.NotFound());

app.Run();