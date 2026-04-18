using IoT.Device.CST328;
using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Threading;

namespace Iot.Device.CST328
{
    /// <summary>
    /// 
    /// CST328 Touch Library for nanoFramework
    /// 
    /// Loosely based on existing nanoFramework touch devices source code.
    /// And publically available datasheets for the CST328
    /// Polling and interrupt events supported.
    /// 
    /// Tested on Waveshare CST328 devices.
    /// 
    /// </summary>
    public class CST328 : IDisposable
    {
        //Hardware set up
        private readonly I2cDevice _i2cDevice;
        private GpioController _gpioController;
        private GpioPin _resetPin;
        private GpioPin _interruptPin;
        //Command array reused often
        private readonly byte[] _command = new byte[2];
        //Cached data
        private readonly TouchPoint[] _touchPoints = new TouchPoint[5];
        private byte _touchCount;
        //Convenience map
        private readonly Register[] FingerLookup = new Register[5] { Register.FINGER_1_ID, Register.FINGER_2_ID, Register.FINGER_3_ID, Register.FINGER_4_ID, Register.FINGER_5_ID };

        //Interrupt event
        public delegate void TouchEventHandler(int touchCount);
        public event TouchEventHandler OnTouchEvent;

        public byte TouchCount { get => _touchCount; }

        public CST328(I2cDevice i2cDevice, int resetPin = -1, int interruptPin = -1)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentException(nameof(i2cDevice));
            _gpioController = new GpioController();
            if(resetPin != -1)
                _resetPin = _gpioController.OpenPin(resetPin, PinMode.Output);
            if(interruptPin != -1)
                _interruptPin = _gpioController.OpenPin(interruptPin, PinMode.InputPullUp);
        }

        public void Dispose()
        {
            DisableInterrupt();
            _resetPin?.Dispose();
            _resetPin = null!;
            _interruptPin?.Dispose();
            _interruptPin = null!;
            _gpioController?.Dispose();
            _gpioController = null!;
        }

        //Also see mode section for soft reset
        public void HardSystemReset()
        {
            if (_resetPin != null)
            {
                _resetPin.Write(PinValue.High);
                Thread.Sleep(10);
                _resetPin.Write(PinValue.Low);
                Thread.Sleep(10);
                _resetPin.Write(PinValue.High);
                Thread.Sleep(200); //As per manufacturer documentation
            }
        }

        //Hardware availability test
        public bool IsAvailable()
        {
            // A few attempts as per the manufacturer examples
            for (int i = 0; i < 3; i++)
            {
                ModeDebugInfo();
                var reg = ReadRegister(Register.CST328_INFO_3);
                var fw_vc = (UInt16)((reg >> 16) & 0xFFFF);
                if (fw_vc == 0xCACA) //Firmware verification code
                {
                    ModeNormal();
                    return true;
                }
                Thread.Sleep(10);
            }
            return false;
        }

        //Mode change functions
        public void ChipSystemReset() => WriteCommand(Register.CHIP_SYSTEM_RESET);
        public void ModeDebugInfo() => WriteCommand(Register.MODE_DEBUG_INFO);
        public void RedoCalibration() => WriteCommand(Register.REDO_CALIBRATION);
        public void ChipDeepSleep() => WriteCommand(Register.CHIP_DEEP_SLEEP);
        public void ModeDebugPoints() => WriteCommand(Register.MODE_DEBUG_POINT);
        public void ModeNormal() => WriteCommand(Register.MODE_NORMAL);
        public void ModeDebugRawdata() => WriteCommand(Register.MODE_DEBUG_RAWDATA);
        public void ModeDebugWrite() => WriteCommand(Register.MODE_DEBUG_WRITE);
        public void ModeDebugCalibration() => WriteCommand(Register.MODE_DEBUG_CALIBRATION);
        public void ModeDebugDiff() => WriteCommand(Register.MODE_DEBUG_DIFF);
        public void ModeFactory() => WriteCommand(Register.MODE_FACTORY);

