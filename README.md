# JDFixer

Was once based on Kylemc1413's NjsFixer but now has grown to much more.

I wanted a stripped down mod that focused only on JD modification to fix floaty maps without NJS/BPM modification since I don't use those features. I felt there was a gap between Njsfixer and Leveltweaks that isn't filled for JD-focused players and this is my interpretation for meeting those needs.

Supports CustomCampaigns, Tournament Assistant, Multiplayer, OST. Score submission is unaffected. For Beat Saber 1.17.1+.

## New Features
- Selected map's original JD is displayed in the Mod menu. You can easily decide if you want to use JDFixer without having to play the map to feel it. Saves time.
- Selected map's lowest JD allowed by the game is displayed. When it seems like JDFixer "isn't working", check if you exceeded this value.
- Automated JD fixing. The behavior of the Preferences has been changed to selecting the NJS-JD pair that is equal or lower to the selected map's NJS. This allows you to cover large ranges without having to add many values and also handles the rare non-integer NJS
- Added heuristic in the Preferences, where if the selected map's original JD is lower than the JD in the matching NJS-JD pair, the map will run at its original JD. You can toggle this feature.
- Included Upper and Lower NJS Thresholds where Preferences will be ignored: If a map's NJS is at or above the upper threshold, the map will run at its original JD (and vice versa for lower threshold)
- Added Reaction Time display. Reaction time is a function of the map's original NJS and your Jump Distance.
- Added Reaction Time Preferences. Automatically sets a map's JD to give you your preferred reaction time for a given NJS.
- ~~Swapped Enable and Practice Mode buttons because I keep toggling the wrong one lol~~

![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/menu_2.1.2_1_small.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/2.1.0_menu_2.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/2.1.0_menu_3.png)

## How To Use
- Place JDFixer.dll in Plugins folder
- Select a map / difficulty to see its original JD
- Upper and Lower NJS Thresholds are set in JDFixer.json in UserData folder
- Min and max range of JD and RT sliders can be set in JDFixer.json
- Preferences for NJS-JD and NJS-RT pairs are set in-game. Decimal JD and RT values can be set in JDFixer.json
- To access the RT Preferences menu, toggle ON `Use RT Preferences` and click `JD and RT Preferences`
- Enabling `Use JD Preferences` or `Use RT Preferences` will override the JD value in the slider
- Enabling `Use RT Preferences` will override JD Preferences even if `Use JD Preferences` is turned on.
- Hover over menu in-game for explanations
- **Not compatible with NjsFixer and LevelTweaks.** Using with these mods may result in conflicts and unexpected behavior.
- Requires BSIPA, BSML and BS_Utils

**v2.1.3+ will import your settings file**

**v2.1.0 is not compatible with settings files from previous versions:**
Please allow the mod to generate a new JDFixer.json and re-enter your settings in-game. If you are knowledgeable, you can copy the relevant data from the old .json to the new one. Just make sure you do it correctly.

## Understanding Preferences Behavior
Suppose your Preferences contain these NJS-JD pairs: 22-18, 21-16, 18-15.

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

- If `Use JD Preferences` is enabled but no Preferences are set, the map will run at JD value in the slider
- Thresholds override Preferences only
- If you need decimal values for Preferences, you can set them in JDFixer.json.

**Reaction Time Preferences:** 
This works exactly the same as the JD Preferences. The four examples above apply, except in reaction time.
Enabling `Use Reaction Time Preferences` will override JD Preferences even if `Use JD Preferences` is turned on.

## Tournaments and MP
- **Tournament Assistant:** Works with Default, Dual Sync and AutoPause matches. JD slider and Preferences work as normal. Map JD display does not function. Avoid opening the Preferences menu in TA! You will be stuck in it until you relaunch the game. However if you do choose to get yourself stuck inside just before a match, your match will still play fine when the coordinator starts it KEKW.
- **Multiplayer:** JD slider and Preferences work as normal. Map JD display functions only when you select a map from the song browser.
- **CustomCampaigns:** Now Enabled!! Map display and Reaction Time is disabled. Slider and Preferences work as normal. Base game campaign also supported.
- **OST Levels:** Now supported!

## UI Option
The default and minimum Reaction Time Display can be hidden to revert to the previous version's UI.
Set `rt_display_enabled` in JDFixer.json to "false"

![screenshot](https://github.com/zeph-yr/JDFixer/blob/BS_1.16.4_MA_v2.0.3/Screenshots/ui_options.png)

## About
This is my first time writing a mod. I made it for my own needs but friends thought it useful so I think it would be beneficial to share it. I hope others find this useful.
I welcome feedback and suggestions :) 

## Credits
Thanks @Shurdoof for autoupdate!
Thanks Kyle for the original NjsFixer (https://github.com/Kylemc1413/NjsFixer) and thanks to the cool peeps in BSMG discord for the help and advice :)
