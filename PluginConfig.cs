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
        internal static PluginConfig Instance { get; set; }

        internal virtual bool enabled { get; set; } = false;


        internal float jumpDistance { get; set; } = 24f;
        internal virtual int minJumpDistance { get; set; } = 12;
        internal virtual int maxJumpDistance { get; set; } = 35;
        internal virtual bool use_jd_pref { get; set; } = false;

        [UseConverter(typeof(ListConverter<JDPref>))]
        [NonNullable]
        internal virtual List<JDPref> preferredValues { get; set; } = new List<JDPref>();


        internal float reactionTime { get; set; } = 500f;
        internal virtual int minReactionTime { get; set; } = 300;
        internal virtual int maxReactionTime { get; set; } = 1600;
        internal virtual bool use_rt_pref { get; set; } = false;

        [UseConverter(typeof(ListConverter<RTPref>))]
        [NonNullable]
        internal virtual List<RTPref> rt_preferredValues { get; set; } = new List<RTPref>();


        //1.19.1 Feature update
        internal virtual int slider_setting { get; set; } = 0;
        internal virtual int pref_selected { get; set; } = 0;

        internal virtual int use_heuristic { get; set; } = 0;
        internal float lower_threshold { get; set; } = 1f;
        internal float upper_threshold { get; set; } = 100f;

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
        internal virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }

    internal class JDPref
    {
        internal float njs = 16f;
        internal float jumpDistance = 18f;

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
        internal float njs = 16f;
        internal float reactionTime = 800f;

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
