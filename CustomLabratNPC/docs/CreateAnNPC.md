# CustomLabratNPC

## Creating an NPC

> **IMPORTANT**
This guide is *unfinished* and will probably, never be finished. 
This guide is to show you how to setup an avatar for CustomLabratNPC. 
This guide **DOES NOT** hold your hand through the process.

> **NOTE**
This guide will discuss Animating and Scripting, however, for certain scenarios, only Animating may be required.

### Setting up

First, you'll need to install Unity. As of 17/01/2022, SCP Labrat uses Unity 2019.4.20f1. This is the Unity Editor you should be using to create NPCs.

You can grab Unity 2019.4.20f1 here: https://unity3d.com/unity/whats-new/2019.4.21

### Creating your first project

After installing 2019.4.20f1, create a new, empty 3D project. 
Once you're in the new scene, grab the latest release of CustomLabratNPC-Editor.dll 
and drag-and-drop it anywhere in your Assets folder. 

### Getting used to the grind

Everything you need for building is withing the CustomLabratNPC tab at the top of your unity editor.

### Editor Windows Explained

**DynamicBone Bundler**

Bundles DynamicBone into an assembly to be loaded as a Library for avatars that use DynamicBones.

This can be done through the ScriptCompiler, however this is a tool designed to be used with the DynamicBone unitypackage.

**Builder**

The main builder. This is how you bundle your NPC into an AssetBundle (ending in .lnpc) that will be loaded into the game.

**Info**

Just basic info and quick links.

**ScriptCompiler**

Will compile any user made scripts into an assembly to be loaded from Libraries. 
If you chose to create scripting with your NPC, this is how you will load code into the game.

### Setting up an NPC

Once you have your model imported and setup properly, you can go ahead and select the GameObject of the model and add the `CustomNPCDescriptor` component.

### CustomNPCDescriptor Values

**NPCType**

Defines what type of NPC to create. 
An NPC Type value other than Unknown should be seen as a preset/template for the target SCP.
An Unknown NPC Type will rely on the user to *code in their own handling*; Unknown NPCs have no handling and will just import the AssetBundle and Libraries.

**NPCDisplayName**

A display name for your NPC. If left blank at build, this will default to your GameObject's name.

**NPCIcon**

(Currently Unused)

The icon associated with your NPC. Used for UI (not implemented yet).

**OnSCPEvent**

Invokations of any associated NPCEvent handlers. Used for custom scripting.

**Animator Events**

List of AnimatorControllers that handle Animator Parameters/Layers.

### Animator Parameters

| Parameter Name  | Parameter Type | Description                                  |
|-----------------|----------------|----------------------------------------------|
| Created         | Trigger        | Triggers when the avatar is Enabled          |
| Destroyed       | Trigger        | Triggers when the AVatar is Destroyed        |
| LocalPlayerDead | bool           | Returns whether the player is dead or not    |
| IsMoving        | bool           | Returns while the NPC's position is changing |

### How to Build your NPC

1) Open the Builder window under `CustomLabratNPC > Builder`
2) Select the GameObject that you wish to publish
3) Set a Creator Name
4) Select the Output Directory for your NPC
5) Set any Build Options (see below)
6) Hit Build

### Build Options

**Allow Overwriting of Files**

Overwrites the AssetBundle in the Output Directory, if the AssetBundle already exists

**Reset All AssetBundles and Prefabs**

Ensures that the Build process is a fresh build by deleting all previous builds.

### Bundling DynamicBone

After importing DynamicBone, if required by an avatar, or just wanted, you will need to bundle DynamicBone into an Assembly to be loaded at runtime.

1) Open the DynamicBone Bundler window under `CustomLabratNPC > DynamicBundler Bundler`
2) Import DynamicBone if it isn't already
3) Set your DynamicBone Location
  + This should be set to the DynamicBone folder in the Assets root. Please don't select any folder *inside* of DynamicBone
4) Hit Build DynamicBone to Assembly

The output Assembly will be located in `Assets/CustomLabratNPC/DynamicBone/DynamicBone.dll`, the pdb is not required.

Move this file to your Libraries location.

### Compiling Scripts

Compiles all your custom scripts into one Assembly to be loaded at runtime.

1) Open the ScriptCompiler window under `CustomLabratNPC > ScriptCompiler`
2) Drag-and-drop the script you need to compile to the dropbox, and hit the add box. Repeat for any required scripts.
  + Please note that these scripts need to be attached to your avatar (must be referenced in the AssetBundle)
3) Set an Assembly Name. Please make sure that this is a *file friendly* name

The Assembly will output at `Assets/CustomLabratNPC/ScriptCompiler/{AssemblyName}.dll`

Move this file to your Libraries location.