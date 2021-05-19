# UnityMods
A collection of my mods for games that run on the Unity Engine.

## SCP: Labrat Mods

### LabratEyeTracking

#### Mod Loader

[BepInEx v5.4.11](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.11)

#### Installing

Move DLL to `[SCP Labrat Game Folder]/BepInEx/plugins`

#### Info

*May Require Extra Installation, see notes below*

Enables use of the SRanipal SDK to detect if a user has their eyes open or not.

**NOTE:** This mod automatically creates the *unmanaged* assemblies required on launch. If anything goes wrong, take all the dll files from [this directory](https://github.com/200Tigersbloxed/UnityMods/tree/main/LabratEyeTracking/LabratEyeTracking/SRanipal) and move them into the root of your Labrat game folder.

**NOTE:** This *only* works with the SRanipal SDK. Any HMDs that have eye tracking, but *don't* use the SRanipal SDK are NOT supported.

Last Release: https://github.com/200Tigersbloxed/UnityMods/releases/tag/LET-v1.0.0
