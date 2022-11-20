using System.Collections.Generic;
using UnityEngine;

namespace JDFixer
{
    static class BeatmapUtils
    {
        public static float CalculateJumpDistance(float bpm, float njs, float offset)
        {
            float jumpdistance = 0f; // In case
            float halfjump = 4f;
            float num = 60f / bpm;

            // Need to repeat this here even tho it's in BeatmapInfo because sometimes we call this function directly
            if (njs <= 0.01) // Is it ok to == a 0f?
                njs = 10f;

            while (njs * num * halfjump > 17.999)
                halfjump /= 2;

            halfjump += offset;
            if (halfjump < 0.25f)
                halfjump = 0.25f;

            jumpdistance = njs * num * halfjump * 2;

            return jumpdistance;
        }

        // Cant make these public set in BeatmapInfo, crashes
        /*public static void RefreshSliderMinMax(float njs)
        {
            if (PluginConfig.Instance.slider_setting == 0)
            {
                BeatmapInfo.Selected.MinRTSlider = PluginConfig.Instance.minJumpDistance * 500 / njs;
                BeatmapInfo.Selected.MaxRTSlider = PluginConfig.Instance.maxJumpDistance * 500 / njs;

                BeatmapInfo.Selected.MinJDSlider = PluginConfig.Instance.minJumpDistance;
                BeatmapInfo.Selected.MaxJDSlider = PluginConfig.Instance.maxJumpDistance;
            }
            else
            {
                BeatmapInfo.Selected.MinRTSlider = PluginConfig.Instance.minReactionTime;
                BeatmapInfo.Selected.MaxRTSlider = PluginConfig.Instance.maxReactionTime;

                BeatmapInfo.Selected.MinJDSlider = PluginConfig.Instance.minReactionTime * njs / 500;
                BeatmapInfo.Selected.MaxJDSlider = PluginConfig.Instance.maxReactionTime * njs / 500;
            }
        }*/


        internal static string Calculate_ReactionTime_Setpoint_String(float JD_Value, float _selectedBeatmap_NJS)
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap_NJS > 0.002)
                return "<#cc99ff>" + (JD_Value / (2 * _selectedBeatmap_NJS) * 1000).ToString("0") + " ms";

