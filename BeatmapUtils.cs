namespace JDFixer
{
    public static class BeatmapUtils
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
    }
}