using System;

namespace Emanate.Core.Output.DelcomVdi
{
    public class MainForm
    {
        private readonly DelcomHid delcom = new DelcomHid();
        private DelcomHid.HidTxPacketStruct txCmd;
        private string deviceName;
        private string deviceStatus;
        private bool @switch;


        public void Open()
        {
            // Current TID and SID are not supported

            if (delcom.Open() == 0)
            {
                UInt32 serialNumber, version, date, month, year;
                serialNumber = version = date = month = year = 0;
                delcom.GetDeviceInfo(ref serialNumber, ref version, ref date, ref month, ref year);
                year += 2000;
                deviceName = "DeviceName: "+delcom.GetDeviceName();
                deviceStatus = "Device Status: Found. SerialNumber=" + serialNumber.ToString() + " Version=" + version.ToString() + " " + month.ToString() + "/" + date.ToString() + "/" + year.ToString();


                // Optionally -Enable event counter use that auto switch feature work
                txCmd.MajorCmd = 101;
                txCmd.MinorCmd = 38;
                txCmd.LSBData = 1;
                txCmd.MSBData = 0;
                delcom.SendCommand(txCmd); 

            }
            else
            {
                deviceName = "DeviceName: offine";
                deviceStatus = "Error: Unable to open device.";
            }
        }

        public void Close()
        {
            delcom.Close();
            deviceName = "DeviceName: offine";
            deviceStatus = "Device Closed.";
        }

        private Boolean IsPresent()
        {
            return delcom.IsOpen();
        }


        public void GreenOff(Color color)
        {
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = (byte)color;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);  // always disable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = 0;
            txCmd.MSBData = (byte)color;
            delcom.SendCommand(txCmd);
        }     

        public void GreenOn(Color color)
        {
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = (byte)color;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);  // always disable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = (byte)color;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd);
        }

        public void GreenFlash(Color color)
        {
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 20;
            txCmd.LSBData = 0;
            txCmd.MSBData = (byte)color;
            delcom.SendCommand(txCmd);  // enable the flash mode 

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 12;
            txCmd.LSBData = (byte)color;
            txCmd.MSBData = 0;
            delcom.SendCommand(txCmd); // and turn it on
        }

        public void GreenApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
            
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 21;
            txCmd.LSBData = offDuty;
            txCmd.MSBData = onDuty;
            delcom.SendCommand(txCmd); // Load the duty cycle

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 26;
            txCmd.LSBData = offset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 34;
            txCmd.LSBData = 0;
            txCmd.MSBData = power;
            delcom.SendCommand(txCmd); // Load the offset
        }

        public void RedApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 22;
            txCmd.LSBData = offDuty;
            txCmd.MSBData = onDuty;
            delcom.SendCommand(txCmd); // Load the duty cycle

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 27;
            txCmd.LSBData = offset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 34;
            txCmd.LSBData = 1;
            txCmd.MSBData = power;
            delcom.SendCommand(txCmd); // Load the offset
        }

        public void BlueApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 23;
            txCmd.LSBData = offDuty;
            txCmd.MSBData = onDuty;
            delcom.SendCommand(txCmd); // Load the duty cycle

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 28;
            txCmd.LSBData = offset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 34;
            txCmd.LSBData = 2;
            txCmd.MSBData = power;
            delcom.SendCommand(txCmd); // Load the offset
        }

        public void Sync(Byte greenOffset, Byte redOffset, Byte blueOffset)
        {
            if (!IsPresent()) return;

            // Alwasy reload the offset, as it is cleared each time
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 26;
            txCmd.LSBData = greenOffset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 27;
            txCmd.LSBData = redOffset;
            delcom.SendCommand(txCmd); // Load the offset

            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 28;
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
            if (!IsPresent()) return;
            //MessageBox.Show("Prescaler out of range!\r\nPrescaler range 0-255\r\nUnits are in ms.\r\nDefault is 10ms", "Warning - Range Error!");

            // Always reload the offset, as it is cleared each time
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 19;
            txCmd.LSBData = prescaler;
            delcom.SendCommand(txCmd); // Load the offset
        }

        public void UpdateSwitch()
        {
            uint port0 = 0;
            if (!IsPresent()) return;
            delcom.ReadPort0(ref port0);
            @switch = (port0 & 0x1) != 0x01;
        }

        public void SwitchAudiConfirm(bool switchAudiConfirm)
        {
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 72;
            txCmd.LSBData = 0;
            txCmd.MSBData = 0;

            if (switchAudiConfirm) txCmd.MSBData = 0x80;
            else                   txCmd.LSBData = 0x80;
            delcom.SendCommand(txCmd); 
        }

        public void SwitchAutoClear(bool autoClear)
        {
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 72;
            txCmd.LSBData = 0;
            txCmd.MSBData = 0;

            if (autoClear) txCmd.MSBData = 0x40;
            else txCmd.LSBData = 0x40;
            delcom.SendCommand(txCmd); 
        }


        public void StartBuzzer(Byte freq, Byte repeat, Byte onTime, Byte offTime)
        {
            if (!IsPresent()) return;
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
            if (!IsPresent()) return;
            txCmd.MajorCmd = 101;
            txCmd.MinorCmd = 70;
            txCmd.LSBData = 0;      // 0=off. 1=on
            delcom.SendCommand(txCmd);  // always disable the flash mode 
        }
    }

    public enum Color : byte
    {
        Unknown = 0,
        Green = 1,
        Yellow = 2,
        Red = 4
    }
}