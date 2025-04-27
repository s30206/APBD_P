-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2025-04-25 09:04:21.695

-- tables
-- Table: Device
CREATE TABLE Device (
                        ID varchar(250)  NOT NULL,
                        Name varchar(250)  NOT NULL,
                        IsOn bit  NOT NULL,
                        CONSTRAINT Device_pk PRIMARY KEY  (ID)
);

-- Table: EmbeddedDevice
CREATE TABLE EmbeddedDevice (
                                ID int  NOT NULL,
                                Device_ID varchar(250)  NOT NULL,
                                IpAddress varchar(250)  NOT NULL,
                                NetworkName varchar(250)  NOT NULL,
                                CONSTRAINT EmbeddedDevice_pk PRIMARY KEY  (ID,Device_ID)
);

-- Table: PersonalComputer
CREATE TABLE PersonalComputer (
                                  ID int  NOT NULL,
                                  Device_ID varchar(250)  NOT NULL,
                                  OperatingSystem varchar(250)  NOT NULL,
                                  CONSTRAINT PersonalComputer_pk PRIMARY KEY  (ID,Device_ID)
);

-- Table: Smartwatch
CREATE TABLE Smartwatch (
                            ID int  NOT NULL,
                            Device_ID varchar(250)  NOT NULL,
                            BatteryPercentage int  NOT NULL,
                            CONSTRAINT Smartwatch_pk PRIMARY KEY  (ID,Device_ID)
);

-- foreign keys
-- Reference: Table_2_Device (table: EmbeddedDevice)
ALTER TABLE EmbeddedDevice ADD CONSTRAINT Table_2_Device
    FOREIGN KEY (Device_ID)
    REFERENCES Device (ID);

-- Reference: Table_3_Device (table: PersonalComputer)
ALTER TABLE PersonalComputer ADD CONSTRAINT Table_3_Device
    FOREIGN KEY (Device_ID)
    REFERENCES Device (ID);

-- Reference: Table_4_Device (table: Smartwatch)
ALTER TABLE Smartwatch ADD CONSTRAINT Table_4_Device
    FOREIGN KEY (Device_ID)
    REFERENCES Device (ID);

-- End of file.

INSERT INTO Device (ID, Name, IsOn) VALUES ('ED-1', 'Raspberry Pi 4', 1);
INSERT INTO Device (ID, Name, IsOn) VALUES ('P-1', 'Dell Laptop', 0);
INSERT INTO Device (ID, Name, IsOn) VALUES ('SW-1', 'Apple Watch', 1);

INSERT INTO EmbeddedDevice (ID, Device_ID, IpAddress, NetworkName) VALUES
    (1, 'ED-1', '192.168.1.10', 'HomeNetwork');

INSERT INTO PersonalComputer (ID, Device_ID, OperatingSystem) VALUES
    (1, 'P-1', 'Windows 11');

INSERT INTO Smartwatch (ID, Device_ID, BatteryPercentage) VALUES
    (1, 'SW-1', 87);
