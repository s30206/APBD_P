using System.Text.Json;
using System.Text.Json.Nodes;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class SmartwatchParser : IDeviceParser
{
    protected JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public Device ParseDevice(SqlDataReader reader)
    {
        return new Smartwatch
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsOn = reader.GetBoolean(2),
            BatteryPercentage = reader.GetInt32(5)
        };

    }

    public Device? ParseJsonDevice(JsonNode json)
    {
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<Smartwatch>(json.ToString(), _options);
    }

    public bool InsertDevice(Device device, SqlConnection conn)
    {
        throw new NotImplementedException();
    }
}