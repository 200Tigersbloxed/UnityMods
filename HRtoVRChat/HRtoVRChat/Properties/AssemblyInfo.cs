using System.Runtime.InteropServices;
using MelonLoader;
using HRtoVRChat;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties


// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.

[assembly: Guid("e60e13d3-e714-4d88-af0d-7df285f13178")]

[assembly: MelonInfo(typeof(MainMod), "HRtoVRChat", "v1.4.0", "200Tigersbloxed")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonOptionalDependencies(new string[] { "UIExpansionKit", "ActionMenuApi", "Windows.Foundation.FoundationContract", "Windows.Foundation.UniversalApiContract" })]