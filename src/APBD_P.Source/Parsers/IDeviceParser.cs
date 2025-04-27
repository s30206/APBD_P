using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Source.Parsers;

public interface IDeviceParser
{
    Device ParseDevice(SqlDataReader reader);
}