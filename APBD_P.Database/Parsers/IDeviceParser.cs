using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Database.Parsers;

public interface IDeviceParser
{
    Device ParseDevice(SqlDataReader reader);
}