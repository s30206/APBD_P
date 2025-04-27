using APBD_P.Source.Parsers;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public class PersonalComputerParser : IDeviceParser
{
    public Device ParseDevice(SqlDataReader reader)
    {
        return new PersonalComputer
        {
            Id = int.Parse(reader.GetString(0).Split("-")[1]),
            Name = reader.GetString(1),
            IsOn = reader.GetBoolean(2),
            OperatingSystem = reader.GetString(5)
        };
    }
}