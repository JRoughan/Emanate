using System;

namespace Emanate.Core.Output.DelcomVdi
{
    [Name("Delcom VDI")]
    public class Device : IDisposable
    {
        private readonly DelcomHid delcom = new DelcomHid();
        private DelcomHid.HidTxPacketStruct txCmd;

        public string DeviceName { get; private set; }
        public string DeviceStatus { get; private set; }

        public void Open()
        {
            // Current TID and SID are not supported

            if (delcom.Open() == 0)
            {
                UInt32 serialNumber, version, date, month, year;
                delcom.GetDeviceInfo(out serialNumber, out version, out date, out month, out year);
                year += 2000;
                DeviceName = "DeviceName: " + delcom.GetDeviceName();
                DeviceStatus = string.Format("Device Status: Found. SerialNumber={0} Version={1} {2}/{3}/{4}", serialNumber, version, month, date, year);

                // Optionally -Enable event counter use that auto switch feature work
                txCmd.MajorCmd = 101;
                txCmd.MinorCmd = 38;
                txCmd.LSBData = 1;
                txCmd.MSBData = 0;
                delcom.SendCommand(txCmd);
            }
            else
            {
                DeviceName = "DeviceName: offine";
                DeviceStatus = "Error: Unable to open device.";
            }
        }

        public void Close()
        {
            delcom.Close();
            DeviceName = "DeviceName: offine";
            DeviceStatus = "Device Closed.";
        }

        private Boolean IsPresent()
        {
            return delcom.IsOpen();
        }

        public void TurnOff(Color color)
        {
            if (!IsPresent())
                return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = color.SetId;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);  // always disable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = 0;
            txCmd.MSBData = color.SetId;
            delcom.SendCommand(txCmd);
        }

        private void TurnOff()
        {
            TurnOff(Color.Green);
            TurnOff(Color.Yellow);
            TurnOff(Color.Red);
        }

        public void TurnOn(Color color)
        {
            if (!IsPresent())
                return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = color.SetId;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);  // always disable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = color.SetId;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);
        }

        public void Flash(Color color)
        {
            if (!IsPresent())
                return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = 0;
            txCmd.MSBData = color.SetId;
            delcom.SendCommand(txCmd);  // enable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = color.SetId;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd); // and turn it on
        }

        public void Apply(Color color, Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent())
                return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = color.DutyId;
            txCmd.LSBData = offDuty;
            txCmd.MSBData = onDuty;
            delcom.SendCommand(txCmd); // Set the duty cycle

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = color.OffsetId;
            txCmd.LSBData = offset;
            delcom.SendCommand(txCmd); // Set the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 34;
            txCmd.LSBData = color.PowerId;
            txCmd.MSBData = power;
            delcom.SendCommand(txCmd); // Set the power
        }

        public void Sync(Byte greenOffset, Byte redOffset, Byte blueOffset)
        {
            if (!IsPresent())
                return;

            // Alwasy reload the offset, as it is cleared each time
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = Color.Green.OffsetId;
            txCmd.LSBData = greenOffset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = Color.Red.OffsetId;
            txCmd.LSBData = redOffset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = Color.Yellow.OffsetId;
            txCmd.LSBData = blueOffset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 25;
            txCmd.LSBData = 7;      // Sync Green, red & blue
            txCmd.MSBData = 0;      // 0=off. 1=on - all on
            delcom.SendCommand(txCmd);  // always disable the flash mode 
        }

        public void Prescaler(Byte prescaler)
        {
            if (!IsPresent())
                return;
            //MessageBox.Show("Prescaler out of range!\r\nPrescaler range 0-255\r\nUnits are in ms.\r\nDefault is 10ms", "Warning - Range Error!");

            // Always reload the offset, as it is cleared each time
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 19;
            txCmd.LSBData = prescaler;
            delcom.SendCommand(txCmd); // Load the offset
        }

        public void StartBuzzer(Byte freq, Byte repeat, Byte onTime, Byte offTime)
        {
            if (!IsPresent())
                return;
            //MessageBox.Show("Buzzer Paramters out of range!\r\nFreq 1-255 Value=1/(FreqHz*256E-6)\r\nRepeat 0-255 0=Continous, 255= Repeat forever.\r\nOnTime 0-100 Units ms.\r\nOffTime 0-100 Units ms.", "Warning - Range Error!");

            txCmd.MajorCmd = 102;
            txCmd.MinorCmd = 70;
            txCmd.LSBData = 1;          // Enable buzzer
            txCmd.MSBData = freq;       // =1/(freqHz*256us)
            txCmd.ExtData0 = repeat;    // repeat value
            txCmd.ExtData1 = onTime;    // On time
            txCmd.ExtData2 = offTime;   // Off time

            delcom.SendCommand(txCmd);  // always disable the flash mode 
        }

        public void StopBuzzer()
        {
            if (!IsPresent())
                return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 70;
            txCmd.LSBData = 0;      // 0=off. 1=on
            delcom.SendCommand(txCmd);  // always disable the flash mode 
        }

        protected void Dispose(bool disposing)
        {
            StopBuzzer();
            TurnOff();
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Device()
        {
            Dispose(false);
        }
    }

    public class NameAttribute : Attribute
    {
        public NameAttribute(string delcomVdi)
        {
            DelcomVdi = delcomVdi;
        }

        public string DelcomVdi { get; private set; }
    }
}