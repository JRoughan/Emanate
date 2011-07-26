using System;

namespace Emanate.Core.Output.DelcomVdi
{
    public class MainForm
    {
        private readonly DelcomHid Delcom = new DelcomHid();
        private DelcomHid.HidTxPacketStruct TxCmd;
        private string deviceName;
        private string deviceStatus;
        private bool switchAudiConfirm;
        private bool @switch;
        private string preScaler;
        private string blueOffDuty;
        private string blueOnDuty;
        private string BluePower;
        private string BuzzerFreq;
        private string BuzzerRepeat;
        private string BuzzerOnTime;
        private string BuzzerOffTime;
        private string RedOffDuty;
        private string RedOnDuty;
        private string RedPower;
        private string GreenOffDuty;
        private string GreenOnDuty;
        private string GreenPower;


        public void Open()
        {
            // Current TID and SID are not supported

            if (Delcom.Open() == 0)
            {
                UInt32 serialNumber, version, date, month, year;
                serialNumber = version = date = month = year = 0;
                Delcom.GetDeviceInfo(ref serialNumber, ref version, ref date, ref month, ref year);
                year += 2000;
                deviceName = "DeviceName: "+Delcom.GetDeviceName();
                deviceStatus = "Device Status: Found. SerialNumber=" + serialNumber.ToString() + " Version=" + version.ToString() + " " + month.ToString() + "/" + date.ToString() + "/" + year.ToString();


                // Optionally -Enable event counter use that auto switch feature work
                TxCmd.MajorCmd = 101;
                TxCmd.MinorCmd = 38;
                TxCmd.LSBData = 1;
                TxCmd.MSBData = 0;
                Delcom.SendCommand(TxCmd); 

            }
            else
            {
                deviceName = "DeviceName: offine";
                deviceStatus = "Error: Unable to open device.";
            }
        }

        public void Close()
        {
            Delcom.Close();
            deviceName = "DeviceName: offine";
            deviceStatus = "Device Closed.";
        }

        private Boolean IsPresent()
        {
            return Delcom.IsOpen();
        }


        public void GreenOff()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 1;
            Delcom.SendCommand(TxCmd);
        }

        public void GreenOn()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);
        }

        public void GreenFlash()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 1;
            Delcom.SendCommand(TxCmd);  // enable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd); // and turn it on
        }


        public void RedOff()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 2;
            Delcom.SendCommand(TxCmd);
        }

        public void RedOn()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);
        }

        public void RedFlash()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 2;
            Delcom.SendCommand(TxCmd);  // enable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd); // and turn it on
        }


        public void BlueOff()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 4;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 4;
            Delcom.SendCommand(TxCmd);
        }

        public void BlueOn()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 4;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 4;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd);
        }

        public void BlueFlash()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 20;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 4;
            Delcom.SendCommand(TxCmd);  // enable the flash mode 

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 12;
            TxCmd.LSBData = 4;
            TxCmd.MSBData = 0;
            Delcom.SendCommand(TxCmd); // and turn it on
        }

        public void GreenApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
            
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 21;
            TxCmd.LSBData = offDuty;
            TxCmd.MSBData = onDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 26;
            TxCmd.LSBData = offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = power;
            Delcom.SendCommand(TxCmd); // Load the offset
        }

        public void RedApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 22;
            TxCmd.LSBData = offDuty;
            TxCmd.MSBData = onDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 27;
            TxCmd.LSBData = offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = power;
            Delcom.SendCommand(TxCmd); // Load the offset
        }

        public void BlueApply(Byte offDuty, Byte onDuty, Byte offset, Byte power)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 23;
            TxCmd.LSBData = offDuty;
            TxCmd.MSBData = onDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 28;
            TxCmd.LSBData = offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = power;
            Delcom.SendCommand(TxCmd); // Load the offset
        }

        public void Sync(Byte greenOffset, Byte redOffset, Byte blueOffset)
        {
            if (!IsPresent()) return;

            // Alwasy reload the offset, as it is cleared each time
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 26;
            TxCmd.LSBData = greenOffset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 27;
            TxCmd.LSBData = redOffset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 28;
            TxCmd.LSBData = blueOffset;
            Delcom.SendCommand(TxCmd); // Load the offset


            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 25;
            TxCmd.LSBData = 7;      // Sync Green, red & blue
            TxCmd.MSBData = 0;      // 0=off. 1=on - all on
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 
        }

        public void Prescaler()
        {
            if (!IsPresent()) return;
            Byte prescaler;
            try
            {
                prescaler = Convert.ToByte(preScaler);
            }
            catch (Exception)
            {
                //MessageBox.Show("Prescaler out of range!\r\nPrescaler range 0-255\r\nUnits are in ms.\r\nDefault is 10ms", "Warning - Range Error!");
                return; //exit
            }

            // Alwasy reload the offset, as it is clear each time
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 19;
            TxCmd.LSBData = prescaler;
            Delcom.SendCommand(TxCmd); // Load the offset
        }

        public void UpdateSwitch()
        {
            uint port0 = 0;
            if (!IsPresent()) return;
            Delcom.ReadPort0(ref port0);
            @switch = (port0 & 0x1) != 0x01;
        }

        public void SwitchAudiConfirm()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 72;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 0;

            if (switchAudiConfirm) TxCmd.MSBData = 0x80;
            else                            TxCmd.LSBData = 0x80;
            Delcom.SendCommand(TxCmd); 
        }

        public void SwitchAutoClear(bool autoClear)
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 72;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 0;

            if (autoClear) TxCmd.MSBData = 0x40;
            else TxCmd.LSBData = 0x40;
            Delcom.SendCommand(TxCmd); 
        }


        public void StartBuzzer(Byte freq, Byte repeat, Byte onTime, Byte offTime)
        {
            if (!IsPresent()) return;
            //MessageBox.Show("Buzzer Paramters out of range!\r\nFreq 1-255 Value=1/(FreqHz*256E-6)\r\nRepeat 0-255 0=Continous, 255= Repeat forever.\r\nOnTime 0-100 Units ms.\r\nOffTime 0-100 Units ms.", "Warning - Range Error!");

            TxCmd.MajorCmd = 102;
            TxCmd.MinorCmd = 70;
            TxCmd.LSBData = 1;          // Enable buzzer
            TxCmd.MSBData = freq;       // =1/(freqHz*256us)
            TxCmd.ExtData0 = repeat;    // repeat value
            TxCmd.ExtData1 = onTime;    // On time
            TxCmd.ExtData2 = offTime;   // Off time

            Delcom.SendCommand(TxCmd);  // always disable the flash mode 
        }

        public void StopBuzzer()
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 70;
            TxCmd.LSBData = 0;      // 0=off. 1=on
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 
        }
    }
}