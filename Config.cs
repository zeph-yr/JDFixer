/*using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JDFixer
{
    public class JDPref
    {
        public float njs = 16f;
        public float jumpDistance = 24f;

        public JDPref(float njs, float jumpDistance)
        {
            this.njs = njs;
            this.jumpDistance = jumpDistance;
        }
    }

    // Reaction Time Mode
    public class RTPref
    {
        public float njs = 16f;
        public float reactionTime = 800f;

        public RTPref(float njs, float reactionTime)
        {
            this.njs = njs;
            this.reactionTime = reactionTime;
        }
    }


    public class JDFixerConfig
    {
        public bool enabled;
        //public bool enabledInPractice;
        public float jumpDistance;
        public int minJumpDistance;
        public int maxJumpDistance;
        public bool usePreferredJumpDistanceValues;
        public List<JDPref> preferredValues;
        
        public float upper_threshold;
        public float lower_threshold;
        public bool use_heuristic;

        // Reaction Time Mode
        public bool rt_enabled;
        public List<RTPref> rt_preferredValues;
        public int minReactionTime;
        public int maxReactionTime;
        public bool rt_display_enabled;

        // Values for current selected map difficulty
        //public float selected_mapBPM = 1f;
        //public float selected_mapNJS = 1f;
        //public float selected_mapOffset = 1f;
        //public float selected_mapJumpDistance = 1f;
        //public float selected_mapLowest = 1f;


        public JDFixerConfig()
        {
            enabled = false;
            //enabledInPractice = false;
            jumpDistance = 24f;
            minJumpDistance = 15;
            maxJumpDistance = 35;
            usePreferredJumpDistanceValues = false;
            preferredValues = new List<JDPref>();

            // Defaults for when upper and lower thresholds not set by user (yes it's a bandaid)
            upper_threshold = 100f;
            lower_threshold = 1f;
            use_heuristic = true;

            // Reaction Time Mode
            rt_enabled = false;
            rt_preferredValues = new List<RTPref>();
            minReactionTime = 100;
            maxReactionTime = 2000;
            rt_display_enabled = true;
        }

        [JsonConstructor]
        //public JDFixerConfig(bool enabled, bool enabledInPractice, float jumpDistance, int minJumpDistance, int maxJumpDistance, bool usePreferredJumpDistanceValues, List<NjsPref> preferredValues,float upper_threshold, float lower_threshold, float selected_mapBPM, float selected_mapNJS, float selected_mapOffset, float selected_mapJumpDistance)
        public JDFixerConfig(bool enabled, float jumpDistance, int minJumpDistance, int maxJumpDistance, bool usePreferredJumpDistanceValues, List<JDPref> preferredValues, 
            float upper_threshold, float lower_threshold, bool use_heuristic, 
            bool rt_enabled, List<RTPref> rt_preferredValues, int minReactionTime, int maxReactionTime, bool rt_display_enabled)
        {
            this.enabled = enabled;
            //this.enabledInPractice = enabledInPractice;
            this.jumpDistance = jumpDistance;
            this.minJumpDistance = minJumpDistance;
            this.maxJumpDistance = maxJumpDistance;
            this.usePreferredJumpDistanceValues = usePreferredJumpDistanceValues;
            this.preferredValues = preferredValues;

            this.upper_threshold = upper_threshold;
            this.lower_threshold = lower_threshold;
            this.use_heuristic = use_heuristic;

            // Reaction Time Mode
            this.rt_enabled = rt_enabled;
            this.rt_preferredValues = rt_preferredValues;
            this.minReactionTime = minReactionTime;
            this.maxReactionTime = maxReactionTime;
            this.rt_display_enabled = rt_display_enabled;
        }
    }

    public class Config
    {
        public static JDFixerConfig UserConfig { get; private set; }
        public static string ConfigPath { get; private set; } = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "JDFixer.json");

        public static void Read()
        {
            if (!File.Exists(ConfigPath))
            {
                UserConfig = new JDFixerConfig();
                Write();
            }
            else
            {
                UserConfig = JsonConvert.DeserializeObject<JDFixerConfig>(File.ReadAllText(ConfigPath));
            }
            UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.njs).ToList();
            //UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.jumpDistance).ToList(); // Sort by JD Setpoint

            UserConfig.rt_preferredValues = UserConfig.rt_preferredValues.OrderByDescending(x => x.njs).ToList();
        }

        public static void Write()
        {
            UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.njs).ToList();
            //UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.jumpDistance).ToList(); // Sort by JD Setpoint

            UserConfig.rt_preferredValues = UserConfig.rt_preferredValues.OrderByDescending(x => x.njs).ToList();

            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(UserConfig, Formatting.Indented));
        }
    }
}*/