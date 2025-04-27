using APBD_P.Database.Parsers;
using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database;

public class DeviceService : IDeviceService
{
    private string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<DeviceDB> GetAllDevices()
    {
        List<DeviceDB> devices = [];
        
        const string query = "SELECT * FROM Device";

        using (SqlConnection connection = new SqlConnection(_connectionString))
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
            "ED" => typeof(EmbeddedDevice),
            "P" => typeof(PersonalComputer),
            "SW" => typeof(Smartwatch),
            _ => null
        };
        
        if (devType == null) return null;

        string query = $"SELECT * FROM Device JOIN {devType.Name} dev on Device.ID = dev.Device_ID WHERE Device.ID = @id";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (!reader.HasRows) return null;

                reader.Read();

                IDeviceParser? parser = null;
                Device? device = null;

                if (devType == typeof(EmbeddedDevice))
                {
                    parser = new EmbeddedDeviceParser();
                }
                else if (devType == typeof(PersonalComputer))
                {
                    parser = new PersonalComputerParser();
                }
                else if (devType == typeof(Smartwatch))
                {
                    parser = new SmartwatchParser();
                }
                
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
        int countRows = -1;

        /*switch (device.GetType().ToString())
        {
            case "EmbeddedDevice":
            {
                const string insertString = "insert into device (Id, Name, IsOn, IpAddress, NetworkName) values (@Id, @Name, @IsOn, @IpAddress, @NetworkName)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    EmbeddedDevice dev = (EmbeddedDevice)device;

                    SqlCommand command = new SqlCommand(insertString, connection);
                    command.Parameters.AddWithValue("@Id", device.Id);
                    command.Parameters.AddWithValue("@Name", device.Name);
                    command.Parameters.AddWithValue("@IsOn", device.IsOn);
                    command.Parameters.AddWithValue("@IpAddress", dev.IpAddress);
                    command.Parameters.AddWithValue("@NetworkName", dev.NetworkName);

                    connection.Open();
                    
                    countRows = command.ExecuteNonQuery();
                }
                break;
            }
            case "Smartwatch":
            {
                const string insertString = "insert into devices (Id, Name, IsOn, BatteryPercentage) values (@Id, @Name, @IsOn, @BatteryPercentage)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    Smartwatch dev = (Smartwatch)device;
                    
                    SqlCommand command = new SqlCommand(insertString, connection);
                    command.Parameters.AddWithValue("@Id", dev.Id);
                    command.Parameters.AddWithValue("@Name", dev.Name);
                    command.Parameters.AddWithValue("@IsOn", dev.IsOn);
                    command.Parameters.AddWithValue("@BatteryPercentage", dev.BatteryPercentage);
                    
                    connection.Open();
                    
                    countRows = command.ExecuteNonQuery();
                }
                break;
            }
            case "PersonalComputer":
            {
                const string insertString = "insert into devices (Id, Name, IsOn, OperatingSystem) values (@Id, @Name, @IsOn, @OperatingSystem)";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    PersonalComputer dev = (PersonalComputer)device;
                    
                    SqlCommand command = new SqlCommand(insertString, connection);
                    command.Parameters.AddWithValue("@Id", dev.Id);
                    command.Parameters.AddWithValue("@Name", dev.Name);
                    command.Parameters.AddWithValue("@IsOn", dev.IsOn);
                    command.Parameters.AddWithValue("@OperatingSystem", dev.OperatingSystem);
                    
                    connection.Open();
                    
                    countRows = command.ExecuteNonQuery();
                    break;
                }
            }
        }*/

        return countRows != -1;
    }
}