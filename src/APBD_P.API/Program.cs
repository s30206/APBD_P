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
                /* Example
                 * {
                 *   "id": "SW-2",
                 *   "name": "Test",
                 *   "ison": true,
                 *   "batterypercentage": 88
                 * }
                 */
                var result = DeserializeJsonDevice(request);

                if (result == null) return Results.BadRequest();
                
                return deviceService.AddDevice(result) ? 
                    Results.Created("/api/devices/", result.Id) : Results.BadRequest();
            }
            catch (Exception ex)
            {
                return Results.BadRequest();
            }
        }
        case "text/plain":
        {
            StreamReader reader = new StreamReader(request.Body);
            string text = await reader.ReadToEndAsync();

            try
            {
                /* Example
                 * SW-2, Test, true, 88
                 */
                var parts = text.Split(",");
                IDeviceParser? parser = parts[0].Trim().Split("-")[0] switch
                {
                    "ED" => new EmbeddedDeviceParser(),
                    "P" => new PersonalComputerParser(),
                    "SW" => new SmartwatchParser(),
                    _ => null
                };
                
                if (parser == null) return Results.BadRequest();

                var result = parser.ParseTextDevice(parts);
                
                if (result == null) return Results.BadRequest();
                
                return deviceService.AddDevice(result) ? 
                    Results.Created("/api/devices/", result.Id) : Results.BadRequest();
            }
            catch (Exception ex)
            {
                return Results.BadRequest();
            }
        }
        default:
        {
            return Results.BadRequest();
        }
    }
}).Accepts<string>("application/json", ["text/plain"]);

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

    return parserJson?.ParseJsonDevice(json);
}