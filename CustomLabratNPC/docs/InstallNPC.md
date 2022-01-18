# CustomLabratNPC

**Installing Custom NPCs**

## Finding your Game's Directory

Open Steam Library, and find SCP Labrat in your Games list. Hit the gear icon, then select `Manage`, and hit `Browse local files`.

![find it lol](https://i.imgur.com/bVlWhvc.png)

## Moving your .lnpc file to where it should go

Copy your .lnpc file (the file that houses all assets for an NPC) and move it to the CustomLabratNPC folder.

## Moving any extra assemblies

Lets say you have DynamicBone.dll or some other Assembly that is required for an avatar, and you need to move it. 
Inside of your CustomLabratNPC folder, there will be another folder named `Libraries`. All assemblies in that folder will be loaded at runtime.

> ***IMPORTANT***
Please know that loading any unknown assemblies is always dangerous. Please keep in mind that SCP Labrat will *never* ask for admin permissions (UAC).

Loading external assemblies require the `loadUnsafeCode` config value to be set to `true`.