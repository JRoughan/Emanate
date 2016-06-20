using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
// ReSharper disable All

namespace Emanate.Delcom
{    
    internal sealed partial class Hid  
    {
        private HIDP_CAPS capabilities; 
        internal HIDD_ATTRIBUTES DeviceAttributes; 
        
        ///  <summary>
        ///  For reports the device sends to the host.
        ///  </summary>
        internal abstract class ReportIn  
        {             
            ///  <summary>
            ///  Each class that handles reading reports defines a Read method for reading 
            ///  a type of report. Read is declared as a Sub rather
            ///  than as a Function because asynchronous reads use a callback method 
            ///  that can access parameters passed by ByRef but not Function return values.
            ///  </summary>
            internal abstract bool Read( SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref Boolean myDeviceDetected, ref Byte[] readBuffer);
         }      
        
        ///  <summary>
        ///  For reading Feature reports.
        ///  </summary>
        
        internal class InFeatureReport : ReportIn 
        {             
            ///  <summary>
            ///  reads a Feature report from the device.
            ///  </summary>
            ///  
            ///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
            ///  <param name="readHandle"> the handle for reading Input reports from the device. </param>
            ///  <param name="writeHandle"> the handle for writing Output reports to the device. </param>
            ///  <param name="myDeviceDetected"> tells whether the device is currently attached.</param>
            ///  <param name="inFeatureReportBuffer"> contains the requested report.</param>
            internal override bool Read( SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref Boolean myDeviceDetected, ref Byte[] inFeatureReportBuffer) 
            {
                //  ***
                //  API function: HidD_GetFeature
                //  Attempts to read a Feature report from the device.
                    
                //  Requires:
                //  A handle to a HID
                //  A pointer to a buffer containing the report ID and report
                //  The size of the buffer. 
                    
                //  Returns: true on success, false on failure.
                //  ***                    
                   
                return HidD_GetFeature(hidHandle, inFeatureReportBuffer, inFeatureReportBuffer.Length);
            }             
        }         
        
        ///  <summary>
        ///  For reading Input reports via control transfers
        ///  </summary>
        
        internal class InputReportViaControlTransfer : ReportIn 
        {             
            ///  <summary>
            ///  reads an Input report from the device using a control transfer.
            ///  </summary>
            ///  
            ///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
            ///  <param name="readHandle"> the handle for reading Input reports from the device. </param>
            ///  <param name="writeHandle"> the handle for writing Output reports to the device. </param>
            ///  <param name="myDeviceDetected"> tells whether the device is currently attached. </param>
            ///  <param name="inputReportBuffer"> contains the requested report. </param>
            internal override bool Read( SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref Boolean myDeviceDetected, ref Byte[] inputReportBuffer) 
            {
                //  ***
                //  API function: HidD_GetInputReport
                    
                //  Purpose: Attempts to read an Input report from the device using a control transfer.
                //  Supported under Windows XP and later only.
                    
                //  Requires:
                //  A handle to a HID
                //  A pointer to a buffer containing the report ID and report
                //  The size of the buffer. 
                    
                //  Returns: true on success, false on failure.
                //  ***
                    
                return HidD_GetInputReport(hidHandle, inputReportBuffer, inputReportBuffer.Length + 1); 
            }      
        }         
        
        ///  <summary>
        ///  For reading Input reports.
        ///  </summary>
        
        internal class InputReportViaInterruptTransfer : ReportIn 
        {
            ///  <summary>
            ///  closes open handles to a device.
            ///  </summary>
            ///  
            ///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
            ///  <param name="readHandle"> the handle for reading Input reports from the device. </param>
            ///  <param name="writeHandle"> the handle for writing Output reports to the device. </param>
            private void CancelTransfer(SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle) 
            {
                //  ***
                //  API function: CancelIo
                    
                //  Purpose: Cancels a call to ReadFile
                    
                //  Accepts: the device handle.
                    
                //  Returns: True on success, False on failure.
                //  ***
                    
                NativeMethods.CancelIo(readHandle);               
                                                           
                //  The failure may have been because the device was removed,
                //  so close any open handles and
                //  set myDeviceDetected=False to cause the application to
                //  look for the device on the next attempt.
                    
                if ( ( !( hidHandle.IsInvalid ) ) ) 
                { 
                    hidHandle.Close(); 
                } 
                    
                if ( ( !( readHandle.IsInvalid ) ) ) 
                { 
                    readHandle.Close(); 
                } 
                    
                if ( ( !( writeHandle.IsInvalid ) ) ) 
                { 
                    writeHandle.Close(); 
                }
            }             
            
