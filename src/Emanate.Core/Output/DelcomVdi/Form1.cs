// Delcom C# USB HID Visual Indicator Example
// Dec 16, 2008
// This code show how to control the Delcom USB HID Device.

// Steps to make this project.
// 1 Create your project
// 2 Copy the follow file in the project source directory
//      DelcomHID.cs, DeviceManager.cs, DeviceManademenedDeclarations.cs
//      FileIODeclarations.cs, Hid.cs, HidDeclarations.cs
// 3 Add the above file to your project. Use add existing in Solution Explorer
// 4 Add the following inside your namespace:
//      DelcomHID Delcom = new DelcomHID();   // declare the Delcom class


using System;

namespace Emanate.Core.Output.DelcomVdi
{
    public class MainForm
    {
        private DelcomHid Delcom = new DelcomHid();   // declare the Delcom class
        private DelcomHid.HidTxPacketStruct TxCmd;
        private string label_DeviceName;
        private string label_DeviceStatus;
        private bool checkBox_SwitchAutoClear;
        private bool checkBox_SwitchAudiConfirm;
        private bool checkBox_Switch;
        private string textBox_PreScaler;
        private string textBox_GreenOffset;
        private string textBox_RedOffset;
        private string textBox_BlueOffset;
        private string textBox_BlueOffDuty;
        private string textBox_BlueOnDuty;
        private string textBox_BluePower;
        private string textBox_BuzzerFreq;
        private string textBox_BuzzerRepeat;
        private string textBox_BuzzerOnTime;
        private string textBox_BuzzerOffTime;
        private string textBox_RedOffDuty;
        private string textBox_RedOnDuty;
        private string textBox_RedPower;
        private string textBox_GreenOffDuty;
        private string textBox_GreenOnDuty;
        private string textBox_GreenPower;


        private void button_Open_Click(object sender, EventArgs e)
        {
            // Current TID and SID are not supported

            if (Delcom.Open() == 0)
            {
                UInt32 SerialNumber, Version, Date, Month, Year;
                SerialNumber = Version = Date = Month = Year = 0;
                Delcom.GetDeviceInfo(ref SerialNumber, ref Version, ref Date, ref Month, ref Year);
                Year += 2000;
                label_DeviceName = "DeviceName: "+Delcom.GetDeviceName();
                label_DeviceStatus = "Device Status: Found. SerialNumber=" + SerialNumber.ToString() + " Version=" + Version.ToString() + " " + Month.ToString() + "/" + Date.ToString() + "/" + Year.ToString();


                // Optionally -Enable event counter use that auto switch feature work
                TxCmd.MajorCmd = 101;
                TxCmd.MinorCmd = 38;
                TxCmd.LSBData = 1;
                TxCmd.MSBData = 0;
                Delcom.SendCommand(TxCmd); 

            }
            else
            {
                label_DeviceName = "DeviceName: offine";
                label_DeviceStatus = "Error: Unable to open device.";
            }
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Delcom.Close();
            label_DeviceName = "DeviceName: offine";
            label_DeviceStatus = "Device Closed.";
        }

        private Boolean IsPresent()
        {
            return Delcom.IsOpen();
        }


        private void button_GreenOff_Click(object sender, EventArgs e)
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

        private void button_GreenOn_Click(object sender, EventArgs e)
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

        private void button_GreenFlash_Click(object sender, EventArgs e)
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


        private void button_RedOff_Click(object sender, EventArgs e)
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

        private void button_RedOn_Click(object sender, EventArgs e)
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

        private void button_RedFlash_Click(object sender, EventArgs e)
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


        private void button_BlueOff_Click(object sender, EventArgs e)
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

        private void button_BlueOn_Click(object sender, EventArgs e)
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

        private void button_BlueFlash_Click(object sender, EventArgs e)
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

        private void button_GreenApply_Click(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            Byte OffDuty, OnDuty, Offset, Power;
            try
            {
                OffDuty = Convert.ToByte(textBox_GreenOffDuty);
                OnDuty = Convert.ToByte(textBox_GreenOnDuty);
                Offset = Convert.ToByte(textBox_GreenOffset);
                Power = Convert.ToByte(textBox_GreenPower);
                if (Power > 100) { Power = 0; Power /= Power; } // force the catch by divide by zero
            }
            catch (Exception ex)
            {
                //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
                return; //exit
            }
            
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 21;
            TxCmd.LSBData = OffDuty;
            TxCmd.MSBData = OnDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 26;
            TxCmd.LSBData = Offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = Power;
            Delcom.SendCommand(TxCmd); // Load the offset
        }


        private void button_RedApply_Click(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            Byte OffDuty, OnDuty, Offset, Power;
            try
            {
                OffDuty = Convert.ToByte(textBox_RedOffDuty);
                OnDuty = Convert.ToByte(textBox_RedOnDuty);
                Offset = Convert.ToByte(textBox_RedOffset);
                Power = Convert.ToByte(textBox_RedPower);
                if (Power > 100) { Power = 0; Power /= Power; } // force the catch by divide by zero
            }
            catch (Exception ex)
            {
                //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
                return; //exit
            }

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 22;
            TxCmd.LSBData = OffDuty;
            TxCmd.MSBData = OnDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 27;
            TxCmd.LSBData = Offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 1;
            TxCmd.MSBData = Power;
            Delcom.SendCommand(TxCmd); // Load the offset

        }
       

