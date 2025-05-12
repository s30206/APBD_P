CREATE PROCEDURE AddEmbedded
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsOn BIT,
    @IpAddress VARCHAR(50),
    @NetworkName VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert into Device table
        INSERT INTO Device (Id, Name, IsOn)
        VALUES (@DeviceId, @Name, @IsOn);

        -- Insert into Embedded table
        INSERT INTO EmbeddedDevice (IpAddress, NetworkName, Device_Id)
        VALUES (@IpAddress, @NetworkName, @DeviceId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

CREATE PROCEDURE AddSmartwatch
    @ID INT,
    @Device_ID NVARCHAR(50),
    @BatteryPercentage INT
AS
BEGIN
    INSERT INTO Smartwatch (ID, Device_ID, BatteryPercentage)
    VALUES (@ID, @Device_ID, @BatteryPercentage);
END;

CREATE PROCEDURE AddPersonalComputer
    @ID INT,
    @Device_ID NVARCHAR(50),
    @OperatingSystem NVARCHAR(100)
AS
BEGIN
    INSERT INTO PersonalComputer (ID, Device_ID, OperatingSystem)
    VALUES (@ID, @Device_ID, @OperatingSystem);
END;