            ///  <summary>
            ///  Creates an event object for the overlapped structure used with ReadFile. 
            ///  </summary>
            ///  
            ///  <param name="hidOverlapped"> the overlapped structure </param>
            ///  <param name="eventObject"> the event object </param>
            private void PrepareForOverlappedTransfer(ref NativeOverlapped hidOverlapped, out IntPtr eventObject) 
            {
                //  ***
                //  API function: CreateEvent
                    
                //  Purpose: Creates an event object for the overlapped structure used with ReadFile.
                    
                //  Accepts:
                //  A security attributes structure or IntPtr.Zero.
                //  Manual Reset = False (The system automatically resets the state to nonsignaled 
                //  after a waiting thread has been released.)
                //  Initial state = False (not signaled)
                //  An event object name (optional)
                    
                //  Returns: a handle to the event object
                //  ***

                eventObject = NativeMethods.CreateEvent(IntPtr.Zero, false, false, "");                     
                                    
                //  Set the members of the overlapped structure.
                    
                hidOverlapped.OffsetLow = 0; 
                hidOverlapped.OffsetHigh = 0; 
                hidOverlapped.EventHandle = eventObject;
            }             
            
            ///  <summary>
            ///  reads an Input report from the device using interrupt transfers.
            ///  </summary>
            ///  
            ///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
            ///  <param name="readHandle"> the handle for reading Input reports from the device. </param>
            ///  <param name="writeHandle"> the handle for writing Output reports to the device. </param>
            ///  <param name="myDeviceDetected"> tells whether the device is currently attached. </param>
            ///  <param name="inputReportBuffer"> contains the requested report. </param>
            internal override bool Read( SafeFileHandle hidHandle, SafeFileHandle readHandle, SafeFileHandle writeHandle, ref Boolean myDeviceDetected, ref Byte[] inputReportBuffer) 
            {                 
                IntPtr eventObject;
				var hidOverlapped = new NativeOverlapped();

                //  Set up the overlapped structure for ReadFile.
                    
                PrepareForOverlappedTransfer( ref hidOverlapped, out eventObject );

                // Allocate memory for the input buffer and overlapped structure. 

                var nonManagedBuffer = Marshal.AllocHGlobal(inputReportBuffer.Length);
                var nonManagedOverlapped = Marshal.AllocHGlobal(Marshal.SizeOf(hidOverlapped));
                Marshal.StructureToPtr(hidOverlapped, nonManagedOverlapped, false);			
                                        
                //  ***
                //  API function: ReadFile
                //  Purpose: Attempts to read an Input report from the device.
                    
                //  Accepts:
                //  A device handle returned by CreateFile
                //  (for overlapped I/O, CreateFile must have been called with FILE_FLAG_OVERLAPPED),
                //  A pointer to a buffer for storing the report.
                //  The Input report length in bytes returned by HidP_GetCaps,
                //  A pointer to a variable that will hold the number of bytes read. 
                //  An overlapped structure whose hEvent member is set to an event object.
                    
                //  Returns: the report in ReadBuffer.
                    
                //  The overlapped call returns immediately, even if the data hasn't been received yet.
                    
                //  To read multiple reports with one ReadFile, increase the size of ReadBuffer
                //  and use NumberOfBytesRead to determine how many reports were returned.
                //  Use a larger buffer if the application can't keep up with reading each report
                //  individually. 
                //  ***                    

                var numberOfBytesRead = 0;
                var success = NativeMethods.ReadFile(readHandle, nonManagedBuffer, inputReportBuffer.Length, ref numberOfBytesRead, nonManagedOverlapped);
 
                if (!success)
                {
                    Debug.WriteLine("waiting for ReadFile");

                    //  API function: WaitForSingleObject

                    //  Purpose: waits for at least one report or a timeout.
                    //  Used with overlapped ReadFile.

                    //  Accepts:
                    //  An event object created with CreateEvent
                    //  A timeout value in milliseconds.

                    //  Returns: A result code.

                    var result = NativeMethods.WaitForSingleObject(eventObject, 3000);

                    //  Find out if ReadFile completed or timeout.

                    switch (result)
                    {
                        case NativeMethods.WAIT_OBJECT_0:

                            //  ReadFile has completed

                            success = true;
                            Debug.WriteLine("ReadFile completed successfully.");

                            // Get the number of bytes read.

                            //  API function: GetOverlappedResult

                            //  Purpose: gets the result of an overlapped operation.
								
                            //  Accepts:
                            //  A device handle returned by CreateFile.
                            //  A pointer to an overlapped structure.
                            //  A pointer to a variable to hold the number of bytes read.
                            //  False to return immediately.
								
                            //  Returns: non-zero on success and the number of bytes read.	

                            NativeMethods.GetOverlappedResult(readHandle, nonManagedOverlapped, ref numberOfBytesRead, false);

                            break;

                        case NativeMethods.WAIT_TIMEOUT:

                            //  Cancel the operation on timeout

                            CancelTransfer(hidHandle, readHandle, writeHandle);
                            Debug.WriteLine("Readfile timeout");
                            myDeviceDetected = false;
                            break;
                        default:

                            //  Cancel the operation on other error.

                            CancelTransfer(hidHandle, readHandle, writeHandle);
                            Debug.WriteLine("Readfile undefined error");
                            myDeviceDetected = false;
                            break;
                    }
						
                }
                if (success)
                {
                    // A report was received.
                    // Copy the received data to inputReportBuffer for the application to use.

                    Marshal.Copy(nonManagedBuffer, inputReportBuffer, 0, numberOfBytesRead);
                }
                return success;
            }             
        } 
                
