# JDFixer

Was once based on Kylemc1413's NjsFixer but has grown to much more.

I wanted a stripped down mod that focused only on JD modification to fix floaty maps without NJS/BPM modification since I don't use those features. I felt there was a gap between Njsfixer and Leveltweaks that isn't filled for JD-focused players and this is my interpretation for meeting those needs.

Supports CustomCampaigns, Tournament Assistant, Multiplayer, OST / DLC / Base Campaign. Score submission is unaffected. For Beat Saber 1.17.1+.

## New Features
- **Selected map's original JD and RT is displayed.** You can easily decide if you want to use JDFixer without having to play the map to feel it. Saves time.
- **Selected map's lowest JD and RT allowed by the game is displayed.** When it seems like JDFixer "isn't working", check if you exceeded this value.
- **Automated JD and RT fixing.** Preferences has been changed to selecting the NJS-JD (or RT) pair that is equal or lower to the selected map's NJS. This allows you to cover large ranges without having to add many values and also handles the rare non-integer NJS. For RT: Automatically sets a map's JD to give your preferred reaction time for a given NJS. Ability to fix constant JD or RT across all maps.
- **Heuristic for Preferences** where if the selected map's original JD is lower than the JD in the matching NJS-JD pair, the map will run at its original JD. You **must** set base game settings to `Dynamic Default` if you enable this feature.
- **Upper and Lower NJS Thresholds where Preferences will be ignored.** If a map's NJS is at or above the upper threshold, the map will run at its original JD and RT (and vice versa for lower threshold)

![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.19/Screenshots/3.0.0_menu_3.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.19/Screenshots/3.1.0_menu_2.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/2.1.0_menu_3.png)

## How To Use
- Place JDFixer.dll in Plugins folder
- Select a map or difficulty to see its original JD and RT
- Set up automated preferences to select JD or RT based on map NJS with in-game menu (If you require finer decimal values for NJS, JD and RT, you can edit your preferences in /UserData/JDFixer.json)
- To access the RT Preferences menu, set `Automated Preferences` to `ReactionTime` and click `JD and RT Preferences`
- Setting `Automated Preferences` to `JumpDistance` or `ReactionTime` will override the JD and RT sliders
- Choose either JD or RT slider to remember its last value **(v3.1.0)**
- Min and max ranges of JD and RT sliders can be changed in /UserData/JDFixer.json
- If you enable `Use Map JD If Lower`, you **must** set base game settings to `Dynamic Default`
- Upper and Lower NJS Thresholds are configured in /UserData/JDFixer.json
- Hover over menu in-game for explanations
- **Not compatible with NjsFixer and LevelTweaks.** Using with these mods may result in conflicts and unexpected behavior.

**v3.0.0+ for BS 1.19.0+ requires BSIPA, BSML, and SiraUtil 3.0.0+**

### Legacy Versions
- v2.1.6- for BS 1.18.3- requires BS_Utils
- v2.1.3+ will import your settings file
- v2.1.0 is not compatible with settings files from previous versions: Delete or rename your old JDFixer.json and allow the mod to generate a new one. Re-enter your settings in-game. If you are knowledgeable, you can copy the relevant data from the old json file to the new one. Just make sure you do it correctly.

## Understanding Preferences Behavior
Suppose your Jump Distance Preferences contain these NJS-JD pairs: 22-18, 21-16, 18-15.

**Example 1:**
Your selected map's NJS is 22 and JD is 20. 
The map will run at 18 JD because there is an exact match for 22 NJS

**Example 2:**
Your selected map's NJS is 21.5 and JD is 20. 
The map will run at 16 JD because 21 NJS is the closest lower match.

**Example 3:**
Your selected map's NJS is 21.5 and JD is 14 *and* `Use Map JD If Lower` is toggled ON.
The map will run at its original 14 JD because it is lower than your matching preference (21-16).

**Example 4:**
Your selected map's NJS is 23 and JD is 20 *and* your `Upper Threshold` is set to 23.
The map will run at its original 20 JD because it triggered the threhold.

**Example 5:**
To run every map at a constant JD regardless of its NJS, create a single preference with 0 NJS and your desired JD (such as 0 NJS - 18 JD)

- If `Automated Preferences` is set to `JumpDistance` but no Preferences are set, the map will run at JD and RT slider value
- Thresholds override Preferences only
- If you need decimal values for Preferences, you can set them in JDFixer.json.

**Reaction Time Preferences:** 
This works exactly the same as JD Preferences. The five examples above apply, except in reaction time. Reaction time is a function of the map's original NJS and Jump Distance. This means that RT Preferences automatically sets a map's JD to give your preferred reaction time for a given NJS.

## Tournaments and MP
- **Tournament Assistant:** Supports Default, Dual Sync and AutoPause matches. You can only use one of the sliders at a time. As usual, enabling Preferences override both sliders. Avoid opening the Preferences menu in TA! You will be stuck in it until you relaunch the game or coordinator lets you out. However if you do choose to get yourself stuck inside just before a match, your match will still play fine when the coordinator starts it KEKW.
- **Multiplayer:** You can only use one of the sliders at a time. As usual, enabling Preferences override both sliders. It is safe to open the Preferences menu here lol.
- **CustomCampaigns:** Supported with map display **(v3.1.0)**
- **OST, DLC Levels, Base Campaign:** Supported.

## UI Option
The default and minimum Reaction Time Display can be hidden.
Change `rt_display_enabled` in /UserData/JDFixer.json to "false"

![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/ui_options.png)

## About
This is my first time writing a mod. I made it for my own needs but friends thought it useful so I think it would be beneficial to share it. I hope others find this useful.
I welcome feedback and suggestions :) 

## Credits
Thanks @Shurdoof for autoupdate!
Thanks Kyle for the original NjsFixer (https://github.com/Kylemc1413/NjsFixer) and thanks to the cool peeps in BSMG discord for the help and advice :)
