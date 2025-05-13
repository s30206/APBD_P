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
        var rowVersionQuery =
            "select device.DeviceVersion as DeviceRaw, s.SWVersion as SWRaw from device join Smartwatch s on device.ID = s.DeviceID where s.DeviceID = @id";

        byte[] deviceRaw = null;
        byte[] swRaw = null;
        
        using (var command = new SqlCommand(rowVersionQuery, conn, transaction))
        {
            command.Parameters.AddWithValue("@id", id);
            
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                deviceRaw = (byte[])reader["DeviceRaw"];
                swRaw = (byte[])reader["SWRaw"];
            }
            else
            {
                throw new Exception("Smartwatch not found");
            }
            reader.Close();
        }
        
        Smartwatch dev = (Smartwatch)device;
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

        string updateSmartwatch = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceID = @Id and SWVersion = @SWVersion";
        using (var command = new SqlCommand(updateSmartwatch, conn, transaction))
        {
            command.Parameters.AddWithValue("@BatteryPercentage", dev.BatteryPercentage);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.Add("@SWVersion", SqlDbType.Timestamp).Value = swRaw;
            
            if (command.ExecuteNonQuery() == 0)
                throw new Exception("Smartwatch update failed");
        }

        return true;
    }
}