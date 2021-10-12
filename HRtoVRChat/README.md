# HRtoVRChat

*Send your HeartRate into VRChat as a Parameter*

## Info

HRtoVRChat allows you to send your Heart Rate into VRChat via. Expression Parameters. HRtoVRChat only uses 3 int parameters, as to not require to use a bunch of storage. (see parameters section)

## Dependencies

Please make sure all the mods in the list are installed before continuing.

+ [VRCUtilityKit](https://github.com/loukylor/VRC-Mods#vrchatutilitykit)
  + Also found on [VRCMelonAssistant](https://github.com/knah/VRCMelonAssistant)

## Installing

1) Download and Install the latest release of [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) to VRChat (see warning section)
2) Run VRChat at least once to generate required files, directories, and assemblies
3) Download the latest release of HRtoVRChat, and move the dll to the Mods folder. (Don't download source code!)
4) Run VRChat again (this will generate the config for HRtoVRChat)
5) Done! Please continue to the Config section

## Config

After installing, you then have to tell HRtoVRChat which service you'll be using to get your heart rate. Below is a table of which Heart Rate monitors we support, if you'd like to request support, please use the [Discussions Tab](https://github.com/200Tigersbloxed/UnityMods/discussions) to do so.

Please also consider contributing to add more support with other Heart Rate Monitors

| Device       | HRType         | Info                                            |
|--------------|----------------|-------------------------------------------------|
| FitbitHRtoWS | `fitbithrtows` | https://github.com/200Tigersbloxed/FitbitHRtoWS |
| HypeRate     | `hyperate`     | https://github.com/200Tigersbloxed/HypeRate.NET |
| Pulsoid/API  | `pulsoid`      | https://pulsoid.net/                            |
| TextFile     | `textfile`     | just a text file that's saved on your pc lol    |

Take note of HRType, as you'll need to know which you you have to put in the `hrtype` config value.

### Values

Below is a table of all the config values and a description. Please update the config accordingly.

| Config Value        | Default Value             | Description                                                                             |
|---------------------|---------------------------|-----------------------------------------------------------------------------------------|
| `hrtype`            | `(string)` unknown        | The type of service where to get Heart Rate data from.                                  |
| `fitbiturl`         | `(string)` `String.Empty` | (FitbitHRtoWS Only) The WebSocket URL to connect to.                                    |
| `hyperatesessionid` | `(string)` `String.Empty` | (HypeRate Only) The HypeRate SessionId to subscribe to.                                 |
| `pulsoidfeed`       | `(string)` `String.Empty` | (Pulsoid/API Only) The URL to GET from an API. (Designed for Pulsoid)                   |
| `textfilelocation`  | `(string)` `String.Empty` | (TextFile Only) Location of the text file where HR data should be read from             |
| `ShowDebug`         | `(bool)` `false`          | Shows Debug logs for debugging HRtoVRChat.                                              |
| `UIXSupprt`         | `(bool)` `true`           | Supports the [UserInfoExtensions](https://github.com/knah/VRCMods#ui-expansion-kit) Mod |
| `AMAPISupport`      | `(bool)` `true`           | Supports the [ActionMenuApi](https://github.com/gompocp/ActionMenuApi) Mod              |

### Editing the Config

The config file can be found at `[Steam Install]/common/VRChat/UserData/MelonPreferences.cfg` and is in a [TOML format](https://toml.io/en/).

Or, if you have [UIExpansionKit](https://github.com/knah/VRCMods#ui-expansion-kit), you can edit the mod settings directly in-game by:

1) Open your QuickMenu
2) Go to settings
3) Hit `Mod Settings` in the bottom-right
4) Scroll down to the `HRtoVRChat` section

## Avatar Setup

Yes, avatar specific setup is required to use this mod properly. Please see the [Avatar Setup Guide](https://github.com/200Tigersbloxed/UnityMods/blob/main/HRtoVRChat/AvatarSetup.md) (still a WIP) for more information.

If you'd like to test with a public avatar, use [emmVRC](https://thetrueyoshifan.com/mods/emmvrc/)'s avatar search, and search for `HRVRC`, or if you have a way to apply an avatar from an avatarId, the avatar's Id is `avtr_459421d6-d409-40e4-9c7f-218869e4e984`.

# ATTENTION

It is against the VRChat ToS to modify your client in any way, shape, or form. By using this mod, you understand that your account may be suspended or deleted!

So please don't complain to me when you get banned for having your Heart Rate shown in a public instance, because you knew it was against the ToS to mod.

# Latest Release

Click [here](https://github.com/200Tigersbloxed/UnityMods/releases/tag/hrtvrc-v1.2.0) for the latest release

# Extra Info

+ Library [ParamLib](https://github.com/benaclejames/ParamLib) is used to update parameters on an avatar. Code is kept up to date with latest push; no changes to code are made.
+ Library [VRChatUtilityKit](https://github.com/loukylor/VRC-Mods#vrchatutilitykit) is used to update parameters on Local Avatar Update (changed avatars); no changes to code are made.
