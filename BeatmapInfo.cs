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
            UnitJDOffset = BeatmapUtils.CalculateJumpDistance(bpm, njs, offset + PluginConfig.Instance.offset_fraction) - BeatmapUtils.CalculateJumpDistance(bpm, njs, offset); // 1/8th beat offset in JD units
            Logger.log.Debug("QOffset JD: " + UnitJDOffset);

            UnitRTOffset = BeatmapUtils.Calculate_ReactionTime_Setpoint_Float(bpm, njs) * PluginConfig.Instance.offset_fraction;
            Logger.log.Debug("QOffset RT: " + UnitRTOffset);


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
        public float UnitJDOffset { get; }
        public float UnitRTOffset { get; }
    }
}