            return "<#cc99ff>0 ms";
        }

        internal static float Calculate_ReactionTime_Setpoint_Float(float JD_Value, float _selectedBeatmap_NJS)
        {
            if (_selectedBeatmap_NJS > 0.002)
                return JD_Value / (2 * _selectedBeatmap_NJS) * 1000;

            return 0f;
        }


        internal static string Calculate_JumpDistance_Setpoint_String(float RT_Value, float _selectedBeatmap_NJS)
        {
            return "<#ffff00>" + (RT_Value * (2 * _selectedBeatmap_NJS) / 1000).ToString("0.#");
        }

        internal static float Calculate_JumpDistance_Setpoint_Float(float RT_Value, float _selectedBeatmap_NJS)
        {
            return RT_Value * (2 * _selectedBeatmap_NJS) / 1000;
        }


        // 1.26.0
        internal static float Get_JD_Increment_For_RT(float RT_Value, float _selectedBeatmap_MinRTSlider, float _selectedBeatmap_MaxRTSlider, float _selectedBeatmap_MinJDSlider, float _selectedBeatmap_MaxJDSlider)
        {
            return (_selectedBeatmap_MaxJDSlider - _selectedBeatmap_MinJDSlider) * RT_Value / (_selectedBeatmap_MaxRTSlider - _selectedBeatmap_MinRTSlider);
        }

        internal static int Get_JD_NumberOfSteps(float Step_JD_Slider, float _selectedBeatmap_MinJDSlider, float _selectedBeatmap_MaxJDSlider)
        {
            return Mathf.RoundToInt((_selectedBeatmap_MaxJDSlider - _selectedBeatmap_MinJDSlider) / Step_JD_Slider + 1);
        }

        internal static int Get_Num_Snap_Points()
        {
            return jd_snap_points.Count;
        }


        private static List<float> jd_snap_points = new List<float>();

        internal static void Create_JD_Snap_Points(float _selectedBeatmap_JumpDistance, float _selectedBeatmap_UnitJDOffset, float _selectedBeatmap_MinJDSlider, float _selectedBeatmap_MaxJDSlider)
        {
            jd_snap_points.Clear();
            jd_snap_points.Add(_selectedBeatmap_JumpDistance);

            float point = _selectedBeatmap_JumpDistance + _selectedBeatmap_UnitJDOffset;
            while (point <= _selectedBeatmap_MaxJDSlider)
            {
                jd_snap_points.Add(point);
                point += _selectedBeatmap_UnitJDOffset;
            }

            point = _selectedBeatmap_JumpDistance - _selectedBeatmap_UnitJDOffset;
            while (point >= _selectedBeatmap_MinJDSlider)
            {
                jd_snap_points.Insert(0, point);
                point -= _selectedBeatmap_UnitJDOffset;
            }

            for (int i = 0; i < jd_snap_points.Count; i++)
            {
                Logger.log.Debug(i + ": " + jd_snap_points[i]);
            }
        }

        internal static float Calculate_JumpDistance_Nearest_Offset(float JD_Value)
        {
            Logger.log.Debug("Count: " + jd_snap_points.Count);

            if (jd_snap_points.Count == 0)
            {
                Logger.log.Debug("empty: " + JD_Value);
                return JD_Value;
            }

            if (jd_snap_points.Count == 1)
            {
                Logger.log.Debug("single index: " + jd_snap_points[0]);
                return jd_snap_points[0];
            }

            int index = jd_snap_points.BinarySearch(JD_Value);
            Logger.log.Debug("index: " + index);

            if (index < 0)
            {
                Logger.log.Debug("~index: " + ~index);
                Logger.log.Debug("~index - 1: " + (~index - 1));


                if (~index >= jd_snap_points.Count)
                {
                    Logger.log.Debug("nearest lower index: " + jd_snap_points[~index - 1]);
                    return jd_snap_points[~index - 1];
                }

                Logger.log.Debug("nearest upper index: " + jd_snap_points[~index]);
                return jd_snap_points[~index];
            }

            Logger.log.Debug("exact index: " + jd_snap_points[index]);
            return jd_snap_points[index];
        }


        private static List<float> rt_snap_points = new List<float>();

        internal static void Create_RT_Snap_Points(float _selectedBeatmap_ReactionTime, float _selectedBeatmap_UnitRTOffset, float _selectedBeatmap_MinRTSlider, float _selectedBeatmap_MaxRTSlider)
        {
            rt_snap_points.Clear();
            rt_snap_points.Add(_selectedBeatmap_ReactionTime);

            float point = _selectedBeatmap_ReactionTime + _selectedBeatmap_UnitRTOffset;
            while (point <= _selectedBeatmap_MaxRTSlider)
            {
                rt_snap_points.Add(point);
                point += _selectedBeatmap_UnitRTOffset;
            }

            point = _selectedBeatmap_ReactionTime - _selectedBeatmap_UnitRTOffset;
            while (point >= _selectedBeatmap_MinRTSlider)
            {
                rt_snap_points.Insert(0, point);
                point -= _selectedBeatmap_UnitRTOffset;
            }

            for (int i = 0; i < rt_snap_points.Count; i++)
            {
                Logger.log.Debug(i + ": " + rt_snap_points[i]);
            }
        }

        internal static float Calculate_ReactionTime_Nearest_Offset(float RT_Value)
        {
            Logger.log.Debug("Count: " + rt_snap_points.Count);

            if (rt_snap_points.Count == 0)
            {
                return RT_Value;
            }

            if (rt_snap_points.Count == 1)
            {
                return rt_snap_points[0];
            }

            int index = rt_snap_points.BinarySearch(RT_Value);

            if (index < 0)
            {
                if (~index >= rt_snap_points.Count)
                {
                    return rt_snap_points[~index - 1];
                }

                return rt_snap_points[~index];
            }

            return rt_snap_points[index];
        }
    }
}