        public void LoadTouchPoints()
        {
            //Read all touch data blocks
            byte[] touchData = ReadBlock(Register.FINGER_1_ID, 27);

            //Finger 1, co-ordinates are 12-bit integers (max 4096, plenty for any touch screen)
            _touchPoints[0].Pressed = (touchData[0] & 0x0F) == 6;
            _touchPoints[0].X = (UInt16)((touchData[1] << 4) | ((touchData[3] >> 4) & 0x0F));
            _touchPoints[0].Y = (UInt16)((touchData[2] << 4) | (touchData[3] & 0x0F));
            _touchPoints[0].Pressure = touchData[4];

            _touchCount = touchData[5]; //cache touch count

            int dataIndex = 7; //Skip past finger 1 block, touch count byte and fixed 0xAB byte
            for (int i = 1; i < 5; i++)
            {
                _touchPoints[i].Pressed = (touchData[dataIndex] & 0x0F) == 6;
                _touchPoints[i].X = (UInt16)((touchData[dataIndex + 1] << 4) | ((touchData[dataIndex + 3] >> 4) & 0x0F));
                _touchPoints[i].Y = (UInt16)((touchData[dataIndex + 2] << 4) | (touchData[dataIndex + 3] & 0x0F));
                _touchPoints[i].Pressure = touchData[dataIndex + 4];
                dataIndex += 5; // Jump to next touch point in block
            }
        }

        //Return a cached touch point
        public TouchPoint CachedTouchPoint(int fingerId) => _touchPoints[fingerId - 1];

        //Read, cache and return a touch point
        public TouchPoint ReadTouchPoint(int fingerId)
        {
            byte[] touchData = ReadBlock(FingerLookup[fingerId - 1], 5);
            var touchpoint = new TouchPoint()
            {
                Pressed = (touchData[0] & 0x0F) == 6,
                X = (UInt16)((touchData[1] << 4) | ((touchData[3] >> 4) & 0x0F)),
                Y = (UInt16)((touchData[2] << 4) | (touchData[3] & 0x0F)),
                Pressure = touchData[4]
            };
            _touchPoints[fingerId - 1] = touchpoint;
            return touchpoint;
        }

        //Read, cache and return touch count
        public byte ReadTouchCount()
        {
            _touchCount = ReadByte(Register.KEY_REPORT);
            return _touchCount;
        }

        //Interrupts (must have pin defined)
        public void EnableInterrupt()
        {
            if (_interruptPin == null)
                throw new InvalidOperationException("no interrupt pin defined");
            _interruptPin.ValueChanged += HardwareTouchEvent;
        }

        public void DisableInterrupt() => _interruptPin.ValueChanged -= HardwareTouchEvent;

        private void HardwareTouchEvent(object sender, PinValueChangedEventArgs e)
        {
            if (e.ChangeType == PinEventTypes.Falling)
            {
                //Return touch count
                //Could alter this to deliver a TouchPoint object
                OnTouchEvent?.Invoke(ReadTouchCount());
            }
        }

        public VersionInfo GetVersionInfo()
        {
            var vi = new VersionInfo();
            ModeDebugInfo();

            //Read all the registers as one block
            var block = ReadBlock(Register.CST328_INFO_1, 28);
            vi.NumTx = block[0];
            vi.NumRx = block[2];
            vi.KeyNum = block[3];
            vi.ResX = BitConverter.ToUInt16(block, 4);
            vi.ResY = BitConverter.ToUInt16(block, 6);
            //Skip verification and unused registers
            vi.IcType = BitConverter.ToUInt16(block, 16);
            vi.ProjectId = BitConverter.ToUInt16(block, 18);
            vi.FwBuild = BitConverter.ToUInt16(block, 20);
            vi.FwMinor = block[22];
            vi.FwMajor = block[23];
            vi.ChecksumL = BitConverter.ToUInt16(block, 24);
            vi.ChecksumH = BitConverter.ToUInt16(block, 26);

            ModeNormal();
            return vi;
        }

        //Convenience functions
        private void WriteCommand(Register command)
        {
            CommandToByteArray(command);
            I2cTransferResult result = _i2cDevice.Write(_command);
            if(result.Status != I2cTransferStatus.FullTransfer || result.BytesTransferred != _command.Length)
                throw new Exception("I2C transfer failure sending command");
        }
        private byte ReadByte(Register command) => ReadBlock(command, 1)[0];
        private UInt32 ReadRegister(Register command) => BitConverter.ToUInt32(ReadBlock(command, 4), 0);
        private byte[] ReadBlock(Register command, int length)
        {
            CommandToByteArray(command);
            byte[] block = new byte[length];
            I2cTransferResult result = _i2cDevice.WriteRead(_command, block);
            if (result.Status != I2cTransferStatus.FullTransfer || result.BytesTransferred != (_command.Length + block.Length))
                throw new Exception("I2C transfer failure reading block");
            return block;
        }
        private void CommandToByteArray(Register command)
        {
            UInt16 command16 = (UInt16)command;
            _command[0] = (byte)(command16 >> 8);
            _command[1] = (byte)(command16 & 0xFF);
        }
    }
}
