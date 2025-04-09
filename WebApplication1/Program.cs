using APBD_P1;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string inputPath = "../APBD_P/TextFiles/input.txt";

builder.Services.AddSingleton(DeviceManagerFactory.CreateDeviceManager(inputPath));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", (IDeviceManager manager) =>
    Results.Ok(manager.ReturnAllDevices()));

app.MapGet("/api/devices/{id}", (string id, IDeviceManager manager) =>
{
    var device = manager.GetById(id);
    return device is null ? Results.NotFound() : Results.Ok(device);
});

app.MapPost("/api/devices", ([FromBody] Device device, IDeviceManager manager) =>
{
    try
    {
        manager.AddDevice(device);
        return Results.Created($"/api/devices/{device.Id}", device);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapPut("/api/devices/{id}", (string id, [FromBody] Device newDevice, IDeviceManager manager) => manager.EditDataById(id, newDevice)
    ? Results.Ok(manager.GetById(id))
    : Results.NotFound());

app.MapDelete("/api/devices/{id}", (string id, IDeviceManager manager) => manager.RemoveDeviceById(id)
    ? Results.Ok()
    : Results.NotFound());

app.Run();