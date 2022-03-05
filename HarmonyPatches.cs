using HarmonyLib;
using System;
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

            // Bit of an issue... to calculate the map's original JD for the heuristic, we need the map's offset
            // This was fine when Init had noteJumpStartBeatOffset, but that's replaced with noteJumpValue.
            // noteJumpValue is only equal to the offset when the base game settings is Dynamic Default.

            // Will just have to make a note to users as instructions. Not worth trying to find the map when in TA, Campaigns or MP
            float noteJumpStartBeatOffset = noteJumpValue;  


            float mapNJS = startNoteJumpMovementSpeed;
            Logger.log.Debug("mapNJS:" + mapNJS.ToString());

            if (mapNJS <= 0.01) // Just in case?
                mapNJS = 10;


            // JD setpoint from Slider
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
            if (PluginConfig.Instance.usePreferredReactionTimeValues)
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

            //noteJumpStartBeatOffset = simOffset;            
            
            // 1.19.0+
            noteJumpValue = simOffset;

            //Logger.log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
            Logger.log.Debug($"DesiredJumpDis {desiredJumpDis} | Simulated Offset {simOffset}");
        }
    }


    [HarmonyPatch(typeof(MissionSelectionMapViewController), "SongPlayerCrossfadeToLevelAsync")]
    internal class MissionSelectionPatch
    {
        internal static IPreviewBeatmapLevel cc_level = null;

        static void Postfix(IPreviewBeatmapLevel level)
        {
            cc_level = level;
        }
    }

    // Not supporting 1.19.0 anymore
    /*[HarmonyPatch(typeof(CoreMathUtils), "CalculateHalfJumpDurationInBeats")]
    internal class CoreMathPatch
    {
        public static float Postfix(float __result, float startHalfJumpDurationInBeats, float maxHalfJumpDistance, float noteJumpMovementSpeed, float oneBeatDuration, float noteJumpStartBeatOffset)
        {
            // Force override 1.19.0's BPM lock
            if (Plugin.game_version == "1.19.0" && __result < 1.01)
            {
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

            return __result;
        }
    }*/


    //[HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), nameof(BeatmapObjectSpawnMovementData.GetJumpingNoteSpawnData))]
    internal class TimeControllerPatch
    {
        internal static DateTime af = new DateTime(2022, 4, 1);

        internal static BeatmapObjectSpawnMovementData.NoteSpawnData Postfix(BeatmapObjectSpawnMovementData.NoteSpawnData __result)
        {
            //if (DateTime.Now >= af) // Don't activate til midnight
            if (true)
            {
                float jumpDuration = __result.jumpDuration * (1 + TimeController.audioTime.songTime / TimeController.length * 0.75f); // * 1 might be more funny lol

                //Logger.log.Debug("songtime: " + TimeController.audioTime.songTime);
                //Logger.log.Debug("jumpDuration: " + jumpDuration);

                return new BeatmapObjectSpawnMovementData.NoteSpawnData(__result.moveStartPos, __result.moveEndPos, __result.jumpEndPos, __result.jumpGravity, __result.moveDuration, jumpDuration);
            }

            return __result;
        }
    }
}