# JDFixer

Based on Kylemc1413's NjsFixer

I wanted a stripped down mod that focused only on JD modification to fix floaty maps without NJS/BPM modification since I don't use those features. I felt there was a gap between Njsfixer and Leveltweaks that isn't filled for JD-focused players and this is my interpretation for meeting those needs.

Works in multiplayer. Score submission is unaffected. For Beat Saber 1.16.1.

## New Features
- Selected map's original JD is displayed in the Mod menu. You can easily decide if you want to use the JDFixer without having to play the map to feel it. Saves time.
- Automated JD fixing. The behavior of the Preferences has been changed to selecting the NJS-JD pair that is equal or lower to the selected map's NJS. This allows you to cover large ranges without having to add many values and also handles the rare non-integer NJS
- Added heuristic in the Preferences, where if the selected map's original JD is lower than the JD in the matching NJS-JD pair, the map will run at its original JD. If you don't like this feature, you can still use the slider to force JD as much as possible
- Included Upper and Lower NJS Thresholds where Preferences will be ignored: If a map's NJS is at or above the upper threshold, the map will run at its original JD (and vice versa for lower threshold)
- Swapped Enable and Practice Mode buttons because I keep toggling the wrong one lol

![screenshot](https://github.com/zeph-yr/JDFixer/blob/master/menu1_small.png)
![screenshot](https://github.com/zeph-yr/JDFixer/blob/master/menu2_small.png)

## How To Use
- Place JDFixer.dll in Plugins folder
- To see the current map difficulty's JD, click the Map JD button to refresh (it does not update automatically)
- Upper and Lower NJS Thresholds can be set in JDFixer.json in UserData folder
- Min and max range in the JD slider can be edited in JDFixer.json
- Preferences for NJS-JD pairs are set in-game
- Enabling "Use Preferred JD Values" will override the JD value in the slider
- Hover over menu in-game for explanations
- **Not compatible with NjsFixer.** Using both mods may result in conflicts and unexpected behavior
- Requires BSIPA, BSML and BS_Utils

## Understanding The Preferences Behavior
(to be updated with example)

## About
This is my first time writing a mod. I made it for my own needs but friends thought it useful so I think it would be beneficial to share it. I hope others find this useful.
I welcome feedback and suggestions :) 

## Credits
Thanks Kyle for the original NjsFixer (https://github.com/Kylemc1413/NjsFixer) and thanks to the cool peeps in BSMG discord for the help and advice :)
