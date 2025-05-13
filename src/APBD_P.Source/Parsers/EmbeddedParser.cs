using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class EmbeddedParser : IDeviceParser
{
    protected JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };
    
    public Device ParseDevice(SqlDataReader reader)
    {
        return new Embedded
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsEnabled = reader.GetBoolean(2),
            IpAddress = reader.GetString(5),
            NetworkName = reader.GetString(6)
        };
    }

    public Device? ParseJsonDevice(JsonNode json)
    {
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<Embedded>(json.ToString(), _options);
    }

    public Device? ParseTextDevice(string[] parts)
    {
        return new Embedded()
        {
            Id = Int32.Parse(parts[0].Trim().Split("-")[1]),
            Name = parts[1].Trim(),
            IsEnabled = bool.Parse(parts[2].Trim()),
            IpAddress = parts[3].Trim(),
            NetworkName = parts[4].Trim()
        };
    }

    public bool InsertDevice(Device device, SqlConnection conn, SqlTransaction transaction)
    {
        Embedded dev = (Embedded)device;

        string shortName = "ED";
        var command = new SqlCommand("AddEmbedded", conn, transaction);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@DeviceID", $"{shortName}-{dev.Id}");
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", dev.IsEnabled);
        command.Parameters.AddWithValue("@IpAddress", dev.IpAddress);
        command.Parameters.AddWithValue("@NetworkName", dev.NetworkName);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }

    public bool UpdateDevice(string id, Device device, SqlConnection conn, SqlTransaction transaction)
    {
        var rowVersionQuery =
            "select device.DeviceVersion as DeviceRaw, s.EmbeddedVersion as EDRaw from device join Embedded s on device.ID = s.DeviceID where s.DeviceID = @id";

        byte[] deviceRaw = null;
        byte[] edRaw = null;
        
        using (var command = new SqlCommand(rowVersionQuery, conn, transaction))
        {
            command.Parameters.AddWithValue("@id", id);
            
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                deviceRaw = (byte[])reader["DeviceRaw"];
                edRaw = (byte[])reader["EDRaw"];
            }
            else
            {
                throw new Exception("Embedded device not found");
            }
            reader.Close();
        }
        
        Embedded dev = (Embedded)device;
        string updateDevice = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE ID = @ID and DeviceVersion = @DeviceVersion";
        using (var command = new SqlCommand(updateDevice, conn, transaction))
        {
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@Name", dev.Name);
            command.Parameters.AddWithValue("@IsEnabled", dev.IsEnabled);
            command.Parameters.Add("@DeviceVersion", SqlDbType.Timestamp).Value = deviceRaw;
            
            if (command.ExecuteNonQuery() == 0)
                throw new Exception("Device update failed");
        }
        
        string query = "UPDATE Embedded set IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceID = @Id and EmbeddedVersion = @EDVersion";
        using (var command = new SqlCommand(query, conn, transaction))
        {
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@IpAddress", dev.IpAddress);
            command.Parameters.AddWithValue("@NetworkName", dev.NetworkName);
            command.Parameters.Add("EDVersion", SqlDbType.Timestamp).Value = edRaw;

            int rowsAffected = command.ExecuteNonQuery();
        }

        return true;
    }
}