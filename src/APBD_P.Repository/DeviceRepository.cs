using APBD_P.Database.Parsers;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Repository;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
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

    public Device? GetDeviceById(string id, string devType)
    {
        var query = $"SELECT * FROM Device JOIN {devType} d on Device.ID = d.DeviceID WHERE Device.ID = @id";

        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                
                SqlDataReader reader = command.ExecuteReader();
            
                if (!reader.HasRows) return null;

                reader.Read();
                
                IDeviceParser? parser = devType switch
                {
                    nameof(Embedded) => new EmbeddedParser(),
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
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                IDeviceParser? parser = device.GetType().Name switch
                {
                    nameof(Embedded) => new EmbeddedParser(),
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
        string query = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE ID = @ID";

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var command = new SqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);

                if (command.ExecuteNonQuery() == 0) throw new Exception("Failed to update device");

                IDeviceParser? parser = device.GetType().Name switch
                {
                    nameof(Embedded) => new EmbeddedParser(),
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

    public bool DeleteDevice(string id, string devType)
    {
        var queryDeriveDevice = $"DELETE {devType} WHERE DeviceID = @id";
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