        ///  <summary>
        ///  For reports the host sends to the device.
        ///  </summary>
        
        internal abstract class ReportOut  
        {            
            ///  <summary>
            ///  Each class that handles writing reports defines a Write method for 
            ///  writing a type of report.
            ///  </summary>
            ///  
            ///  <param name="reportBuffer"> contains the report ID and report data. </param>
            ///   <param name="deviceHandle"> handle to the device.  </param>
            ///  
            ///  <returns>
            ///   True on success. False on failure.
            ///  </returns>             
            
            internal abstract Boolean Write( Byte[] reportBuffer, SafeFileHandle deviceHandle );      
        } 
        
        ///  <summary>
        ///  For Feature reports the host sends to the device.
        ///  </summary>
        internal class OutFeatureReport : ReportOut 
        {            
            ///  <summary>
            ///  writes a Feature report to the device.
            ///  </summary>
            ///  
            ///  <param name="outFeatureReportBuffer"> contains the report ID and report data. </param>
            ///  <param name="hidHandle"> handle to the device.  </param>
            ///  
            ///  <returns>
            ///   True on success. False on failure.
            ///  </returns>            
            internal override bool Write( Byte[] outFeatureReportBuffer, SafeFileHandle hidHandle ) 
            {
                //  ***
                //  API function: HidD_SetFeature
                    
                //  Purpose: Attempts to send a Feature report to the device.
                    
                //  Accepts:
                //  A handle to a HID
                //  A pointer to a buffer containing the report ID and report
                //  The size of the buffer. 
                    
                //  Returns: true on success, false on failure.
                //  ***
                                      
                var success = HidD_SetFeature(hidHandle, outFeatureReportBuffer, outFeatureReportBuffer.Length); 
                    
                Debug.Print( "HidD_SetFeature success = " + success ); 
                    
                return success;
            }             
        } 
                
        ///  <summary>
        ///  For writing Output reports via control transfers
        ///  </summary>
        
        internal class OutputReportViaControlTransfer : ReportOut 
        {             
            ///  <summary>
            ///  writes an Output report to the device using a control transfer.
            ///  </summary>
            ///  
            ///  <param name="outputReportBuffer"> contains the report ID and report data. </param>
            ///  <param name="hidHandle"> handle to the device.  </param>
            ///  
            ///  <returns>
            ///   True on success. False on failure.
            ///  </returns>            
            
