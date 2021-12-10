using HarmonyLib;
using System.Linq;

namespace JDFixer
{
    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), "Init")]
    internal class SpawnMovementDataUpdatePatch
    {
        public static void Prefix(ref float startNoteJumpMovementSpeed, float startBpm, /*ref float noteJumpStartBeatOffset,*/ ref BeatmapObjectSpawnMovementData.NoteJumpValueType noteJumpValueType, ref float noteJumpValue, ref BeatmapObjectSpawnMovementData __instance, ref bool __state)
        {
            /*bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && PluginConfig.Instance.enabled
                && (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard
                || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer
                || BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Mission);*/

            //BS_Utils.Utilities.LevelType.Tutorial
            //BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings != null

            //__state = WillOverride;

            //if (!WillOverride)
            //    return;

            if (PluginConfig.Instance.enabled == false)
            {
                return;
            }

           Logger.log.Debug("Start Map");

            // BS 1.19.0
            noteJumpValueType = BeatmapObjectSpawnMovementData.NoteJumpValueType.BeatOffset;
            float noteJumpStartBeatOffset = noteJumpValue;


            float mapNJS = startNoteJumpMovementSpeed;
            Logger.log.Debug("mapNJS:" + mapNJS.ToString());

            if (mapNJS <= 0.01) // Just in case?
                mapNJS = 10;

            // JD setpoint from Slider
            float desiredJumpDis = PluginConfig.Instance.jumpDistance;


            // NJS-RT setpoints from Preferences
            if (PluginConfig.Instance.rt_enabled)
            {
                if (mapNJS <= PluginConfig.Instance.lower_threshold || mapNJS >= PluginConfig.Instance.upper_threshold)
                {
                    return;
                }

                var rt_pref = PluginConfig.Instance.rt_preferredValues.FirstOrDefault(x => x.njs <= mapNJS);

                if (rt_pref != null)
                    desiredJumpDis = rt_pref.reactionTime * mapNJS / 500;

                if (BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset) <= desiredJumpDis && PluginConfig.Instance.use_heuristic)
                {
                    return;
                }
            }

            // NJS-JD setpoints from Preferences
            else if (PluginConfig.Instance.usePreferredJumpDistanceValues)
            {
                // For Acc and Speed Maps:
                if (mapNJS <= PluginConfig.Instance.lower_threshold || mapNJS >= PluginConfig.Instance.upper_threshold)
                {
                    Logger.log.Debug("Using Threshold");
                    return;
                }

                var pref = PluginConfig.Instance.preferredValues.FirstOrDefault(x => x.njs <= mapNJS);
                Logger.log.Debug("Using Preference");

                if (pref != null)
                    desiredJumpDis = pref.jumpDistance;

                // Heuristic: If map's original JD is less than the matching preference entry, play map at original JD
                // Rationale: I created this mod because I don't like floaty maps. If the original JD chosen by the
                // mapper is lower than my pick, it's probably more optimal than my pick.
                if (BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset) <= desiredJumpDis && PluginConfig.Instance.use_heuristic)
                {
                    Logger.log.Debug("Not Fixing: Original JD below or equal setpoint");
                    Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
                    return;
                }
            }

            // Calculate New Offset Given Desired JD:
            //Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");

            float simOffset = 0;
            float numCurr = 60f / startBpm;
            float num2Curr = 4f;

            while (mapNJS * numCurr * num2Curr > 18f)
                num2Curr /= 2f;

            if (num2Curr < 0.25f)
                num2Curr = 0.25f;

            float jumpDurCurr = num2Curr * numCurr * 2f;
            float jumpDisCurr = mapNJS * jumpDurCurr;

            float desiredJumpDur = desiredJumpDis / mapNJS;
            float desiredHalfJumpDur = desiredJumpDur / 2f / num2Curr;
            float jumpDurMul = desiredJumpDur / jumpDurCurr;

            simOffset = (num2Curr * jumpDurMul) - num2Curr;

            // BS 1.19.0
            noteJumpValue = simOffset;
            //noteJumpStartBeatOffset = simOffset;


            Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
        }

        /*public static void Postfix(ref float ____jumpDistance, bool __state)
        {
            if (__state)
                Logger.log.Debug("Final Jump Distance: " + ____jumpDistance);
        }*/
    }


    // Note: Patching DidActivate works only when diff is clicked
    //[HarmonyPatch(typeof(StandardLevelDetailViewController), "DidActivate")]

    /*[HarmonyPatch(typeof(StandardLevelDetailViewController), MethodType.Constructor)]
    internal class StandardLevelDetailViewControllerPatch
    {
        public static void Postfix(ref StandardLevelDetailViewController __instance)
        {
            //Plugin.leveldetail = __instance;
            JDFixer.Managers.JDFixerUIManager.levelDetail = __instance;
            //Logger.log.Debug("leveldetail found");
        }
    }


    //[HarmonyPatch(typeof(MissionSelectionMapViewController), "DidActivate")]
    [HarmonyPatch(typeof(MissionSelectionMapViewController), MethodType.Constructor)]
    internal class MissionSelectionMapViewControllerPatch
    {
        public static void Postfix(ref MissionSelectionMapViewController __instance)
        {
            Plugin.missionselection = __instance;
            //Logger.log.Debug("missionselection found");
        }
    }*/
}