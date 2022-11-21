using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDFixer
{
    class BeatmapOffsets
    {
        internal static List<float> JD_Snap_Points = new List<float>();
        internal static List<float> RT_Snap_Points = new List<float>();

        internal static List<string> JD_Offset_Points = new List<string>();
        internal static List<string> RT_Offset_Points = new List<string>();

        internal static float jd_snap_value = 0f;
        internal static float rt_snap_value = 0f;

        internal static string jd_offset_snap_value = "";
        internal static string rt_offset_snap_value = "";


        internal static void Create_Snap_Points(ref List<float> Snap_Points, ref List<string> Offset_Points, float _selectedBeatmap_Offset, float _selectedBeatmap_JD_RT, float _selectedBeatmap_UnitOffset, float _selectedBeatmap_MinSlider, float _selectedBeatmap_MaxSlider)
        {
            Logger.log.Debug("Create Snap Points");
            Logger.log.Debug("Min: " + _selectedBeatmap_MinSlider + " " + _selectedBeatmap_MaxSlider);

            Snap_Points.Clear();
            Snap_Points.Add(_selectedBeatmap_JD_RT);

            Offset_Points.Clear();
            Offset_Points.Add("( 0, " + _selectedBeatmap_Offset.ToString("0.##") + " )");

            float point = _selectedBeatmap_JD_RT + _selectedBeatmap_UnitOffset;
            int multiple = 1;
            while (point <= _selectedBeatmap_MaxSlider)
            {
                Snap_Points.Add(point);
                point += _selectedBeatmap_UnitOffset;

                Offset_Points.Add("( " + multiple + "/" + PluginConfig.Instance.offset_fraction + ", " + (_selectedBeatmap_Offset + multiple / PluginConfig.Instance.offset_fraction).ToString("0.##") + " )");
                multiple += 1;
            }

            point = _selectedBeatmap_JD_RT - _selectedBeatmap_UnitOffset;
            multiple = -1;
            while (point >= _selectedBeatmap_MinSlider)
            {
                Snap_Points.Insert(0, point);
                point -= _selectedBeatmap_UnitOffset;

                Offset_Points.Insert(0, "( " + multiple + "/" + PluginConfig.Instance.offset_fraction + ", " + (_selectedBeatmap_Offset + multiple / PluginConfig.Instance.offset_fraction).ToString("0.##") + " )");
                multiple -= 1;
            }

            // Debug:
            for (int i = 0; i < Snap_Points.Count; i++)
            {
                Logger.log.Debug(i + ": " + Snap_Points[i]);
                Logger.log.Debug(i + ": " + Offset_Points[i]);
            }
        }


        internal static void Calculate_Nearest_JD_Snap_Point(float JD_Value)
        {
            Logger.log.Debug("Count: " + JD_Snap_Points.Count + " " + JD_Value);

            if (JD_Snap_Points.Count == 0)
            {
                Logger.log.Debug("empty: " + JD_Value);

                jd_offset_snap_value = "";
                jd_snap_value = JD_Value;

                return;
            }

            for (int i = 0; i < JD_Snap_Points.Count; i++)
            {
                Logger.log.Debug(i + ": " + JD_Snap_Points[i]);

                if (JD_Snap_Points[i] >= JD_Value)
                {
                    jd_offset_snap_value = JD_Offset_Points[i];
                    jd_snap_value = JD_Snap_Points[i];

                    return;
                }
            }

            jd_offset_snap_value = JD_Offset_Points[JD_Offset_Points.Count - 1];
            jd_snap_value = JD_Snap_Points[JD_Snap_Points.Count - 1];
        }


        internal static void Calculate_Nearest_RT_Snap_Point(float RT_Value)
        {
            Logger.log.Debug("Count: " + RT_Snap_Points.Count + " " + RT_Value);

            if (RT_Snap_Points.Count == 0)
            {
                Logger.log.Debug("empty: " + RT_Value);

                rt_offset_snap_value = "";
                rt_snap_value = RT_Value;

                return;
            }

            for (int i = 0; i < RT_Snap_Points.Count; i++)
            {
                Logger.log.Debug(i + ": " + RT_Snap_Points[i]);

                if (RT_Snap_Points[i] >= RT_Value)
                {
                    rt_offset_snap_value = RT_Offset_Points[i];
                    rt_snap_value = RT_Snap_Points[i];

                    return;
                }
            }

            rt_offset_snap_value = RT_Offset_Points[RT_Offset_Points.Count - 1];
            rt_snap_value = RT_Snap_Points[RT_Snap_Points.Count - 1];
        }

        /*internal static (string, float) Calculate_Nearest_Snap_Point(ref List<float> Snap_Points, ref List<string> Offset_Points, float JD_RT_Value)
        {
            Logger.log.Debug("Count: " + Snap_Points.Count + " " + JD_RT_Value);

            if (Snap_Points.Count == 0)
            {
                Logger.log.Debug("empty: " + JD_RT_Value);
                return ("", JD_RT_Value);
            }

            //int index = 0;
            for (int i = 0; i < Snap_Points.Count; i++)
            {
                //index = i;
                Logger.log.Debug(i + ": " + Snap_Points[i]);

                if (Snap_Points[i] >= JD_RT_Value)
                {
                    return (Offset_Points[i], Snap_Points[i]);
                    //break;
                }
            }
            return (Offset_Points[Snap_Points.Count - 1], Snap_Points[Snap_Points.Count - 1]);*/

        /*if (Snap_Points.Count == 1)
        {
            Logger.log.Debug("single index: " + Snap_Points[0]);
            return (JD_Offset_Points[0], Snap_Points[0]);
        }

        int index = Snap_Points.BinarySearch(JD_RT_Value);
        Logger.log.Debug("index: " + index);

        if (index < 0)
        {
            Logger.log.Debug("~index: " + ~index);

            if (~index >= Snap_Points.Count)
            {
                Logger.log.Debug("end index: " + Snap_Points[~index - 1]);
                return (Offset_Points[~index - 1], Snap_Points[~index - 1]);
            }

            Logger.log.Debug("nearest upper index: " + Snap_Points[~index]);
            return (Offset_Points[~index], Snap_Points[~index]);
        }

        Logger.log.Debug("exact index: " + Snap_Points[index]);
        return (Offset_Points[index], Snap_Points[index]);*/
        //}
    }
}
