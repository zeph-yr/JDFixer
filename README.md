# JDFixer

Based on Kylemc1413's NjsFixer

I wanted a stripped down mod that focused only on JD modification to fix floaty maps without NJS/BPM modification since I don't use those features. I felt there was a gap between Njsfixer and Leveltweaks that isn't filled for JD-focused players and this is my interpretation for meeting those needs.

Works in multiplayer, Tournament Assistant, CustomCampaigns. Score submission is unaffected. For Beat Saber 1.16.4.

## New Features
- Selected map's original JD is displayed in the Mod menu. You can easily decide if you want to use JDFixer without having to play the map to feel it. Saves time.
- Selected map's lowest JD allowed by the game is displayed. When it seems like JDFixer "isn't working", check if you exceeded this value.
- Automated JD fixing. The behavior of the Preferences has been changed to selecting the NJS-JD pair that is equal or lower to the selected map's NJS. This allows you to cover large ranges without having to add many values and also handles the rare non-integer NJS
- Added heuristic in the Preferences, where if the selected map's original JD is lower than the JD in the matching NJS-JD pair, the map will run at its original JD. You can toggle this feature.
- Included Upper and Lower NJS Thresholds where Preferences will be ignored: If a map's NJS is at or above the upper threshold, the map will run at its original JD (and vice versa for lower threshold)
- Added Reaction Time display. Reaction time is a function of the map's original NJS and your Jump Distance
- ~~Swapped Enable and Practice Mode buttons because I keep toggling the wrong one lol~~

![screenshot](https://github.com/zeph-yr/JDFixer/blob/master/2.0.3_menu_1_small.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/master/2.0.3_menu_2_small.png)

## How To Use
- Place JDFixer.dll in Plugins folder
- Select a map / difficulty to see its original JD
- Upper and Lower NJS Thresholds can be set in JDFixer.json in UserData folder
- Min and max range in the JD slider can be edited in JDFixer.json
- Preferences for NJS-JD pairs are set in-game
- Enabling "Use Preferred JD Values" will override the JD value in the slider
- Hover over menu in-game for explanations
- **Not compatible with NjsFixer and LevelTweaks** Using with these mods may result in conflicts and unexpected behavior.
- Requires BSIPA, BSML and BS_Utils

## Understanding Preferences Behavior
Suppose your Preferences contain these NJS-JD pairs: 22-18, 21-16, 18-15.

**Example 1:**
Your selected map's NJS is 22 and JD is 20. 
The map will run at 18 JD because there is an exact match for 22 NJS

**Example 2:**
Your selected map's NJS is 21.5 and JD is 20. 
The map will run at 16 JD because 21 NJS is the closest lower match.

**Example 3:**
Your selected map's NJS is 21.5 and JD is 14 *and* "Use Map JD If Lower" is toggled ON.
The map will run at its original 14 JD because it is lower than your matching preference (21-16).
- If "Use Preferred JD Values" is enabled but no Preferences are set, the map will run at JD value in the slider
- Thresholds apply to Preferences only

- If "Use Preferred JD Values" is enabled but no Preferences are set, the map will run at JD value in the slider
- Thresholds apply to Preferences only

## Tournaments and MP
- **Tournament Assistant:** Works with Default, Dual Sync and AutoPause matches. JD slider and Preferences work as normal. Map JD display does not function. Avoid opening the Preferences menu in TA! You will be stuck in it until you relaunch the game. However if you do choose to get yourself stuck inside just before a match, your match will still play fine when the coordinator starts it KEKW.
- **Multiplayer:** JD slider and Preferences work as normal. Map JD display functions only when you select a map from the song browser.
- **CustomCampaigns:** Now Enabled!! Map display and Reaction Time not enabled. Base game campaign also supported.
- **OST Levels:** Now supported!

## About
This is my first time writing a mod. I made it for my own needs but friends thought it useful so I think it would be beneficial to share it. I hope others find this useful.
I welcome feedback and suggestions :) 

## Credits
Thanks Shurdoof for autoupdate!
Thanks Kyle for the original NjsFixer (https://github.com/Kylemc1413/NjsFixer) and thanks to the cool peeps in BSMG discord for the help and advice :)
