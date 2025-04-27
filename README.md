# Overview

This project contains an API that connects to a Microsoft SQL Server database. The database connection is configured via a connection string stored in the `appsettings.json` file.

# Setting up appsettings.json

Create a file named `appsettings.json` inside the `src/APBD_P.API` folder with the following content:

<pre>
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Database": ""
  }
}
</pre>

In the `Database` field, provide your connection string to the Microsoft SQL Server instance.
