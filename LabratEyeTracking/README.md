# LabratEyeTracking

Bring Eye tracking to SCP: Labrat

## Mod Loader

[BepInEx v5.4.11](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.11)

## Installing

1) Move DLL to `[SCP Labrat Game Folder]/BepInEx/plugins`
2) Run the Game at least once
3) Navigate to `BepInEx/config/lol.fortnite.www.labrateyetracking.cfg` and open the file with a text editor (I use VSCode)
4) Set the Configuration value to either 1 or 2 (see the table below)

## Eye Trackers

|SDK|SDK Type<int>|Unmanaged Libraries|
|---|---|---|
|SRanipal|1|[Only the DLLs](https://github.com/200Tigersbloxed/UnityMods/tree/main/LabratEyeTracking/LabratEyeTracking/SRanipal)|
|Pimax (aSeeVRClient)|2|[Only the DLLs](https://github.com/200Tigersbloxed/UnityMods/tree/main/LabratEyeTracking/LabratEyeTracking/PimaxEyeTracker)|

## Info

*May Require Extra Installation, see notes below*

Enables use of the SRanipal SDK to detect if a user has their eyes open or not.

**NOTE:** This mod automatically creates the *unmanaged* assemblies required on launch. If something goes wrong, please see the Unmanaged libraries (see table above) and download the DLLs for your SDK type and move them to the root of your game directory.

**NOTE:** This *only* works with the SRanipal SDK and aSeeVRClient (Pimax). Any HMDs that have eye tracking, but *don't* use the SRanipal SDK or aSeeVRClient are NOT supported.

Latest Release: https://github.com/200Tigersbloxed/UnityMods/releases/tag/LET-v1.2.0
  
## Credits

+ [PimaxEyeTracker](https://github.com/NGenesis/PimaxEyeTracker) by NGenesis
+ [SRanipalTrackingInterface.cs](https://github.com/benaclejames/VRCFaceTracking/blob/master/VRCFaceTracking/SRanipalTrackingInterface.cs) by benaclejames
