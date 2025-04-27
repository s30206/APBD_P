using APBD_P.Database.Parsers;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database;

public class DeviceService : IDeviceService
{
    private readonly string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDB> GetAllDevices()
    {
        List<DeviceDB> devices = [];
        
        const string query = "SELECT * FROM Device";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var device = new DeviceDB
                        {
                            ID = reader.GetString(0),
                            Name = reader.GetString(1),
                            IsOn = reader.GetBoolean(2)
                        };

                        devices.Add(device);
                    }
                }
            }
            finally
            {
                connection.Close();
            }
         
            return devices;
        }
    }

    public Device? GetDeviceById(string id)
    {
        var devType = id.Split('-')[0] switch
        {
            "ED" => nameof(EmbeddedDevice),
            "P" => nameof(PersonalComputer),
            "SW" => nameof(Smartwatch),
            _ => null
        };
        
        if (devType == null) return null;

        var query = $"SELECT * FROM Device JOIN {devType} d on Device.ID = d.Device_ID WHERE Device.ID = @id";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (!reader.HasRows) return null;

                reader.Read();

                IDeviceParser? parser = devType switch
                {
                    nameof(EmbeddedDevice) => new EmbeddedDeviceParser(),
                    nameof(PersonalComputer) => new PersonalComputerParser(),
                    nameof(Smartwatch) => new SmartwatchParser(),
                    _ => null
                };
                
                return parser?.ParseDevice(reader);
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public bool AddDevice(Device device)
    {
        throw new NotImplementedException();
    }

    public Device? UpdateDevice(string id, Device device)
    {
        throw new NotImplementedException();
    }

    public bool DeleteDevice(string id)
    {
        var devType = id.Split('-')[0] switch
        {
            "ED" => nameof(EmbeddedDevice),
            "P" => nameof(PersonalComputer),
            "SW" => nameof(Smartwatch),
            _ => null
        };
        
        if (devType == null) return false;

        var queryDeriveDevice = $"DELETE {devType} WHERE Device_ID = @id";
        var queryDevice = "DELETE FROM Device WHERE ID = @id";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(queryDeriveDevice, connection);
            command.Parameters.AddWithValue("@id", id);

            try
            {
                var result = command.ExecuteNonQuery();

                if (result == 0) return false;
                
                command = new SqlCommand(queryDevice, connection);
                command.Parameters.AddWithValue("@id", id);
                
                return command.ExecuteNonQuery() > 0;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}