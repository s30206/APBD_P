using System.Text.Json;
using System.Text.Json.Nodes;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class EmbeddedDeviceParser : IDeviceParser
{
    protected JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    
    public Device ParseDevice(SqlDataReader reader)
    {
        return new EmbeddedDevice
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsOn = reader.GetBoolean(2),
            IpAddress = reader.GetString(5),
            NetworkName = reader.GetString(6)
        };
    }

    public Device? ParseJsonDevice(JsonNode json)
    {
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<EmbeddedDevice>(json.ToString(), _options);
    }

    public Device? ParseTextDevice(string[] parts)
    {
        return new EmbeddedDevice()
        {
            Id = Int32.Parse(parts[0].Trim().Split("-")[1]),
            Name = parts[1].Trim(),
            IsOn = bool.Parse(parts[2].Trim()),
            IpAddress = parts[3].Trim(),
            NetworkName = parts[4].Trim()
        };
    }

    public bool InsertDevice(Device device, SqlConnection conn, SqlTransaction transaction)
    {
        EmbeddedDevice dev = (EmbeddedDevice)device;
        string query = "Insert into EmbeddedDevice (ID, Device_ID, IpAddress, NetworkName) values (@ID, @Device_ID, @IpAddress, @NetworkName)";
        
        string queryMax = "select coalesce(max(id), 1) from EmbeddedDevice";

        var command = new SqlCommand(queryMax, conn, transaction);
        var reader = command.ExecuteReader();
        reader.Read();
        int maxId = reader.GetInt32(0);
        reader.Close();

        string shortName = "ED";
        command = new SqlCommand(query, conn, transaction);
        command.Parameters.AddWithValue("@ID", maxId);
        command.Parameters.AddWithValue("@Device_ID", $"{shortName}-{dev.Id}");
        command.Parameters.AddWithValue("@IpAddress", dev.IpAddress);
        command.Parameters.AddWithValue("@NetworkName", dev.NetworkName);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }

    public bool UpdateDevice(string id, Device device, SqlConnection conn, SqlTransaction transaction)
    {
        EmbeddedDevice dev = (EmbeddedDevice)device;
        string query = "UPDATE EmbeddedDevice set IpAddress = @IpAddress, NetworkName = @NetworkName WHERE Device_ID = @Id";
        
        var command = new SqlCommand(query, conn, transaction);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@IpAddress", dev.IpAddress);
        command.Parameters.AddWithValue("@NetworkName", dev.NetworkName);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }
}