            internal override Boolean Write( Byte[] outputReportBuffer, SafeFileHandle hidHandle ) 
            {
                //  ***
                //  API function: HidD_SetOutputReport
                    
                //  Purpose: 
                //  Attempts to send an Output report to the device using a control transfer.
                //  Requires Windows XP or later.
                    
                //  Accepts:
                //  A handle to a HID
                //  A pointer to a buffer containing the report ID and report
                //  The size of the buffer. 
                    
                //  Returns: true on success, false on failure.
                //  ***                    
                   
                var success = HidD_SetOutputReport(hidHandle, outputReportBuffer, outputReportBuffer.Length + 1); 
                    
                Debug.Print( "HidD_SetOutputReport success = " + success ); 
                    
                return success;
            }           
        }       
        
        ///  <summary>
        ///  For Output reports the host sends to the device.
        ///  Uses interrupt or control transfers depending on the device and OS.
        ///  </summary>
        
        internal class OutputReportViaInterruptTransfer : ReportOut 
        {             
            ///  <summary>
            ///  writes an Output report to the device.
            ///  </summary>
            ///  
            ///  <param name="outputReportBuffer"> contains the report ID and report data. </param>
            ///  <param name="writeHandle"> handle to the device.  </param>
            ///  
            ///  <returns>
            ///   True on success. False on failure.
            ///  </returns>            
            
            internal override Boolean Write( Byte[] outputReportBuffer, SafeFileHandle writeHandle ) 
            {
                //  The host will use an interrupt transfer if the the HID has an interrupt OUT
                //  endpoint (requires USB 1.1 or later) AND the OS is NOT Windows 98 Gold (original version). 
                //  Otherwise the the host will use a control transfer.
                //  The application doesn't have to know or care which type of transfer is used.
                    
                var numberOfBytesWritten = 0; 
                    
                //  ***
                //  API function: WriteFile
                    
                //  Purpose: writes an Output report to the device.
                    
                //  Accepts:
                //  A handle returned by CreateFile
                //  An integer to hold the number of bytes written.
                    
                //  Returns: True on success, False on failure.
                //  ***
                    
                var success = NativeMethods.WriteFile(writeHandle, outputReportBuffer, outputReportBuffer.Length, ref numberOfBytesWritten, IntPtr.Zero);
                    
                Debug.Print( "WriteFile success = " + success ); 
                    
                if ( !( ( success ) ) ) 
                { 
                        
                    if ( ( !( writeHandle.IsInvalid ) ) ) 
                    { 
                        writeHandle.Close(); 
                    } 
                }                     
                return success;
            }           
        } 
     
        ///  <summary>
        ///  Remove any Input reports waiting in the buffer.
        ///  </summary>
        ///  
        ///  <param name="hidHandle"> a handle to a device.   </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        
        internal Boolean FlushQueue( SafeFileHandle hidHandle )
        {
            //  ***
            //  API function: HidD_FlushQueue
                
            //  Purpose: Removes any Input reports waiting in the buffer.
                
            //  Accepts: a handle to the device.
                
            //  Returns: True on success, False on failure.
            //  ***

            return HidD_FlushQueue( hidHandle );
        }

