# HRtoVRChat

*Send your HeartRate into VRChat as a Parameter*

> ___
> # 🛑 PLEASE READ THIS 🛑
> ## HRtoVRChat is now entering it's **deprecated** stages, and will soon become **obsolete**!
> All new releases will no longer come with build artifacts; users will be required to build themselves, and new releases will only occur for **game breaking changes**. Releases from here on out will no longer be updated to the VRCMG (so you can no longer update via. MelonAssistant). Please consider upgrading to [HRtoVRChat_OSC](https://github.com/200Tigersbloxed/HRtoVRChat_OSC)
> ___

## Info

HRtoVRChat allows you to send your Heart Rate into VRChat via. Expression Parameters. HRtoVRChat only uses 3 int parameters, as to not require to use a bunch of storage. (see parameters section)

## Installing

1) Download and Install the latest release of [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) to VRChat (see warning section)
2) Run VRChat at least once to generate required files, directories, and assemblies
3) Download the latest release of HRtoVRChat, and move the dll to the Mods folder. (Don't download source code!)
4) Run VRChat again (this will generate the config for HRtoVRChat)
5) Done! Please continue to the Config section

## Config

After installing, you then have to tell HRtoVRChat which service you'll be using to get your heart rate. Below is a table of which Heart Rate monitors we support, if you'd like to request support, please use the [Discussions Tab](https://github.com/200Tigersbloxed/UnityMods/discussions) to do so.

Please also consider contributing to add more support with other Heart Rate Monitors

| Device          | HRType          | Info                                                                     |
|-----------------|-----------------|--------------------------------------------------------------------------|
| FitbitHRtoWS    | `fitbithrtows`  | https://github.com/200Tigersbloxed/FitbitHRtoWS                          |
| HypeRate        | `hyperate`      | https://www.hyperate.io/                                                 |
| Pulsoid/Stromno | `pulsoid`       | https://pulsoid.net/ https://www.stromno.com/                            |
| PulsoidSocket   | `pulsoidsocket` | https://github.com/pulsoid-oss/pulsoid-api#read_heart_rate_via_websocket |
| TextFile        | `textfile`      | A .txt file containing only a number                                     |
| Omnicept        | `omnicept`      | https://www.hp.com/us-en/vr/reverb-g2-vr-headset-omnicept-edition.html   |
| SDK             | `sdk`           | https://github.com/200Tigersbloxed/HRtoVRChat_OSC/blob/main/SDK.md       |

**[For a full list of all the supported devices, please click here](https://www.fortnite.lol/projects/hrtovrchat#h.mlz9fz56ioey)**

Take note of HRType, as you'll need to know which you you have to put in the `hrtype` config value.

### Values

Below is a table of all the config values and a description. Please update the config accordingly.

| Config Value        | Default Value             | Description                                                                             |
|---------------------|---------------------------|-----------------------------------------------------------------------------------------|
| `hrtype`            | `(string)` unknown        | The type of service where to get Heart Rate data from.                                  |
| `fitbiturl`         | `(string)` `String.Empty` | (FitbitHRtoWS Only) The WebSocket URL to connect to.                                    |
| `hyperatesessionid` | `(string)` `String.Empty` | (HypeRate Only) The HypeRate SessionId to subscribe to.                                 |
| `pulsoidwidget`     | `(string)` `String.Empty` | (Pulsoid Only) The URL to GET from an API.                                              |
| `pulsoidkey`        | `(string)` `String.Empty` | (PulsoidSocket Only) API Key for Pulsoid's Sockets.                                     |
| `textfilelocation`  | `(string)` `String.Empty` | (TextFile Only) Location of the text file where HR data should be read from             |
| `ShowDebug`         | `(bool)` `false`          | Shows Debug logs for debugging HRtoVRChat.                                              |
| `MaxHR`             | `(double)` 150            | Maximum range for the `HRPercent (float)` parameter                                     |
| `MinHR`             | `(double)` 0              | Minimum range for the `HRPercent (float)` parameter                                     |
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

## Migration Guide for v1.5.0 (Pulsoid only)

While both HypeRate and Pulsoid were affected by their respective API changes, only Pulsoid needs to migrate. First, the old Pulsoid/API config value is **removed**. Now, there's two ways to migrate.

**Method 1** - Official Sockets *(Not Recommended)*

If you can figure out how to get an `access_token` from Pulsoid, then set the `pulsoidkey` config value to that token, and set `hrtype` to `pulsoidsocket`.
https://github.com/pulsoid-oss/pulsoid-api#read_heart_rate_via_websocket

**Method 2** - Third-Party Sockets

Set `hrtype` to `pulsoid` and set `pulsoidwidget` to your Pulsoid widgetId.

To get your widgetId, go to your Pulsoid dashboard, go to the Widgets tab, hit Configure on any widget (I recommend the default one), and copy the long string of characters after the last slash in the URL (red/highlighted in attached image)

![widgetId](https://cdn.discordapp.com/attachments/887159486677151814/937249892995326012/unknown.png)

# ATTENTION

It is against the VRChat ToS to modify your client in any way, shape, or form. By using this mod, you understand that your account may be suspended or deleted!

So please don't complain to me when you get banned for having your Heart Rate shown in a public instance, because you knew it was against the ToS to mod.

# Latest Release

Click [here](https://github.com/200Tigersbloxed/UnityMods/releases/tag/hrtvrc-v1.5.1) for the latest release

# Extra Info

+ Library [ParamLib](https://github.com/benaclejames/ParamLib) is used to update parameters on an avatar. Code is kept up to date with latest push; no changes to code are made.
