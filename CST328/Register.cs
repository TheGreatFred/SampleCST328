namespace Iot.Device.CST328
{
    internal enum Register
    {
        // Version Information Registers
        // The operating mode should be set to MODE_DEBUG_INFO to read these registers.
        // 2 byte register address, 4 bytes per record

        // BYTE 3: KEY_NUM
        // BYTE 2: TP_NRX
        // BYTE 1: NC
        // BYTE 0: TP_NTX
        CST328_INFO_1 = 0XD1F4,
        // BYTE 3 ~ BYTE 2: TP_RESY
        // BYTE 1 ~ BYTE 0: TP_RESX
        CST328_INFO_2 = 0XD1F8,
        // BYTE 3 ~ BYTE 2: 0XCACA (fixed value)
        // BYTE 1 ~ BYTE 0: BOOT_TIMER
        CST328_INFO_3 = 0XD1FC,
        // BYTE 3 ~ BYTE 2: IC_TYPE
        // BYTE 1 ~ BYTE 0: PROJECT_ID
        CST328_INFO_4 = 0XD204,
        // BYTE 3: FW_MAJOR
        // BYTE 2: FW_MINOR
        // BYTE 1 ~ BYTE 0: FW_BUILD
        CST328_INFO_5 = 0XD208,
        // BYTE 3:CHECKSNM_H
        // BYTE 2:CHECKSNM_H
        // BYTE 1:CHECKSNM_L
        // BYTE 0:CHECKSNM_L
        CST328_INFO_6 = 0XD20C,

        // Operating Mode Registers
        // 2 byte register address
        MODE_DEBUG_INFO = 0xD101,
        CHIP_SYSTEM_RESET = 0xD102,
        REDO_CALIBRATION = 0xD104,
        CHIP_DEEP_SLEEP = 0xD105,
        MODE_DEBUG_POINT = 0xD108,
        MODE_NORMAL = 0xD109,
        MODE_DEBUG_RAWDATA = 0xD10A,
        MODE_DEBUG_WRITE = 0xD10B,
        MODE_DEBUG_CALIBRATION = 0xD10C,
        MODE_DEBUG_DIFF = 0xD10D,
        MODE_FACTORY = 0xD119,

        // Touch information registers
        // 2 byte register address, 1 byte per record
        // BIT 7 ~ BIT 4: 1st finger ID
        // BIT 3 ~ BIT 0: 1st finger state: pressed (0x06) or lifted
        FINGER_1_ID = 0xD000,
        // BIT 7 ~ BIT 0: The X coordinate value of the 1st finger is eight high digits: X_Position>>4
        FINGER_1_XH8 = 0xD001,
        // BIT 7 ~ BIT 0: The Y coordinate value of the 1st finger is eight high digits: Y_Position>>4
        FINGER_1_YH8 = 0xD002,
        // BIT 7 ~ BIT 4: The X coordinate value of the 1st finger, X_Position & 0x0F
        // BIT 3 ~ BIT 0: The Y coordinate value of the 1st finger, Y_Position & 0x0F
        FINGER_1_XL4YL4 = 0xD003,
        // BIT 7 ~ BIT 0: 1st finger pressure value
        FINGER_1_Z = 0xD004,
        // BIT 7 ~ BIT 4: Report button flag (0x80)
        // BIT 3 ~ BIT 0: Report the number of fingers
        KEY_REPORT = 0xD005,
        // BIT 7 ~ BIT 0: Fixed 0xAB
        FIXED_0XAB = 0xD006,
        // BIT 7 ~ BIT 4: 2nd finger ID
        // BIT 3 ~ BIT 0: 2nd finger state: pressed (0x06) or lifted
        FINGER_2_ID = 0xD007,
        // BIT 7 ~ BIT 0: The X coordinate value of the 2nd finger is eight high digits: X_Position >> 4
        FINGER_2_XH8 = 0xD008,
        // BIT 7 ~ BIT 0: The Y coordinate value of the 2nd finger is eight high digits: Y_Position >> 4
        FINGER_2_YH8 = 0xD009,
        // BIT 7 ~ BIT 4: The X coordinate value of the 2nd finger, X_Position & 0x0F
        // BIT 3 ~ BIT 0: The Y coordinate value of the 2nd finger, Y_Position & 0x0F
        FINGER_2_XL4YL4 = 0xD00A,
        // BIT 7 ~ BIT 0: 2nd finger pressure value
        FINGER_2_Z = 0xD00B,
        // BIT 7 ~ BIT 4: 3rd finger ID
        // BIT 3 ~ BIT 0: 3rd finger state: pressed (0x06) or lifted
        FINGER_3_ID = 0xD00C,
        // BIT 7 ~ BIT 0: The X coordinate value of the 3rd finger is eight high digits: X_Position >> 4
        FINGER_3_XH8 = 0xD00D,
        // BIT 7 ~ BIT 0: The Y coordinate value of the 3rd finger is eight high digits: Y_Position >> 4
        FINGER_3_YH8 = 0xD00E,
        // BIT 7 ~ BIT 4: The X coordinate value of the 3rd finger, X_Position & 0x0F
        // BIT 3 ~ BIT 0: The Y coordinate value of the 3rd finger, Y_Position & 0x0F
        FINGER_3_XL4YL4 = 0xD00F,
        // BIT 7 ~ BIT 0: 3rd finger pressure value
        FINGER_3_Z = 0xD010,
        // BIT 7 ~ BIT 4: 4th finger ID
        // BIT 3 ~ BIT 0: 4th finger state: pressed (0x06) or lifted
        FINGER_4_ID = 0xD011,
        // BIT 7 ~ BIT 0: The X coordinate value of the 4th finger is eight high digits: X_Position >> 4
        FINGER_4_XH8 = 0xD012,
        // BIT 7 ~ BIT 0: The Y coordinate value of the 4th finger is eight high digits: Y_Position >> 4
        FINGER_4_YH8 = 0xD013,
        // BIT 7 ~ BIT 4: The X coordinate value of the 4th finger, X_Position & 0x0F
        // BIT 3 ~ BIT 0: The Y coordinate value of the 4th finger, Y_Position & 0x0F
        FINGER_4_XL4YL4 = 0xD014,
        // BIT 7 ~ BIT 0: 4th finger pressure value
        FINGER_4_Z = 0xD015,
        // BIT 7 ~ BIT 4: 5th finger ID
        // BIT 3 ~ BIT 0: 5th finger state: pressed (0x06) or lifted
        FINGER_5_ID = 0xD016,
        // BIT 7 ~ BIT 0: The X coordinate value of the 5th finger is eight high digits: X_Position >> 4
        FINGER_5_XH8 = 0xD017,
        // BIT 7 ~ BIT 0: The Y coordinate value of the 5th finger is eight high digits: Y_Position >> 4
        FINGER_5_YH8 = 0xD018,
        // BIT 7 ~ BIT 4: The X coordinate value of the 5th finger, X_Position & 0x0F
        // BIT 3 ~ BIT 0: The Y coordinate value of the 5th finger, Y_Position & 0x0F
        FINGER_5_XL4YL4 = 0xD019,
        // BIT 7 ~ BIT 0: 5th finger pressure value
        FINGER_5_Z = 0xD01A
    }
}
