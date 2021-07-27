using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JDFixer
{
    public class JDPref
    {
        public float njs = 12f;
        public float jumpDistance = 24f;

        public JDPref(float njs, float jumpDistance)
        {
            this.njs = njs;
            this.jumpDistance = jumpDistance;
        }
    }

    public class JDFixerConfig
    {
        public bool enabled = false;
        public bool enabledInPractice = false;
        public float jumpDistance = 24f;
        public int minJumpDistance = 15;
        public int maxJumpDistance = 35;
        public bool usePreferredJumpDistanceValues = false;
        public List<JDPref> preferredValues = new List<JDPref>();

        // Defaults for when upper and lower thresholds not set by user (yes it's a bandaid)
        public float upper_threshold = 100f;
        public float lower_threshold = 1f;
        public bool use_heuristic = true;

        // Values for current selected map difficulty
        //public float selected_mapBPM = 1f;
        //public float selected_mapNJS = 1f;
        //public float selected_mapOffset = 1f;
        //public float selected_mapJumpDistance = 1f;
        //public float selected_mapLowest = 1f;


        public JDFixerConfig()
        {

        }
        [JsonConstructor]
        //public JDFixerConfig(bool enabled, bool enabledInPractice, float jumpDistance, int minJumpDistance, int maxJumpDistance, bool usePreferredJumpDistanceValues, List<NjsPref> preferredValues,float upper_threshold, float lower_threshold, float selected_mapBPM, float selected_mapNJS, float selected_mapOffset, float selected_mapJumpDistance)
        public JDFixerConfig(bool enabled, bool enabledInPractice, float jumpDistance, int minJumpDistance, int maxJumpDistance, bool usePreferredJumpDistanceValues, List<JDPref> preferredValues, 
            float upper_threshold, float lower_threshold, bool use_heuristic)
        {
            this.enabled = enabled;
            this.enabledInPractice = enabledInPractice;
            this.jumpDistance = jumpDistance;
            this.minJumpDistance = minJumpDistance;
            this.maxJumpDistance = maxJumpDistance;
            this.usePreferredJumpDistanceValues = usePreferredJumpDistanceValues;
            this.preferredValues = preferredValues;

            this.upper_threshold = upper_threshold;
            this.lower_threshold = lower_threshold;
            this.use_heuristic = use_heuristic;

            // Values for current selected map difficulty
            //this.selected_mapBPM = selected_mapBPM;
            //this.selected_mapNJS = selected_mapNJS;
            //this.selected_mapOffset = selected_mapOffset;
            //this.selected_mapJumpDistance = selected_mapJumpDistance;
            //this.selected_mapLowest = selected_mapLowest;
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
        }

        public static void Write()
        {
            UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.njs).ToList();
            //UserConfig.preferredValues = UserConfig.preferredValues.OrderByDescending(x => x.jumpDistance).ToList(); // Sort by JD Setpoint

            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(UserConfig, Formatting.Indented));
        }
    }
}