        ///  <summary>
        ///  Retrieves a structure with information about a device's capabilities. 
        ///  </summary>
        ///  
        ///  <param name="hidHandle"> a handle to a device. </param>
        ///  
        ///  <returns>
        ///  An HIDP_CAPS structure.
        ///  </returns>
        internal HIDP_CAPS GetDeviceCapabilities( SafeFileHandle hidHandle ) 
        {             
            var preparsedData = new IntPtr();
            //Byte[] valueCaps = new Byte[ 1024 ]; // (the array size is a guess)

			try
			{
				//  ***
				//  API function: HidD_GetPreparsedData

				//  Purpose: retrieves a pointer to a buffer containing information about the device's capabilities.
				//  HidP_GetCaps and other API functions require a pointer to the buffer.

				//  Requires: 
				//  A handle returned by CreateFile.
				//  A pointer to a buffer.

				//  Returns:
				//  True on success, False on failure.
				//  ***

				HidD_GetPreparsedData(hidHandle, ref preparsedData);

				//  ***
				//  API function: HidP_GetCaps

				//  Purpose: find out a device's capabilities.
				//  For standard devices such as joysticks, you can find out the specific
				//  capabilities of the device.
				//  For a custom device where the software knows what the device is capable of,
				//  this call may be unneeded.

				//  Accepts:
				//  A pointer returned by HidD_GetPreparsedData
				//  A pointer to a HIDP_CAPS structure.

				//  Returns: True on success, False on failure.
				//  ***

				var result = HidP_GetCaps(preparsedData, ref capabilities);
				if ((result != 0))
				{
					Debug.WriteLine("");
					Debug.WriteLine("  Usage: " + Convert.ToString(capabilities.Usage, 16));
					Debug.WriteLine("  Usage Page: " + Convert.ToString(capabilities.UsagePage, 16));
					Debug.WriteLine("  Input Report Byte Length: " + capabilities.InputReportByteLength);
					Debug.WriteLine("  Output Report Byte Length: " + capabilities.OutputReportByteLength);
					Debug.WriteLine("  Feature Report Byte Length: " + capabilities.FeatureReportByteLength);
					Debug.WriteLine("  Number of Link Collection Nodes: " + capabilities.NumberLinkCollectionNodes);
					Debug.WriteLine("  Number of Input Button Caps: " + capabilities.NumberInputButtonCaps);
					Debug.WriteLine("  Number of Input Value Caps: " + capabilities.NumberInputValueCaps);
					Debug.WriteLine("  Number of Input Data Indices: " + capabilities.NumberInputDataIndices);
					Debug.WriteLine("  Number of Output Button Caps: " + capabilities.NumberOutputButtonCaps);
					Debug.WriteLine("  Number of Output Value Caps: " + capabilities.NumberOutputValueCaps);
					Debug.WriteLine("  Number of Output Data Indices: " + capabilities.NumberOutputDataIndices);
					Debug.WriteLine("  Number of Feature Button Caps: " + capabilities.NumberFeatureButtonCaps);
					Debug.WriteLine("  Number of Feature Value Caps: " + capabilities.NumberFeatureValueCaps);
					Debug.WriteLine("  Number of Feature Data Indices: " + capabilities.NumberFeatureDataIndices);

					//  ***
					//  API function: HidP_GetValueCaps

					//  Purpose: retrieves a buffer containing an array of HidP_ValueCaps structures.
					//  Each structure defines the capabilities of one value.
					//  This application doesn't use this data.

					//  Accepts:
					//  A report type enumerator from hidpi.h,
					//  A pointer to a buffer for the returned array,
					//  The NumberInputValueCaps member of the device's HidP_Caps structure,
					//  A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.

					//  Returns: True on success, False on failure.
					//  ***                    


					Int32 vcSize = capabilities.NumberInputValueCaps;
					var valueCaps = new Byte[vcSize];
					HidP_GetValueCaps(HidP_Input, valueCaps, ref vcSize, preparsedData);
				}
			}
			finally
			{
				 //  ***
					//  API function: HidD_FreePreparsedData
                    
					//  Purpose: frees the buffer reserved by HidD_GetPreparsedData.
                    
					//  Accepts: A pointer to the PreparsedData structure returned by HidD_GetPreparsedData.
                    
					//  Returns: True on success, False on failure.
					//  ***

				if (preparsedData != IntPtr.Zero)
				{
					HidD_FreePreparsedData(preparsedData);
				}
			} 
            
            return capabilities;             
        }         
        
        ///  <summary>
        ///  Creates a 32-bit Usage from the Usage Page and Usage ID. 
        ///  Determines whether the Usage is a system mouse or keyboard.
        ///  Can be modified to detect other Usages.
        ///  </summary>
        ///  
        ///  <param name="myCapabilities"> a HIDP_CAPS structure retrieved with HidP_GetCaps. </param>
        ///  
        ///  <returns>
        ///  A String describing the Usage.
        ///  </returns>
        internal String GetHidUsage( HIDP_CAPS myCapabilities ) 
        {
            var usageDescription = "";

            //  Create32-bit Usage from Usage Page and Usage ID.
                
            int usage = myCapabilities.UsagePage * 256 + myCapabilities.Usage; 
                
            if ( usage == Convert.ToInt32( 0X102 ) )
            { 
                usageDescription = "mouse"; } 
                
            if ( usage == Convert.ToInt32( 0X106 ) )
            { 
                usageDescription = "keyboard"; }

            return usageDescription;             
        }         
        
