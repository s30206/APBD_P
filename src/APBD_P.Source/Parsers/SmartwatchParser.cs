using System.Data;
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
            IsEnabled = reader.GetBoolean(2),
            BatteryPercentage = reader.GetInt32(5)
        };

    }

    public Device? ParseJsonDevice(JsonNode json)
    {
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<Smartwatch>(json.ToString(), _options);
    }

    public Device? ParseTextDevice(string[] parts)
    {
        return new Smartwatch()
        {
            Id = Int32.Parse(parts[0].Trim().Split("-")[1]),
            Name = parts[1].Trim(),
            IsEnabled = bool.Parse(parts[2].Trim()),
            BatteryPercentage = Int32.Parse(parts[3].Trim())
        };
    }
    
    public bool InsertDevice(Device device, SqlConnection conn, SqlTransaction transaction)
    {
        Smartwatch dev = (Smartwatch)device;
        
        string shortName = "SW";
        var command = new SqlCommand("AddSmartwatch", conn, transaction);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@DeviceID", $"{shortName}-{dev.Id}");
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", dev.IsEnabled);
        command.Parameters.AddWithValue("@BatteryPercentage", dev.BatteryPercentage);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }

    public bool UpdateDevice(string id, Device device, SqlConnection conn, SqlTransaction transaction)
    {
        Smartwatch dev = (Smartwatch)device;
        string query = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceID = @Id";
        
        var command = new SqlCommand(query, conn, transaction);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@BatteryPercentage", dev.BatteryPercentage);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }
}