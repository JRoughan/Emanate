using System.Reflection;
using System.Runtime.InteropServices;
using Emanate.Core;
using Emanate.Vso.Admin;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Emanate.Vso.Admin")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("43c8d75b-e435-4bac-a2b0-2ebfdbf8d29e")]

[assembly: EmanateAdminModule(typeof(VsoAdminModule))]

