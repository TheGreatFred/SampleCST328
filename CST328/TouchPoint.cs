using System;

namespace IoT.Device.CST328
{
    public struct TouchPoint
    {
        public bool Pressed;
        public UInt16 X;
        public UInt16 Y;
        public byte Pressure;
    }
}
