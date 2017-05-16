using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Emanate.Core;
using Emanate.TeamCity2017;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Emanate.TeamCity2017")]
[assembly: AssemblyDescription("TeamCity module for Emanate build monitor. 2017 Edition")]
[assembly: AssemblyConfiguration("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("Emanate.UnitTests")]
[assembly: InternalsVisibleTo("Emanate.IntegrationTests")]

[assembly: EmanateModule(typeof(TeamCity2017Module))]
