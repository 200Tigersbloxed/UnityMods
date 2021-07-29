# Customization
A guide on how to customize your HideWithCanvasVRC mod

## Getting the Environment Setup

First, I just created an empty scene with Unity 2018.4.20f1. Once the scene loads, go to `Window > Package Manager`

We'll need to import two packages:
+ TextMeshPro
+ Asset Bundle Browser

## Importing the Template

Once you import both of the packages, you can close out of the Package Manager.

Now, download the unitypackage from the latest release, and import that into your project. After importing, you should see a HideWithCanvasVRC folder inside of your Assets folder.
Open this folder and you'll be presented with 2 folders and a prefab.

Hierarchy of the Folder:
+ HideWithCanvasVRC
  + Materials
    + Materials used
  + Notifications
    + Notification Sound
  + HideWithCanvasVRC
    + Prefab for setup

## Setting up the Prefab

What we need to do is import the prefab into the scene. Go ahead and drag and drop the prefab into the hierarchy.

**IMPORTANT**

Make sure the Prefab is positioned and rotated at 0, 0, 0, and the scale set to 1, 1, 1! You may encounter issues if you offset the prefab!

Now, right-click the prefab, and select `Unpack Prefab`. You should notice that the prefab turned from the color blue to grey.

Next, expand the prefab in the hierarchy, and you'll see more objects inside. Here's the layout for the children:

+ HideWithCanvasVRC (unpacked prefab)
  + Canvas
    + Panel
      + readme
      + instructions
      + footer
  + PlayerCameraFollower (experimental)
    + NotificationSound
    + NotificationText (WIP)
    + EnabledNotifier (WIP)

**Deleting any of the following objects will break code: Canvas, PlayerCameraFollower, NotificationSound, NotificationText, and EnabledNotifier.**

**Do not change the name of the following objects: HideWithCanvasVRC, Canvas, PlayerCameraFollower, NotificationSound, NotificationText, and EnabledNotifier.**

You may delete anything inside of the Canvas transform; nothing is hard-coded to it.

If you'd like to change the sound, first, import your sound into the Notifications folder, then select the `PlayerCameraFollower/NotificationSound` in the scene, and finally,
drag and drop the sound you imported into the AudioClip variable (at the top of the Audio Source Component).

## Finalizing Changes

Once you've made your changes, navigate into your HideWithCanvasVRC folder, and then delete the prefab in there (it's okay if you lose that prefab, you can always re-import and get it back)
(also make sure you've unpacked the prefab in the scene before deleting it). Next, drag and drop your edited Prefab, from the Scene's Hierarchy, into the folder, and you should see
the GameObject appear in the folder, and your GameObject in the Scene's Hierarchy will turn blue again. Select the GameObject you just saved to the folder, and at the bottom,
you should see it says "AssetBundle" with two dropdowns. Select the first dropdown, and you should see a selection that says `hidewithcanvasvrc`; if you don't then add a new one
and call it that.

## Exporting the Package

Now we need to export the package to an Asset Bundle. Navigate to `Window > AssetBundle Browser`, and open the `Configure` tab. In here, you should see a list of objects on the left.
Select the hidewithcanvasvrc object, and make sure all of your dependencies are there. Once you've verified that they're all listed, navigate to the `Build` tab.

Set your build target to `Standalone Windows 64`, and the output path can be where you'd like, we will need access to this folder for the next step. Hit the `Build` button, and wait for
the build to finish.

## Installing the AssetBundle

Finally, we need to install this AssetBundle.

**NOTE**

For this to work, you need to have run VRChat with the mod installed at least once.

Open the folder with the build, and you should see something with 4 files:

+ **hidewithcanvasvrc**
+ hidewithcanvasvrc.manifest
+ StandaloneWindows64
+ StandaloneWindows64.manifest

We're going to copy the `hidewithcanvasvrc` file. Next, navigate to your `[Steam Folder]\steamapps\common\VRChat\VRChat_Data\StreamingAssets\hidewithcanvasvrc` folder. You should
already see a `hidewithcanvasvrc` file in there, delete that one, and replace it with the one you just built and copied. That's it! When you launch VRChat, your customized canvas
should appear now!

## What Happens if all goes Wrong?

You can delete the `hidewithcanvasvrc` directory or AssetBundle, and the mod will replace it with the old default one. All good to go!
