using HarmonyLib;
using System.Linq;

namespace JDFixer
{
    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), "Init")]
    internal class SpawnMovementDataUpdatePatch
    {
        public static void Prefix(ref float startNoteJumpMovementSpeed, float startBpm, /*ref float noteJumpStartBeatOffset,*/ ref BeatmapObjectSpawnMovementData.NoteJumpValueType noteJumpValueType, ref float noteJumpValue, ref BeatmapObjectSpawnMovementData __instance, ref bool __state)
        {
            if (PluginConfig.Instance.enabled == false)
            {
                return;
            }

           Logger.log.Debug("Start Map");

            // BS 1.19.0
            noteJumpValueType = BeatmapObjectSpawnMovementData.NoteJumpValueType.BeatOffset;
            float noteJumpStartBeatOffset = noteJumpValue;


            float mapNJS = startNoteJumpMovementSpeed;
            //Logger.log.Debug("mapNJS:" + mapNJS.ToString());

            if (mapNJS <= 0.01) // Just in case?
                mapNJS = 10;

            // JD setpoint from Slider
            //float desiredJumpDis = PluginConfig.Instance.jumpDistance;

            //1.19.1
            float desiredJumpDis;

            if (PluginConfig.Instance.slider_setting == 0)
            {
                desiredJumpDis = PluginConfig.Instance.jumpDistance;
            }
            else
            {
                desiredJumpDis = PluginConfig.Instance.reactionTime * mapNJS / 500;
            }


            // NJS-RT setpoints from Preferences
            if (PluginConfig.Instance.rt_enabled)
            {
                if (mapNJS <= PluginConfig.Instance.lower_threshold || mapNJS >= PluginConfig.Instance.upper_threshold)
                {
                    Logger.log.Debug("Using Threshold");
                    return;
                }

                var rt_pref = PluginConfig.Instance.rt_preferredValues.FirstOrDefault(x => x.njs <= mapNJS);
                Logger.log.Debug("Using Preference");

                if (rt_pref != null)
                    desiredJumpDis = rt_pref.reactionTime * mapNJS / 500;

                if (BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset) <= desiredJumpDis && PluginConfig.Instance.use_heuristic)
                {
                    Logger.log.Debug("Not Fixing: Original JD below or equal setpoint");
                    Logger.log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");
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

            while (mapNJS * numCurr * num2Curr > 17.999)
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

            //Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
            //Logger.log.Debug($"DesiredJumpDis {desiredJumpDis} | Simulated Offset {simOffset}");
        }
    }


    [HarmonyPatch(typeof(MissionSelectionMapViewController), "SongPlayerCrossfadeToLevelAsync")]
    internal class MissionSelectionPatch
    {
        internal static IPreviewBeatmapLevel cc_level = null;

        static void Postfix(IPreviewBeatmapLevel level)
        {
            cc_level = null;

            if (level != null)
            {
                cc_level = level;
            }
        }
    }


    [HarmonyPatch(typeof(CoreMathUtils), "CalculateHalfJumpDurationInBeats")]
    internal class CoreMathPatch
    {
        public static float Postfix(float __result, float startHalfJumpDurationInBeats, float maxHalfJumpDistance, float noteJumpMovementSpeed, float oneBeatDuration, float noteJumpStartBeatOffset)
        {
            if (IPA.Utilities.UnityGame.GameVersion.ToString() == "1.19.1")
            {
                return __result;
            }


            // For 1.19.0 only
            float num = startHalfJumpDurationInBeats;
            float num2 = noteJumpMovementSpeed * oneBeatDuration;
            float num3 = num2 * num;
            maxHalfJumpDistance -= 0.001f;

            while (num3 > maxHalfJumpDistance)
            {
                num /= 2f;
                num3 = num2 * num;
            }
            
            num += noteJumpStartBeatOffset;
            
            if (num < 0.25f)
            {
                num = 0.25f;
            }

            return num;
        }
    }
}