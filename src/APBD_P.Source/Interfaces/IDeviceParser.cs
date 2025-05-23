﻿using System.Text.Json.Nodes;
using APBD_P1;
using Microsoft.Data.SqlClient;

namespace APBD_P.Source.Parsers;

public interface IDeviceParser
{
    Device ParseDevice(SqlDataReader reader);
    Device? ParseJsonDevice(JsonNode json);
    Device? ParseTextDevice(string[] parts);
    bool InsertDevice(Device device, SqlConnection conn, SqlTransaction transaction);
    bool UpdateDevice(string id, Device device, SqlConnection conn, SqlTransaction transaction);
}