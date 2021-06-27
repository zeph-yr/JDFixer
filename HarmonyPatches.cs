using HarmonyLib;
using System.Linq;

namespace JDFixer
{

    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), "Init")]
    internal class SpawnMovementDataUpdatePatch
    {
        public static void Prefix(ref float startNoteJumpMovementSpeed, float startBpm, ref float noteJumpStartBeatOffset, ref BeatmapObjectSpawnMovementData __instance, ref bool __state)
        {
            bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel 
                && Config.UserConfig.enabled && (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) && (Config.UserConfig.enabledInPractice || BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings == null);
            __state = WillOverride;
            if (!WillOverride) return;


            float mapNJS = startNoteJumpMovementSpeed;

            // JD setpoint from Slider
            float desiredJumpDis = Config.UserConfig.jumpDistance;


            // NJS-JD setpoints from Preferences
            if (Config.UserConfig.usePreferredJumpDistanceValues)
            {
                // For Acc and Speed Maps:
                if (mapNJS <= Config.UserConfig.lower_threshold || mapNJS >= Config.UserConfig.upper_threshold)
                {
                    return;
                }

                else
                {
                    var pref = Config.UserConfig.preferredValues.FirstOrDefault(x => x.njs <= mapNJS);
                    //Logger.log.Debug("Using Preference");

                    if (pref != null)
                        desiredJumpDis = pref.jumpDistance;
                }
                

                // Heuristic: If map's original JD is less than the matching preference entry, play map at original JD
                // Rationale: I created this mod because I don't like floaty maps. If the original JD chosen by the
                // mapper is lower than my pick, it's probably more optimal than my pick.
                if (BeatmapInfo.Selected.JumpDistance <= desiredJumpDis && Config.UserConfig.use_heuristic)
                {
                    //Logger.log.Debug("Not Fixing: Original JD below or equal setpoint");
                    //Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
                    return;
                }
            }

            // Calculate New Offset Given Desired JD:
            float simOffset = 0;
            float numCurr = 60f / startBpm;
            float num2Curr = 4f;

            while (mapNJS * numCurr * num2Curr > 18f)
                num2Curr /= 2f;

            if (num2Curr < 1f)
                num2Curr = 1f;

            float jumpDurCurr = num2Curr * numCurr * 2f;
            float jumpDisCurr = mapNJS * jumpDurCurr;

            float desiredJumpDur = desiredJumpDis / mapNJS;
            float desiredHalfJumpDur = desiredJumpDur / 2f / num2Curr;
            float jumpDurMul = desiredJumpDur / jumpDurCurr;

            simOffset = (num2Curr * jumpDurMul) - num2Curr;
            
            noteJumpStartBeatOffset = simOffset;

            //Logger.log.Debug("Fixing");
            //Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
            //Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
        }

        public static void Postfix(ref float ____jumpDistance, bool __state)
        {
            if(__state)
                Logger.log.Debug("Final Jump Distance: " + ____jumpDistance);
        }
    }
}
