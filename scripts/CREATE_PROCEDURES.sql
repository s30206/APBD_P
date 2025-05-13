CREATE PROCEDURE AddEmbedded
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @IpAddress VARCHAR(50),
    @NetworkName VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert into Device table
        INSERT INTO Device (Id, Name, IsEnabled)
        VALUES (@DeviceId, @Name, @IsEnabled);

        -- Insert into Embedded table
        INSERT INTO Embedded (IpAddress, NetworkName, DeviceId)
        VALUES (@IpAddress, @NetworkName, @DeviceId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

    

CREATE PROCEDURE AddSmartwatch
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @BatteryPercentage INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert into Device table
        INSERT INTO Device (Id, Name, IsEnabled)
        VALUES (@DeviceId, @Name, @IsEnabled);

        -- Insert into Smartwatch table
        INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
        VALUES (@BatteryPercentage, @DeviceId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;



CREATE PROCEDURE AddPersonalComputer
    @DeviceId VARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @OperationSystem VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insert into Device table
        INSERT INTO Device (Id, Name, IsEnabled)
        VALUES (@DeviceId, @Name, @IsEnabled);

        -- Insert into PersonalComputer table
        INSERT INTO PersonalComputer (OperationSystem, DeviceId)
        VALUES (@OperationSystem, @DeviceId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
