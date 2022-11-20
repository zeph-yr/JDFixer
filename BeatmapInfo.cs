using System.Collections.Generic;

namespace JDFixer
{
    public delegate void BeatmapInfoEventHandler(BeatmapInfo e);
    public class BeatmapInfo
    {
        public static event BeatmapInfoEventHandler SelectedChanged;

        public static void SetSelected(IDifficultyBeatmap diff)
        {
            var updatedMapInfo = diff == null ? Empty : new BeatmapInfo(diff);
            Selected = updatedMapInfo;

            SelectedChanged?.Invoke(updatedMapInfo);
        }

        public static BeatmapInfo Selected { get; private set; } = Empty;

        public static BeatmapInfo Empty { get; } = new BeatmapInfo();

        private BeatmapInfo()
        {
            // To enable Campaigns and TA to show 0 instead of values from the last selected map in Solo Mode,
            // Better UX as players may forget to ignore the display.
            JumpDistance = 0f;
            MinJumpDistance = 0f;
            ReactionTime = 0f;
            MinReactionTime = 0f;

            // Ultra hack way to prevent divide by zero in Reaction Time Display
            NJS = 0.001f;

            // Experimental
            MinRTSlider = 0f;
            MaxRTSlider = 3000f;

            MinJDSlider = 0f;
            MaxJDSlider = 50f;

            // 1.26.0
            UnitJDOffset = 0.1f;
            UnitRTOffset = 5f;
        }

        public BeatmapInfo(IDifficultyBeatmap diff)
        {
            if (diff == null)
            {
                return;
            }

            float bpm = diff.level.beatsPerMinute;
            float njs = diff.noteJumpMovementSpeed;
            float offset = diff.noteJumpStartBeatOffset;

            if (njs <= 0.01f)
                njs = 10f;

            JumpDistance = BeatmapUtils.CalculateJumpDistance(bpm, njs, offset);
            MinJumpDistance = BeatmapUtils.CalculateJumpDistance(bpm, njs, -50f);
            NJS = njs;
            ReactionTime = JumpDistance * 500 / NJS;
            MinReactionTime = MinJumpDistance * 500 / NJS;

            // Experimental
            if (PluginConfig.Instance.slider_setting == 0)
            {
                MinRTSlider = PluginConfig.Instance.minJumpDistance * 500 / NJS;
                MaxRTSlider = PluginConfig.Instance.maxJumpDistance * 500 / NJS;

                MinJDSlider = PluginConfig.Instance.minJumpDistance;
                MaxJDSlider = PluginConfig.Instance.maxJumpDistance;
            }
            else
            {
                MinRTSlider = PluginConfig.Instance.minReactionTime;
                MaxRTSlider = PluginConfig.Instance.maxReactionTime;

                MinJDSlider = PluginConfig.Instance.minReactionTime * NJS / 500;
                MaxJDSlider = PluginConfig.Instance.maxReactionTime * NJS / 500;
            }

            // 1.26.0
            Offset = offset;

            UnitJDOffset = BeatmapUtils.CalculateJumpDistance(bpm, njs, offset + 1 / PluginConfig.Instance.offset_fraction) - BeatmapUtils.CalculateJumpDistance(bpm, njs, offset); // 1/8th beat offset in JD units
            Logger.log.Debug("QOffset JD: " + UnitJDOffset);

            UnitRTOffset = UnitJDOffset * 500 / njs;
            Logger.log.Debug("QOffset RT: " + UnitRTOffset);

            if (PluginConfig.Instance.use_offset)
            {
                Create_Snap_Points(ref JD_Snap_Points, ref JD_Offset_Points, Offset, JumpDistance, UnitJDOffset, MinJDSlider, MaxJDSlider);
                Create_Snap_Points(ref RT_Snap_Points, ref RT_Offset_Points, Offset, ReactionTime, UnitRTOffset, MinRTSlider, MaxRTSlider);
            }


            //Logger.log.Debug("BeatmapInfo minJD: " + PluginConfig.Instance.minJumpDistance);
            //Logger.log.Debug("BeatmapInfo maxJD: " + PluginConfig.Instance.maxJumpDistance);
            //Logger.log.Debug("BeatmapInfo minRT: " + MinRTSlider);
            //Logger.log.Debug("BeatmapInfo maxRT: " + MaxRTSlider);
        }

        public float JumpDistance { get; }
        public float MinJumpDistance { get; }
        public float NJS { get; }
        public float ReactionTime { get; }
        public float MinReactionTime { get; }

        // Experimental
        public float MinRTSlider { get; }
        public float MaxRTSlider { get; }

        public float MinJDSlider { get; }
        public float MaxJDSlider { get; }


        // 1.26.0
        public float Offset { get; }
        public float UnitJDOffset { get; }
        public float UnitRTOffset { get; }

        internal static List<float> JD_Snap_Points = new List<float>();
        internal static List<float> RT_Snap_Points = new List<float>();

        internal static List<string> JD_Offset_Points = new List<string>();
        internal static List<string> RT_Offset_Points = new List<string>();




        internal static void Create_Snap_Points(ref List<float> Snap_Points, ref List<string> Offset_Points, float Offset, float _selectedBeatmap_JD_RT, float _selectedBeatmap_UnitOffset, float _selectedBeatmap_MinSlider, float _selectedBeatmap_MaxSlider)
        {
            Logger.log.Debug("Create Snap Points");
            Logger.log.Debug("Min: " + _selectedBeatmap_MinSlider + " " + _selectedBeatmap_MaxSlider);

            Snap_Points.Clear();
            Snap_Points.Add(_selectedBeatmap_JD_RT);

            Offset_Points.Clear();
            Offset_Points.Add("[ 0 | " + Offset + " ]");

            float point = _selectedBeatmap_JD_RT + _selectedBeatmap_UnitOffset;
            int multiple = 1;
            while (point <= _selectedBeatmap_MaxSlider)
            {
                Snap_Points.Add(point);
                point += _selectedBeatmap_UnitOffset;

                Offset_Points.Add("[ " + multiple + "/" + PluginConfig.Instance.offset_fraction + " | " + (Offset + multiple / PluginConfig.Instance.offset_fraction) + " ]");
                multiple += 1;
            }

            point = _selectedBeatmap_JD_RT - _selectedBeatmap_UnitOffset;
            multiple = -1;
            while (point >= _selectedBeatmap_MinSlider)
            {
                Snap_Points.Insert(0, point);
                point -= _selectedBeatmap_UnitOffset;

                Offset_Points.Insert(0, "[ " + multiple + "/" + PluginConfig.Instance.offset_fraction + " | " + (Offset + multiple / PluginConfig.Instance.offset_fraction) + " ]");
                multiple -= 1;
            }

            // Debug:
            for (int i = 0; i < Snap_Points.Count; i++)
            {
                Logger.log.Debug(i + ": " + Snap_Points[i]);
                Logger.log.Debug(i + ": " + Offset_Points[i]);
            }
        }

        internal static (string, float) Calculate_Nearest_Snap_Point(ref List<float> Snap_Points, ref List<string> Offset_Points, float JD_RT_Value)
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
            return (Offset_Points[Snap_Points.Count - 1], Snap_Points[Snap_Points.Count - 1]);

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
        }
    }
}
