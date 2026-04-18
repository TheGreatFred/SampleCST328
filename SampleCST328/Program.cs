using Iot.Device.CST328;
using nanoFramework.Hardware.Esp32;
using System.Device.I2c;
using System.Diagnostics;
using System.Threading;

namespace SampleCST328
{
    public class Program
    {
        const int TP_SDA = 1;            // Touch I2C Data
        const int TP_RST = 2;            // Touch Reset
        const int TP_SCL = 3;            // Touch I2C Clock
        const int TP_INT = 4;            // Touch Interrupt
        const int TP_ADDRESS = 0x1A;     // Touch I2C Address

        public static void Main()
        {
            Debug.WriteLine("Starting up CST328...");
            Configuration.SetPinFunction(TP_SDA, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(TP_SCL, DeviceFunction.I2C1_CLOCK);
            I2cDevice i2c = new(new I2cConnectionSettings(1, TP_ADDRESS, I2cBusSpeed.FastMode));

            var cst328 = new CST328(i2c, TP_RST, TP_INT);
            cst328.HardSystemReset();
            if (cst328.IsAvailable())
            {
                var versionInfo = cst328.GetVersionInfo();
                Debug.WriteLine($"CST328 panel found, X = {versionInfo.ResX} Y = {versionInfo.ResY}");
                cst328.OnTouchEvent += (count) =>
                {
                    if (count > 0)
                    {
                        Debug.WriteLine($"Touch event, {count} points of contact");
                        for (int i = 1; i <= count; i++)
                        {
                            var tp = cst328.ReadTouchPoint(i);
                            Debug.WriteLine($"Point {i}: X = {tp.X}, Y = {tp.Y}, Pressure = {tp.Pressure}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Touch event, zero points of contact");
                    }
                };
                cst328.EnableInterrupt();
            }


            Thread.Sleep(Timeout.Infinite);
        }
    }
}
