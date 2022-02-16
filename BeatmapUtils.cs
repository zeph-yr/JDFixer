using BeatSaberMarkupLanguage.GameplaySetup;

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
    }
}
