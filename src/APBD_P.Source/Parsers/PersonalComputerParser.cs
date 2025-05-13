using System.Data;
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
            IsEnabled = reader.GetBoolean(2),
            OperationSystem = reader.GetString(5)
        };
    }

    public Device? ParseJsonDevice(JsonNode json)
    {        
        json["id"] = Int32.Parse(json["id"]?.ToString().Split("-")[1]);
        return JsonSerializer.Deserialize<PersonalComputer>(json.ToString(), _options);
    }

    public Device? ParseTextDevice(string[] parts)
    {
        return new PersonalComputer()
        {
            Id = Int32.Parse(parts[0].Trim().Split("-")[1]),
            Name = parts[1].Trim(),
            IsEnabled = bool.Parse(parts[2].Trim()),
            OperationSystem = parts[3].Trim()
        };
    }

    public bool InsertDevice(Device device, SqlConnection conn, SqlTransaction transaction)
    {
        PersonalComputer dev = (PersonalComputer)device;

        string shortName = "P";
        
        var command = new SqlCommand("AddPersonalComputer", conn, transaction);
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@DeviceID", $"{shortName}-{dev.Id}");
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", dev.IsEnabled);
        command.Parameters.AddWithValue("@OperationSystem", dev.OperationSystem);
        
        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected != 0;
    }

    public bool UpdateDevice(string id, Device device, SqlConnection conn, SqlTransaction transaction)
    {
        var rowVersionQuery =
            "select device.DeviceVersion as DeviceRaw, s.PCVersion as PCRaw from device join PersonalComputer s on device.ID = s.DeviceID where s.DeviceID = @id";

        byte[] deviceRaw = null;
        byte[] pcRaw = null;
        
        using (var command = new SqlCommand(rowVersionQuery, conn, transaction))
        {
            command.Parameters.AddWithValue("@id", id);
            
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                deviceRaw = (byte[])reader["DeviceRaw"];
                pcRaw = (byte[])reader["PCRaw"];
            }
            else
            {
                throw new Exception("PC not found");
            }
            reader.Close();
        }
        
        PersonalComputer dev = (PersonalComputer)device;
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
        
        string query = "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceID = @Id and PCVersion = @PCVersion";
        using (var command = new SqlCommand(query, conn, transaction))
        {
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@OperationSystem", dev.OperationSystem);
            command.Parameters.Add("@PCVersion", SqlDbType.Timestamp).Value = pcRaw;

            if (command.ExecuteNonQuery() == 0)
                throw new Exception("PC update failed");
        }

        return true;
    }
}