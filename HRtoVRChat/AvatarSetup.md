# HRtoVRChat Avatar Setup

The HRtoVRChat-Prefab (which can be found on the Latest Release Artifacts for HRtoVRChat) contains contents to get started with HRtoVRChat. Please read through this **whole document** for information on everything included.

**ATTENTION**

This guide assumes that you have *basic Animation/Animator Controller* knowledge in Unity. Please see the following guide for learning Animation and ANimator Controllers in Unity:

https://learn.unity.com/course/introduction-to-3d-animation-systems

https://learn.unity.com/tutorial/animator-controllers-2019-3

https://learn.unity.com/tutorial/controlling-animation

## HRContainer

The Example HRContainer for an avatar. This was made to work when put inside the avatar's chest bone, if you move it somewhere else, you may have to redo the animations provided. (30 animations/1 layers per 10 animations/3 layers in all)

Be sure that you are also using the provided FX Layer, or have implemented the Layers (excluding Base Layer) into your current FX Controller.

## hrEM

The Expression Menu for the avatar. This is not required, if you'd like to replace this, you can.

## hrEP

The Expression Parameters are required for the avatar. If you'd like to use your parameters, below is a table of the required parameters

| Parameter Name | Parameter Type | Parameter Default | Parameter Saved |
|----------------|----------------|-------------------|-----------------|
| onesHR         | `int`          | 0                 | false           |
| tensHR         | `int`          | 0                 | false           |
| hundredsHR     | `int`          | 0                 | false           |

## hr/HRTestingMaterial

Texture and Material for the background. Can be replaced.

## numbers

### anims

Includes all the animations for changing the numbers. If you move the HRContainer Prefab outside of the avatar's Chest, you will need to redo all of these.

If you need to redo this, then for each animation for all number spots (ones, tens, and hundreds) do the following:
1) Open the animation in the Animation window
2) Clear all the keys
3) Hit the record button
4) Replace the Material of the number spot with the number in the animation-name
  + For example: If the animation name is `ones-one`, you will set the material to the `one` material (found in the materials folder)
5) Stop recording
6) Copy the keyframe to 1 frame after
7) Repeat for all other animations

This also includes the FX Layer for setting the animations. There is 1 layer for each numbered spot (ones, tens, and hundreds). Here's how each layer works:

1) The layer starts with all the animations for the number spot.
2) The Any State is connected to all of the Animations
  + The Conditions for the Any State are `if (numberspot)HR Equals (animation number [example: ones-one == 1]) -> animation`
3) The Animations are connected to the Exit
  + The Conditions for the Exit are `if (numberspot)HR NotEqual (animation number [example: ones-one != 1]) -> exit`
4) Repeat for all other Layers

### materials

Contains the materials for the numbers. These can be changed to whatever shader you'd like but were tested with
PC: XSToon3
Quest: Toon Lit

### 1rowspritesheet

The texture/sprite sheet for the materials. Cropped with Tile Size and offset with Offset.

### number

An example number. You probably won't need to touch this.
