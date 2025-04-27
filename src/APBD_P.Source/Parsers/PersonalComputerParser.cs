using System.Text.Json;
using System.Text.Json.Nodes;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class PersonalComputerParser : IDeviceParser
{
    protected JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    
    public Device ParseDevice(SqlDataReader reader)
    {
        return new PersonalComputer
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsOn = reader.GetBoolean(2),
            OperatingSystem = reader.GetString(5)
        };
    }

    public Device? ParseJsonDevice(JsonNode json)
    {        
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<PersonalComputer>(json.ToString(), _options);
    }

    public bool InsertDevice(Device device, SqlConnection conn)
    {
        PersonalComputer dev = (PersonalComputer)device;
        string query = "Insert into PersonalComputer (ID, Device_ID, OperatingSystem) values (@ID, @Device_ID, @OperatingSystem)";

        string? shortName = "P";

        string queryMax = "select coalesce(max(id), 1) from PersonalComputer";

        var command = new SqlCommand(queryMax, conn);
        var reader = command.ExecuteReader();
        reader.Read();
        int maxId = reader.GetInt32(0);
        reader.Close();
        
        command = new SqlCommand(query, conn);
        command.Parameters.AddWithValue("@ID", maxId);
        command.Parameters.AddWithValue("@Device_ID", $"{shortName}-{dev.Id}");
        command.Parameters.AddWithValue("@OperatingSystem", dev.OperatingSystem);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }
}