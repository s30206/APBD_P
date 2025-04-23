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

    public IEnumerable<Device> GetAllDevices()
    {
        List<Device> devices = [];
        
        const string query = "SELECT * FROM Devices";

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
                        Device? device = null;
                        switch (reader.FieldCount)
                        {
                            case 5:
                            {
                                device = new EmbeddedDevice()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    IsOn = reader.GetBoolean(2),
                                    IpAddress = reader.GetString(3),
                                    NetworkName = reader.GetString(4)
                                };
                                break;
                            }
                            case 4:
                            {
                                try
                                {
                                    device = new Smartwatch()
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1),
                                        IsOn = reader.GetBoolean(2),
                                        BatteryPercentage = reader.GetInt32(3)
                                    };
                                }
                                catch (Exception e)
                                {
                                    device = new PersonalComputer()
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1),
                                        IsOn = reader.GetBoolean(2),
                                        OperatingSystem = reader.GetString(3)
                                    };
                                }

                                break;
                            }
                            default:
                            {
                                throw new Exception("There is no such Device");
                            }
                        }

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

    public bool AddDevice(Device device)
    {
        int countRows = -1;

        switch (device.GetType().ToString())
        {
            case "EmbeddedDevice":
            {
                const string insertString = "insert into devices (Id, Name, IsOn, IpAddress, NetworkName) values (@Id, @Name, @IsOn, @IpAddress, @NetworkName)";

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
        }

        return countRows != -1;
    }
}