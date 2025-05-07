using APBD_P.Database.Parsers;
using APBD_P.Source.Interfaces;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Source.DeviceService;

public class DeviceService : IDeviceService
{
    private readonly string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<DeviceDB> GetAllDevices()
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
        if (device == null) return false;
        
        const string query = "INSERT INTO Device (ID, Name, IsOn) VALUES (@ID, @Name, @IsOn)";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand(query, connection, transaction);

                string? shortName = device.GetType().Name switch
                {
                    nameof(EmbeddedDevice) => "ED",
                    nameof(PersonalComputer) => "P",
                    nameof(Smartwatch) => "SW",
                    _ => null
                };

                command.Parameters.AddWithValue("@ID", $"{shortName}-{device.Id}");
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsOn", device.IsOn);

                if (command.ExecuteNonQuery() == 0) throw new Exception("Failed to add device");

                IDeviceParser? parser = device.GetType().Name switch
                {
                    nameof(EmbeddedDevice) => new EmbeddedDeviceParser(),
                    nameof(PersonalComputer) => new PersonalComputerParser(),
                    nameof(Smartwatch) => new SmartwatchParser(),
                    _ => null
                };

                if (parser == null || !parser.InsertDevice(device, connection, transaction))
                    throw new Exception("Failed to add device");
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public bool UpdateDevice(string id, Device device)
    {
        string query = "UPDATE Device SET Name = @Name, IsOn = @IsOn WHERE ID = @ID";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                SqlCommand command = new SqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsOn", device.IsOn);

                if (command.ExecuteNonQuery() == 0) throw new Exception("Failed to update device");

                IDeviceParser? parser = device.GetType().Name switch
                {
                    nameof(EmbeddedDevice) => new EmbeddedDeviceParser(),
                    nameof(PersonalComputer) => new PersonalComputerParser(),
                    nameof(Smartwatch) => new SmartwatchParser(),
                    _ => null
                };

                if (parser == null || !parser.UpdateDevice(id, device, connection, transaction))
                    throw new Exception("Failed to update device");
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
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
            using var transaction = connection.BeginTransaction();

            try
            {
                var command = new SqlCommand(queryDeriveDevice, connection, transaction);
                command.Parameters.AddWithValue("@id", id);

                if (command.ExecuteNonQuery() == 0) 
                    throw new Exception("Failed to delete device");

                command = new SqlCommand(queryDevice, connection, transaction);
                command.Parameters.AddWithValue("@id", id);

                if (command.ExecuteNonQuery() == 0) 
                    throw new Exception("Failed to delete device");
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}