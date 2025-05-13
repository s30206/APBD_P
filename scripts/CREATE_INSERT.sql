-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-05-13 12:57:05.546

-- tables
-- Table: Device
CREATE TABLE Device (
                        ID varchar(250)  NOT NULL,
                        Name varchar(250)  NOT NULL,
                        IsEnabled bit  NOT NULL,
                        DeviceVersion rowversion  NOT NULL,
                        CONSTRAINT Device_pk PRIMARY KEY  (ID)
);

-- Table: Embedded
CREATE TABLE Embedded (
                          ID int  NOT NULL IDENTITY(1, 1),
                          IpAddress varchar(250)  NOT NULL,
                          NetworkName varchar(250)  NOT NULL,
                          EmbeddedVersion rowversion  NOT NULL,
                          DeviceID varchar(250)  NOT NULL,
                          CONSTRAINT Embedded_pk PRIMARY KEY  (ID,DeviceID)
);

-- Table: PersonalComputer
CREATE TABLE PersonalComputer (
                                  ID int  NOT NULL IDENTITY(1, 1),
                                  OperationSystem varchar(250)  NOT NULL,
                                  PCVersion rowversion  NOT NULL,
                                  DeviceID varchar(250)  NOT NULL,
                                  CONSTRAINT PersonalComputer_pk PRIMARY KEY  (DeviceID,ID)
);

-- Table: Smartwatch
CREATE TABLE Smartwatch (
                            ID int  NOT NULL IDENTITY(1, 1),
                            BatteryPercentage int  NOT NULL,
                            SWVersion rowversion  NOT NULL,
                            DeviceID varchar(250)  NOT NULL,
                            CONSTRAINT Smartwatch_pk PRIMARY KEY  (DeviceID,ID)
);

-- foreign keys
-- Reference: Table_2_Device (table: Embedded)
ALTER TABLE Embedded ADD CONSTRAINT Table_2_Device
    FOREIGN KEY (DeviceID)
        REFERENCES Device (ID);

-- Reference: Table_3_Device (table: PersonalComputer)
ALTER TABLE PersonalComputer ADD CONSTRAINT Table_3_Device
    FOREIGN KEY (DeviceID)
        REFERENCES Device (ID);

-- Reference: Table_4_Device (table: Smartwatch)
ALTER TABLE Smartwatch ADD CONSTRAINT Table_4_Device
    FOREIGN KEY (DeviceID)
        REFERENCES Device (ID);

-- End of file.



exec AddEmbedded @DeviceId = 'ED-1', @Name = 'Raspberry Pi 4', @IsEnabled = 1, @IpAddress = '192.168.1.10', @NetworkName = 'MD Ltd.HomeNetwork';
exec AddPersonalComputer @DeviceId = 'P-1', @Name = 'Dell Laptop', @IsEnabled = 0, @OperationSystem = 'Windows 11';
exec AddSmartwatch @DeviceId = 'SW-1', @Name = 'Apple Watch', @IsEnabled = 1, @BatteryPercentage = 87;