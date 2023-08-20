using HarmonyLib;
using System;
using System.Linq;

namespace JDFixer
{
    [HarmonyPatch(typeof(BeatmapObjectSpawnMovementData), "Init")]
    internal class SpawnMovementDataUpdatePatch
    {
        internal static void Prefix(ref float startNoteJumpMovementSpeed, float startBpm, /*ref float noteJumpStartBeatOffset,*/ ref BeatmapObjectSpawnMovementData.NoteJumpValueType noteJumpValueType, ref float noteJumpValue)
        {
            if (PluginConfig.Instance.enabled == false)
            {
                return;
            }

           Plugin.Log.Debug("Start Map");

            // BS 1.19.0
            noteJumpValueType = BeatmapObjectSpawnMovementData.NoteJumpValueType.BeatOffset;

            // Bit of an issue... to calculate the map's original JD for the heuristic, we need the map's offset
            // This was fine when Init had noteJumpStartBeatOffset, but that's replaced with noteJumpValue.
            // noteJumpValue is only equal to the offset when the base game settings is Dynamic Default.

            // Will just have to make a note to users as instructions. Not worth trying to find the map when in TA, Campaigns or MP
            float noteJumpStartBeatOffset = noteJumpValue;  

            float mapNJS = startNoteJumpMovementSpeed;
            Plugin.Log.Debug("mapNJS:" + mapNJS.ToString());

            if (mapNJS <= 0.01) // Just in case?
                mapNJS = 10;

            // JD setpoint from Slider
            // 1.19.1
            float desiredJumpDis;

            if (PluginConfig.Instance.slider_setting == 0)
            {
                desiredJumpDis = PluginConfig.Instance.jumpDistance;
            }
            else
            {
                desiredJumpDis = PluginConfig.Instance.reactionTime * mapNJS / 500;
            }

            // 1.26.0-1.29.0 Feature update
            if (PluginConfig.Instance.use_offset && PluginConfig.Instance.legacy_display_enabled && 
                PluginConfig.Instance.use_rt_pref == false && PluginConfig.Instance.use_jd_pref == false)
            {
                if (PluginConfig.Instance.slider_setting == 0)
                {
                    desiredJumpDis = BeatmapOffsets.jd_snap_value;
                }
                else
                {
                    desiredJumpDis = BeatmapOffsets.rt_snap_value * mapNJS / 500;
                }
            }

            // NJS-RT setpoints from Preferences
            if (PluginConfig.Instance.use_rt_pref)
            {
                if (mapNJS <= PluginConfig.Instance.lower_threshold || mapNJS >= PluginConfig.Instance.upper_threshold)
                {
                    Plugin.Log.Debug("Using Threshold");
                    
                    //return;
                    desiredJumpDis = BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset);
                    goto SongSpeed; // Yes, a goto.
                }
                
                var rt_pref = PluginConfig.Instance.rt_preferredValues.FirstOrDefault(x => x.njs <= mapNJS);
                Plugin.Log.Debug("Using Preference");

                if (rt_pref != null)
                    desiredJumpDis = rt_pref.reactionTime * mapNJS / 500;

                if (BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset) <= desiredJumpDis && PluginConfig.Instance.use_heuristic == 1)
                {
                    Plugin.Log.Debug("Not Fixing: Original JD below or equal setpoint");
                    Plugin.Log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");

                    //return;
                    desiredJumpDis = BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset);
                    goto SongSpeed;
                }
            }

            // NJS-JD setpoints from Preferences
            else if (PluginConfig.Instance.use_jd_pref)
            {
                if (mapNJS <= PluginConfig.Instance.lower_threshold || mapNJS >= PluginConfig.Instance.upper_threshold)
                {
                    Plugin.Log.Debug("Using Threshold");
                    Plugin.Log.Debug("selected:" + BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset));
                    //return;
                    desiredJumpDis = BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset);
                    goto SongSpeed;
                }

                var pref = PluginConfig.Instance.preferredValues.FirstOrDefault(x => x.njs <= mapNJS);
                Plugin.Log.Debug("Using Preference");

                if (pref != null)
                    desiredJumpDis = pref.jumpDistance;

                // Heuristic: If map's original JD is less than the matching preference entry, play map at original JD
                // Rationale: I created this mod because I don't like floaty maps. If the original JD chosen by the
                // mapper is lower than my pick, it's probably more optimal than my pick.
                if (BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset) <= desiredJumpDis && PluginConfig.Instance.use_heuristic == 1)
                {
                    Plugin.Log.Debug("Not Fixing: Original JD below or equal setpoint");
                    Plugin.Log.Debug($"BPM/NJS/Offset {startBpm}/{startNoteJumpMovementSpeed}/{noteJumpStartBeatOffset}");

                    //return;
                    desiredJumpDis = BeatmapUtils.CalculateJumpDistance(startBpm, mapNJS, noteJumpStartBeatOffset);
                    goto SongSpeed;
                }
            }

            // 1.29.1
            SongSpeed:
            desiredJumpDis = SpawnMovementDataUpdateHelper.Get_Modified_DesiredJD(desiredJumpDis, mapNJS);

            // Calculate New Offset Given Desired JD:
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
            noteJumpValue = simOffset;  // 1.19.0+

            //Plugin.Log.Debug($"HalfJumpCurrent: {num2Curr} | DesiredHalfJump {desiredHalfJumpDur} | DesiredJumpDis {desiredJumpDis} | CurrJumpDis {jumpDisCurr} | Simulated Offset {simOffset}");
            Plugin.Log.Debug($"DesiredJumpDis {desiredJumpDis} | Simulated Offset {simOffset}");
        }
    }


    [HarmonyPatch(typeof(MissionSelectionMapViewController), "SongPlayerCrossfadeToLevelAsync")]
    internal class MissionSelectionPatch
    {
        internal static IPreviewBeatmapLevel cc_level = null;
        internal static void Postfix(IPreviewBeatmapLevel level)
        {
            cc_level = level;
        }
    }


    [HarmonyPatch(typeof(StandardLevelScenesTransitionSetupDataSO), "Init")]
    internal class StandardLevelScenesTransitionSetupDataSOPatch
    {
        internal static void Postfix(GameplayModifiers gameplayModifiers, PracticeSettings practiceSettings)
        {
            BeatmapInfo.speedMultiplier = gameplayModifiers.songSpeedMul;
            if (practiceSettings != null)
            {
                BeatmapInfo.speedMultiplier = practiceSettings.songSpeedMul;
            }
        }
    }


    [HarmonyPatch(typeof(MultiplayerLevelScenesTransitionSetupDataSO), "Init")]
    internal class MultiplayerLevelScenesTransitionSetupDataSOPatch
    {
        internal static void Postfix(GameplayModifiers gameplayModifiers)
        {
            BeatmapInfo.speedMultiplier = gameplayModifiers.songSpeedMul;
        }
    }


    internal class SpawnMovementDataUpdateHelper
    {
        internal static float Get_Modified_DesiredJD(float jumpDis, float mapNJS)
        {
            float new_RT = BeatmapUtils.Calculate_ReactionTime_Setpoint_Float(jumpDis, mapNJS) * BeatmapInfo.speedMultiplier;

            if (PluginConfig.Instance.song_speed_setting == 1)
            {
                return BeatmapUtils.Calculate_JumpDistance_Setpoint_Float(new_RT, mapNJS);
            }
            else if (PluginConfig.Instance.song_speed_setting == 2 &&
                    (PluginConfig.Instance.use_rt_pref || (PluginConfig.Instance.slider_setting == 1 && PluginConfig.Instance.use_jd_pref == false)))
            {
                return BeatmapUtils.Calculate_JumpDistance_Setpoint_Float(new_RT, mapNJS);
            }
            else
            {
                return jumpDis;
            }
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


    internal class TimeControllerPatch
    {
        private static DateTime af = new DateTime(DateTime.Now.Year, 4, 1);
        internal static BeatmapObjectSpawnMovementData.NoteSpawnData Postfix(BeatmapObjectSpawnMovementData.NoteSpawnData __result)
        {
            if (DateTime.Now >= af && TimeController.audioTime.songTime >= 5f)
            {
                //float jumpDuration = __result.jumpDuration * (1 + TimeController.audioTime.songTime / TimeController.length * 0.75f); // * 1 might be more funny lol
                float jumpDuration = (float)(__result.jumpDuration * (1 + 0.30 * Math.Abs(Math.Sin(4 * Math.PI * (TimeController.audioTime.songTime - 5f) / TimeController.length))));
                //Plugin.Log.Debug("jumpDuration: " + jumpDuration);

                return new BeatmapObjectSpawnMovementData.NoteSpawnData(__result.moveStartPos, __result.moveEndPos, __result.jumpEndPos, __result.jumpGravity, __result.moveDuration, jumpDuration);
            }

            return __result;
        }
    }
}