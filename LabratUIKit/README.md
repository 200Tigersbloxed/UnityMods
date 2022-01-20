**LabratUIKit**

Easily add in-game UI functionality to SCP Labrat

## Mod Loader

[MelonLoader v0.5.2](https://github.com/LavaGang/MelonLoader/releases/tag/v0.5.2)

## Installing

1) Install MelonLoader to SCP Labrat
2) Move `LabratUIKit.dll` to `[SCP Labrat Game Folder]/Mods`

Latest Release: https://github.com/200Tigersbloxed/UnityMods/releases/tag/luk-v1.0.0

## Documentation

### Creating a ModMenu

A ModMenu is what a mod creates to tell the framework to create a dedicated UI section for it. This is required to be setup.

```cs
ModMenu modMenu = new ModMenu("ModName", "ModVersion", "ModCreator");
```

The line above will setup our ModMenu. Now we'll set some values for it.

**List<ModContent> modContent**

A list containing all ModContents. (explained below)

## Creating ModContent

A ModContent is a panel inside your ModMenu that can display information, and invoke events.

```cs
private class HelloWorld : ModContent
{
    public override ModContentType ModContentType {get; set;} = ModContentType.Invoke;
    public override string Title {get; set;} = "Hello,";
    public override string Description {get; set;} = "World!";
    public override bool HideInvokeButton => true;
    public override bool ExtendModContentText => true;
}
```

The example above will create a new ModContent, with an Invoke value, that hides the Invoke button.
We're hiding the Invoke Button as we aren't using it. Note that Hiding the Invoke button will do nothing if the ModContentType is Value.

### ModContent Overrides

**ModContentType**

Describes the type of ModContent to create. 
An Invoke will have one button, while a Value will return a Value after an Invoke.

**Title**

Title of the ModContent.

**Description**

Description of the ModContent.

**Image**

An Image to display to the left of the ModContent.

**OnModContentButtonPressed()**

Invokes whenever either an Invoke or Value button is pressed.

**ModValue**

(Value type only) returns the value that was set after a Value invoke.

**HideInvokeButton**

(Invoke type only) Hides the Invoke button.

**ExtendModContentText**

(Invoke type only; HideInvokeButton required) Extends the Title/Description.

**InvokeButtonText**

Sets the button text for the Invoke button

**OnBeingCreated()**

Invoked before the ModContent is created. Used for dynamically setting values.

### ModContent Methods/Values (not overridable)

**IsModContentVisible**

Returns whether or not the ModContent is created.

**ModContent_Container**

Returns the GameObject for the ModContent.

**RequestTextChange()**

Refreshes all text on screen. Does not refresh Image.

## Applying ModContent

After you create ModContent, you need to add it to your ModMenu.

```cs
private class HelloWorld : ModContent
{
    public override ModContentType ModContentType {get; set;} = ModContentType.Invoke;
    public override string {get; set;} = "Hello,";
    public override string Description {get; set;} = "World!";
    public override bool HideInvokeButton => true;
    public override bool ExtendModContentText => true;
}

ModMenu modMenu = new ModMenu("ModName", "ModVersion", "ModCreator");
modMenu.modContent = new List<ModContent>
{
    new HelloWorld()
};
```

Using the two examples above, you can add your ModContent to your ModMenu, and now, you're ready to add your ModMenu!

## Adding the ModMenu

```cs
private class HelloWorld : ModContent
{
    public override ModContentType ModContentType {get; set;} = ModContentType.Invoke;
    public override string {get; set;} = "Hello,";
    public override string Description {get; set;} = "World!";
    public override bool HideInvokeButton => true;
    public override bool ExtendModContentText => true;
}

ModMenu modMenu = new ModMenu("ModName", "ModVersion", "ModCreator");
modMenu.modContent = new List<ModContent>
{
    new HelloWorld()
};
UIKitAPI.AddModToMenu(modMenu);
```

This will now push your ModMenu to the framework, and when you open your Mod screen in SCP Labrat, you should now have a button to open it!

## Using Reflection

Step 1) Create any ModContents you need with `UIKitAPI.CreateModContent`
  + Add all these **objects** to a List<object>
Step 2) `UIKitAPI.CreateModMenu()`

done.
