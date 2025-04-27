using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class SmartwatchParser : IDeviceParser
{
    public Device ParseDevice(SqlDataReader reader)
    {
        return new Smartwatch
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsOn = reader.GetBoolean(2),
            BatteryPercentage = reader.GetInt32(5)
        };
    }
}