        private void button_BlueApply_Click(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            Byte OffDuty, OnDuty, Offset, Power;
            try
            {
                OffDuty = Convert.ToByte(textBox_BlueOffDuty);
                OnDuty = Convert.ToByte(textBox_BlueOnDuty);
                Offset = Convert.ToByte(textBox_BlueOffset);
                Power = Convert.ToByte(textBox_BluePower);
                if (Power > 100) { Power = 0; Power /= Power; } // force the catch by divide by zero
            }
            catch (Exception ex)
            {
                //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
                return; //exit
            }

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 23;
            TxCmd.LSBData = OffDuty;
            TxCmd.MSBData = OnDuty;
            Delcom.SendCommand(TxCmd); // Load the duty cycle

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 28;
            TxCmd.LSBData = Offset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 34;
            TxCmd.LSBData = 2;
            TxCmd.MSBData = Power;
            Delcom.SendCommand(TxCmd); // Load the offset


        }

        
        private void button_Sync_Click(object sender, EventArgs e)
        {
            
            if (!IsPresent()) return;
            Byte GreenOffset, RedOffset, BlueOffset;
            try
            {
                GreenOffset = Convert.ToByte(textBox_GreenOffset);
                RedOffset = Convert.ToByte(textBox_RedOffset);
                BlueOffset = Convert.ToByte(textBox_BlueOffset);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("LED Paramters out of range!\r\nDuty 0-255\r\nOffet 0-255.\r\nPower 0-100", "Warning - Range Error!");
                return; //exit
            }

            // Alwasy reload the offset, as it is clear each time
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 26;
            TxCmd.LSBData = GreenOffset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 27;
            TxCmd.LSBData = RedOffset;
            Delcom.SendCommand(TxCmd); // Load the offset

            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 28;
            TxCmd.LSBData = BlueOffset;
            Delcom.SendCommand(TxCmd); // Load the offset


            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 25;
            TxCmd.LSBData = 7;      // Sync Green, red & blue
            TxCmd.MSBData = 0;      // 0=off. 1=on - all on
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 
        }

        private void button_Prescaler_Click(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            Byte Prescaler;
            try
            {
                Prescaler = Convert.ToByte(textBox_PreScaler);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Prescaler out of range!\r\nPrescaler range 0-255\r\nUnits are in ms.\r\nDefault is 10ms", "Warning - Range Error!");
                return; //exit
            }

            // Alwasy reload the offset, as it is clear each time
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 19;
            TxCmd.LSBData = Prescaler;
            Delcom.SendCommand(TxCmd); // Load the offset

        }

        private void button_UpdateSwitch_Click(object sender, EventArgs e)
        {
            uint Port0 = 0;
            if (!IsPresent()) return;
            Delcom.ReadPort0(ref Port0);
            checkBox_Switch = (Port0 & 0x1) != 0x01;
        }

        private void checkBox_SwitchAudiConfirm_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 72;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 0;

            if (checkBox_SwitchAudiConfirm) TxCmd.MSBData = 0x80;
            else                                    TxCmd.LSBData = 0x80;
            Delcom.SendCommand(TxCmd); 
        }

        private void checkBox_SwitchAutoClear_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 72;
            TxCmd.LSBData = 0;
            TxCmd.MSBData = 0;

            if (checkBox_SwitchAutoClear) TxCmd.MSBData = 0x40;
            else TxCmd.LSBData = 0x40;
            Delcom.SendCommand(TxCmd); 
        }


        private void button_BuzzerStart_Click(object sender, EventArgs e)
        {

            if (!IsPresent()) return;
            Byte Freq, Repeat, OnTime, OffTime;
            try
            {
                Freq = Convert.ToByte(textBox_BuzzerFreq);
                Repeat = Convert.ToByte(textBox_BuzzerRepeat);
                OnTime = Convert.ToByte(textBox_BuzzerOnTime);
                OffTime = Convert.ToByte(textBox_BuzzerOffTime);
                if (Freq==0) { Freq /= Freq; } // force the catch by divide by zero
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Buzzer Paramters out of range!\r\nFreq 1-255 Value=1/(FreqHz*256E-6)\r\nRepeat 0-255 0=Continous, 255= Repeat forever.\r\nOnTime 0-100 Units ms.\r\nOffTime 0-100 Units ms.", "Warning - Range Error!");
                return; //exit
            }



            TxCmd.MajorCmd = 102;
            TxCmd.MinorCmd = 70;
            TxCmd.LSBData = 1;          // Enable buzzer
            TxCmd.MSBData = Freq;       // =1/(freqHz*256us)
            TxCmd.ExtData0 = Repeat;    // repeat value
            TxCmd.ExtData1 = OnTime;    // On time
            TxCmd.ExtData2 = OffTime;   // Off time

            Delcom.SendCommand(TxCmd);  // always disable the flash mode 

        }

        private void button_BuzzerStop_Click(object sender, EventArgs e)
        {
            if (!IsPresent()) return;
            TxCmd.MajorCmd = 101;
            TxCmd.MinorCmd = 70;
            TxCmd.LSBData = 0;      // 0=off. 1=on
            Delcom.SendCommand(TxCmd);  // always disable the flash mode 
        }
    }
}