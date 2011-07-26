using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Emanate.Core.Output.DelcomVdi
{
    public class DelcomHid
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HidTxPacketStruct
        {
            public Byte MajorCmd;
            public Byte MinorCmd;
            public Byte LSBData;
            public Byte MSBData;
            public Byte HidData0;
            public Byte HidData1;
            public Byte HidData2;
            public Byte HidData3;
            public Byte ExtData0;
            public Byte ExtData1;
            public Byte ExtData2;
            public Byte ExtData3;
            public Byte ExtData4;
            public Byte ExtData5;
            public Byte ExtData6;
            public Byte ExtData7;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HidRxPacketStruct
        {
            public Byte Data0;
            public Byte Data1;
            public Byte Data2;
            public Byte Data3;
            public Byte Data4;
            public Byte Data5;
            public Byte Data6;
            public Byte Data7;


            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            //public byte[] Data;      //Data 1 .. 8
        }


        // Class variables
        private SafeFileHandle myDeviceHandle;
        private Boolean myDeviceDetected;
        private String myDevicePathName;
        private readonly DeviceManager myDeviceManager = new DeviceManager();
        private readonly Hid hid = new Hid();
        private const String MODULE_NAME = "Delcom HID USB CS";
        private UInt32 matchingDevicesFound;
        private HidTxPacketStruct myTxHidPacket;


        /// <summary>
        /// Reads the serial Number
        /// returns zero on sucess, else non-zero erro
        /// </summary>
        public UInt32 GetDeviceInfo(ref UInt32 serialNumber, ref UInt32 version, ref UInt32 date, ref UInt32 month, ref UInt32 year)
        {
            try
            {
                var buffer = new Byte[16];
                buffer[0] = 10;

                if (Hid.HidD_GetFeature(myDeviceHandle, buffer, 8) == false)
                    return 1;

                serialNumber = Convert.ToUInt32(buffer[0]);
                serialNumber += Convert.ToUInt32(buffer[1] << 8);
                serialNumber += Convert.ToUInt32(buffer[2] << 16);
                serialNumber += Convert.ToUInt32(buffer[3] << 24);

                // TODO: Check byte conversion is noop
                version = Convert.ToUInt32(Convert.ToByte(buffer[4]));
                date = Convert.ToUInt32(Convert.ToByte(buffer[5]));
                month = Convert.ToUInt32(Convert.ToByte(buffer[6]));
                year = Convert.ToUInt32(Convert.ToByte(buffer[7]));
                return (0);
            }

            catch (Exception ex)
            {
                //throw;
                return (2);
            }
        }


        /// <summary>
        /// Writes the ports values
        /// returns zero on sucess, else non-zero erro
        /// </summary>
        public UInt32 SendCommand(HidTxPacketStruct TxCmd)
        {

            try
            {
                int Length;
                if (TxCmd.MajorCmd == 102)
                    Length = 16;
                else
                    Length = 8;
                if (Hid.HidD_SetFeature(myDeviceHandle, StructureToByteArray(TxCmd), Length) == false)
                    return (1);
                else
                    return (0);
            }

            catch (Exception ex)
            {
                //throw;
                return (2);
            }
        }


        /// <summary>
        /// Writes the ports values
        /// returns zero on sucess, else non-zero erro
        /// </summary>
        public UInt32 WritePorts(UInt32 Port0, UInt32 Port1)
        {

            try
            {
                myTxHidPacket.MajorCmd = 101;
                myTxHidPacket.MinorCmd = 10;
                myTxHidPacket.LSBData = Convert.ToByte(Port0);
                myTxHidPacket.MSBData = Convert.ToByte(Port1);
                if (Hid.HidD_SetFeature(myDeviceHandle, StructureToByteArray(myTxHidPacket), 8) == false)
                    return (1);
                else
                    return (0);
            }

            catch (Exception ex)
            {
                //throw;
                return (2);
            }
        }



        /// <summary>
        /// Reads the currenly value at port zero
        /// returns zero on sucess, else non-zero erro
        /// </summary>
        public UInt32 ReadPort0(ref UInt32 Port0)
        {
            try
            {
                var buffer = new Byte[16];
                buffer[0] = 100;
                if (Hid.HidD_GetFeature(myDeviceHandle, buffer, 8) == false)
                    return 1;

                Port0 = Convert.ToUInt32(buffer[0]);
                return (0);

            }

            catch (Exception ex)
            {
                //throw;
                return (2);
            }
        }


        /// <summary>
        /// Reads the currenly value at both ports
        /// returns zero on sucess, else non-zero erro
        /// </summary>
        public UInt32 ReadPorts(ref UInt32 port0, ref UInt32 port1)
        {

            try
            {
                var buffer = new Byte[16];
                buffer[0] = 100;
                if (Hid.HidD_GetFeature(myDeviceHandle, buffer, 8) == false)
                    return 1;

                port0 = Convert.ToUInt32(buffer[0]);
                port1 = Convert.ToUInt32(buffer[1]);
                return (0);

            }

            catch (Exception ex)
            {
                //throw;
                return (2);
            }
        }

        /// <summary>
        /// Closed the USB HID devicde
        /// </summary>
        public UInt32 Close()
        {
            try
            {
                if (!myDeviceHandle.IsClosed)
                {
                    myDeviceHandle.Close();
                }
            }
            catch (Exception ex)
            {
                //throw;
                return (2);
            }
            return (0);
        }



        ///  <summary>
        /// Opens the first matching device found
        /// Return zero on success,
        /// otherwise non-zero error
        ///  </summary>
        public UInt32 Open()
        {
            return OpenNthDevice(1);
        }


        ///  <summary>
        /// Return non-zero if openned
        /// otherwise non-zero error
        ///  </summary>
        public bool IsOpen()
        {
            if (myDeviceHandle == null || myDeviceHandle.IsClosed || myDeviceHandle.IsClosed)
                return (false);
            else
                return (true);
        }


        ///  <summary>
        /// Opens the Nth matching device found
        /// Return zero on success,
        /// otherwise non-zero error
        ///  </summary>
        public UInt32 OpenNthDevice(UInt32 NthDevice)
        {
            if (myDeviceHandle != null)
                Close();    // if the device is already open, then close it first.

            if (!FindTheHid(NthDevice))
                return (1);
            myDeviceHandle = FileIO.CreateFile(myDevicePathName, FileIO.GENERIC_READ | FileIO.GENERIC_WRITE, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);
            if (myDeviceHandle.IsInvalid)
                return (2);
            return (0);  // device found
        }


        // Get the device name of the current device
        public string GetDeviceName()
        {
            return (myDevicePathName);
        }

        // Returns a count of the matching device on the current system
        public UInt32 GetDevicesCount()
        {
            FindTheHid(0);
            return (matchingDevicesFound);
        }




        ///  <summary>
        ///  Uses a series of API calls to locate a HID-class device
        ///  by its Vendor ID, Product ID and by the Nth number on the list.
        ///  NthDevice: 0=none, used to determine how many matching device are currently
        ///  installed on the system. 1=Find the first matching device, 2=the second matching device,
        ///  and so on...
        ///  </summary>
        ///          
        ///  <returns>
        ///   True if the device is detected, False if not detected.
        ///  </returns>
        private Boolean FindTheHid(UInt32 NthDevice)
        {
            var devicePathName = new String[512];      // Apr 20, 2009 - Increase size from 128 to 512.
            var hidGuid = Guid.Empty;

            const UInt16 productId = 0xB080;
            const UInt16 vendorId = 0x0FC5;
            UInt32 matchingDevices = 0;

            myDeviceDetected = false;
            Hid.HidD_GetHidGuid(ref hidGuid);       //Retrieves the interface class GUID for the HID class.

            //  Fill an array with the device path names of all attached HIDs.
            var deviceFound = myDeviceManager.FindDeviceFromGuid(hidGuid, ref devicePathName);

            //  If there is at least one HID, attempt to read the Vendor ID and Product ID
            //  of each device until there is a match or all devices have been examined.
            if (deviceFound)
            {
                var memberIndex = 0;
                do
                {
                    //  Open the device
                    var hidHandle = FileIO.CreateFile(devicePathName[memberIndex], 0, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);

                    if (!hidHandle.IsInvalid)
                    {   //  Device openned, now find out if it's the device we want.
                        hid.DeviceAttributes.Size = Marshal.SizeOf(hid.DeviceAttributes);
                        //  Retrieves a HIDD_ATTRIBUTES structure containing the Vendor ID, 
                        //  Product ID, and Product Version Number for a device.
                        var success = Hid.HidD_GetAttributes(hidHandle, ref hid.DeviceAttributes);
                        if (success)
                        {
                            //  Find out if the device matches the one we're looking for.
                            if ((hid.DeviceAttributes.VendorID == vendorId) & (hid.DeviceAttributes.ProductID == productId))
                            {
                                matchingDevices++;
                                myDeviceDetected = true;
                            }

                            if (myDeviceDetected && (matchingDevices == NthDevice))
                            {
                                // Device found
                                //  Save the DevicePathName
                                myDevicePathName = devicePathName[memberIndex];
                                hidHandle.Close();
                            }
                            else
                            {
                                //  It's not a match, so close the handle. try the next one
                                myDeviceDetected = false;
                                hidHandle.Close();
                            }
                        }
                        else
                        {
                            //  There was a problem in retrieving the information.
                            myDeviceDetected = false;
                            hidHandle.Close();
                        }
                    }

                    //  Keep looking until we find the device or there are no devices left to examine.
                    memberIndex = memberIndex + 1;
                }
                while (!((myDeviceDetected | (memberIndex == devicePathName.Length))));
            }

            matchingDevicesFound = matchingDevices; // save the device found count

            return myDeviceDetected;
        }

        // Converts a Structure to byte[]
        static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;

        }

        // Converts a byte[] to Structure
        static void ByteArrayToStructure(byte[] bytearray, ref object obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            Marshal.Copy(bytearray, 0, i, len);
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);

        }

    }   // end of the DelcomHID class
}
