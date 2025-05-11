using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;


[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace JDFixer
{
    internal class PluginConfig
    {
        public void OnLoad()
        {
            CheckConflicts();

            var converted = false;
            converted |= TryConvertJD();
            converted |= TryConvertRT();

            if (!converted) return;

            Plugin.Log.Info("Converted legacy preferred values");
        }

        private bool TryConvertJD()
        {
            if (preferredValues == null) return false;

            preferredValues_jd.Add(preferredValues);
            preferredValues = null;
            return true;
        }

        private bool TryConvertRT()
        {
            if (rt_preferredValues == null) return false;

            preferredValues_rt.Add(rt_preferredValues);
            rt_preferredValues = null;
            return true;
        }

        private void CheckConflicts()
        {
            if (pref_selected > preferredValues_jd.Count + preferredValues_rt.Count)
            {
                Plugin.Log.Info(string.Format("Invalid pref_selected value was reset ({0} > {1})", pref_selected,
                    preferredValues_jd.Count + preferredValues_rt.Count));
                pref_selected = 0;
            }

            if (use_jd_pref >= preferredValues_jd.Count)
            {
                Plugin.Log.Info("Invalid use_jd_pref value was reset");
                use_jd_pref = -1;
            }

            if (use_rt_pref >= preferredValues_rt.Count)
            {
                Plugin.Log.Info("Invalid use_rt_pref value was reset");
                use_rt_pref = -1;
            }
        }


        internal static PluginConfig Instance { get; set; }

        internal virtual bool enabled { get; set; } = false;


        internal float jumpDistance { get; set; } = 24f;
        internal virtual int minJumpDistance { get; set; } = 12;
        internal virtual int maxJumpDistance { get; set; } = 35;
        internal virtual int use_jd_pref { get; set; } = -1;

        [UseConverter(typeof(ListConverter<List<JDPref>>))]
        [NonNullable]
        internal virtual List<List<JDPref>> preferredValues_jd { get; set; } = new List<List<JDPref>>();

        /// <summary>
        /// Legacy config value for JumpDistance preferred-values
        /// </summary>
        [UseConverter(typeof(ListConverter<JDPref>))]
        protected virtual List<JDPref> preferredValues { get; set; }


        internal float reactionTime { get; set; } = 500f;
        internal virtual int minReactionTime { get; set; } = 300;
        internal virtual int maxReactionTime { get; set; } = 1600;
        internal virtual int use_rt_pref { get; set; } = -1;

        [UseConverter(typeof(ListConverter<List<RTPref>>))]
        [NonNullable]
        internal virtual List<List<RTPref>> preferredValues_rt { get; set; } = new List<List<RTPref>>();

        /// <summary>
        /// Legacy config value for ReactionTime preferred-values
        /// </summary>
        [UseConverter(typeof(ListConverter<RTPref>))]
        protected virtual List<RTPref> rt_preferredValues { get; set; } = null;

        //1.19.1 Feature update
        internal virtual int slider_setting { get; set; } = 0;
        internal virtual int pref_selected { get; set; } = 0;

        internal virtual int use_heuristic { get; set; } = 0;
        internal virtual float lower_threshold { get; set; } = 1f;
        internal virtual float upper_threshold { get; set; } = 100f;

        internal virtual bool rt_display_enabled { get; set; } = true;
        internal virtual bool legacy_display_enabled { get; set; } = false;

        //1.26.0-1.29.0 Feature update
        internal virtual bool use_offset { get; set; } = false;
        internal virtual float offset_fraction { get; set; } = 8f;

        // 1.29.1
        internal virtual int song_speed_setting { get; set; } = 0;

        internal bool af_enabled { get; set; } = false;


        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        internal virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        /*internal virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }*/
    }

    internal class JDPref
    {
        internal virtual float njs { get; set; } = 16f;
        internal virtual float jumpDistance { get; set; } = 18f;

        public JDPref()
        {
        }

        internal JDPref(float njs, float jumpDistance)
        {
            this.njs = njs;
            this.jumpDistance = jumpDistance;
        }
    }

    // Reaction Time Mode
    internal class RTPref
    {
        internal virtual float njs { get; set; } = 16f;
        internal virtual float reactionTime { get; set; } = 800f;

        public RTPref()
        {
        }

        internal RTPref(float njs, float reactionTime)
        {
            this.njs = njs;
            this.reactionTime = reactionTime;
        }
    }
}