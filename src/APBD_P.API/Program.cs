using System.Text.Json.Nodes;
using APBD_P.Database.Parsers;
using APBD_P.Source.DeviceService;
using APBD_P.Source.Interfaces;
using APBD_P.Source.Parsers;
using APBD_P1;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});

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

app.MapPut("api/devices", async (HttpRequest request, IDeviceService deviceService) =>
{
    string? contentType = request.ContentType?.ToString();

    switch (contentType)
    {
        case "application/json":
        {
            try
            {
                var result = DeserializeJsonDevice(request);

                if (result == null) return Results.BadRequest();

                
                return deviceService.AddDevice(result) ? 
                    Results.Created("/api/devices/", result.Id) : Results.BadRequest();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
        case "text/plain":
        {
            return Results.BadRequest();
        }
        default:
        {
            return Results.BadRequest();
        }
    }
    return Results.Ok();
}).Accepts<string>("application/json", ["text/plain"]);

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

Device? DeserializeJsonDevice(HttpRequest request)
{
    using var reader = new StreamReader(request.Body);
    string rawJson = reader.ReadToEnd();
    var json = JsonNode.Parse(rawJson);

    IDeviceParser? parserJson = json["id"].ToString().Split("-")[0] switch
    {
        "ED" => new EmbeddedDeviceParser(),
        "P" => new PersonalComputerParser(),
        "SW" => new SmartwatchParser(),
        _ => null
    };

    if (parserJson == null) return null;
    return parserJson.ParseJsonDevice(json);
}