        ///  <summary>
        ///  Retrieves the number of Input reports the host can store.
        ///  </summary>
        ///  
        ///  <param name="hidDeviceObject"> a handle to a device  </param>
        ///  <param name="numberOfInputBuffers"> an integer to hold the returned value. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        internal Boolean GetNumberOfInputBuffers( SafeFileHandle hidDeviceObject, ref Int32 numberOfInputBuffers ) 
        {             
            Boolean success;

            if (!((IsWindows98Gold())))
            {
                //  ***
                //  API function: HidD_GetNumInputBuffers

                //  Purpose: retrieves the number of Input reports the host can store.
                //  Not supported by Windows 98 Gold.
                //  If the buffer is full and another report arrives, the host drops the 
                //  ldest report.

                //  Accepts: a handle to a device and an integer to hold the number of buffers. 

                //  Returns: True on success, False on failure.
                //  ***

                success = HidD_GetNumInputBuffers(hidDeviceObject, ref numberOfInputBuffers);
            }
            else
            {
                //  Under Windows 98 Gold, the number of buffers is fixed at 2.

                numberOfInputBuffers = 2;
                success = true;
            }

            return success;
        } 
                
        ///  <summary>
        ///  sets the number of input reports the host will store.
        ///  Requires Windows XP or later.
        ///  </summary>
        ///  
        ///  <param name="hidDeviceObject"> a handle to the device.</param>
        ///  <param name="numberBuffers"> the requested number of input reports.  </param>
        ///  
        ///  <returns>
        ///  True on success. False on failure.
        ///  </returns>
        internal Boolean SetNumberOfInputBuffers( SafeFileHandle hidDeviceObject, Int32 numberBuffers )
        {
            if (IsWindows98Gold())
            {
                //  Not supported under Windows 98 Gold.
                return false;
            }

            //  ***
            //  API function: HidD_SetNumInputBuffers

            //  Purpose: Sets the number of Input reports the host can store.
            //  If the buffer is full and another report arrives, the host drops the 
            //  oldest report.

            //  Requires:
            //  A handle to a HID
            //  An integer to hold the number of buffers. 

            //  Returns: true on success, false on failure.
            //  ***

            HidD_SetNumInputBuffers(hidDeviceObject, numberBuffers);
            return true;
        }

        ///  <summary>
        ///  Find out if the current operating system is Windows XP or later.
        ///  (Windows XP or later is required for HidD_GetInputReport and HidD_SetInputReport.)
        ///  </summary>
        internal Boolean IsWindowsXpOrLater() 
        {
            var myEnvironment = Environment.OSVersion; 
                
            //  Windows XP is version 5.1.
                
            var versionXP = new Version( 5, 1 );

            if (myEnvironment.Version >= versionXP)                 
            { 
                Debug.Write( "The OS is Windows XP or later." ); 
                return true; 
            }
            Debug.Write( "The OS is earlier than Windows XP." ); 
            return false;
        }         
        
        ///  <summary>
        ///  Find out if the current operating system is Windows 98 Gold (original version).
        ///  Windows 98 Gold does not support the following:
        ///  Interrupt OUT transfers (WriteFile uses control transfers and Set_Report).
        ///  HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
        ///  (Not yet tested on a Windows 98 Gold system.)
        ///  </summary>
        private Boolean IsWindows98Gold() 
        {
            Boolean result;
            var myEnvironment = Environment.OSVersion; 
                
            //  Windows 98 Gold is version 4.10 with a build number less than 2183.
                
            var version98SE = new Version( 4, 10, 2183 );

            if (myEnvironment.Version < version98SE)                
            { 
                Debug.Write( "The OS is Windows 98 Gold." );
                result = true; 
            } 
            else 
            { 
                Debug.Write( "The OS is more recent than Windows 98 Gold." );
                result = false; 
            }
            return result;
        }
    } 
}

