using System;

namespace IoT.Device.CST328
{
    public struct VersionInfo
    {
        //CST328_INFO_1
        public byte KeyNum;
        public byte NumTx;
        public byte NumRx;
        //CST328_INFO_2
        public UInt16 ResX;
        public UInt16 ResY;
        //CST328_INFO_4
        public UInt16 IcType;
        public UInt16 ProjectId;
        //CST328_INFO_5
        public byte FwMajor;
        public byte FwMinor;
        public UInt16 FwBuild;
        //CST328_INFO_6
        public UInt16 ChecksumH;
        public UInt16 ChecksumL;
    }
}
