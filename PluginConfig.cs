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
        public static PluginConfig Instance { get; set; }

        public virtual bool enabled { get; set; } = false;

        
        public virtual float jumpDistance { get; set; } = 24f;
        public virtual float stepJumpDistance { get; set; } = 0.1f;
        public virtual float minJumpDistance { get; set; } = 12;
        public virtual float maxJumpDistance { get; set; } = 35;
        public virtual bool usePreferredJumpDistanceValues { get; set; } = false;

        [UseConverter(typeof(ListConverter<JDPref>))]
        [NonNullable]
        public virtual List<JDPref> preferredValues { get; set; } = new List<JDPref>();


        public virtual float reactionTime { get; set; } = 500f;
        public virtual float stepReactionTime { get; set; } = 5f;
        public virtual float minReactionTime { get; set; } = 300;
        public virtual float maxReactionTime { get; set; } = 1600;
        public virtual bool usePreferredReactionTimeValues { get; set; } = false;

        [UseConverter(typeof(ListConverter<RTPref>))]
        [NonNullable]
        public virtual List<RTPref> rt_preferredValues { get; set; } = new List<RTPref>();


        //1.19.1 Feature update
        public virtual int slider_setting { get; set; } = 0;
        public virtual int pref_selected { get; set; } = 0;

        public virtual int use_heuristic { get; set; } = 0;
        public virtual float lower_threshold { get; set; } = 1f;
        public virtual float upper_threshold { get; set; } = 100f;

        public virtual bool rt_display_enabled { get; set; } = true;
        public virtual bool legacy_display_enabled { get; set; } = false;

        //1.26.0 Feature update
        public virtual bool use_offset { get; set; } = false;
        public virtual float offset_fraction { get; set; } = 8f;
        public bool af_enabled { get; set; } = true;


        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }

    public class JDPref
    {
        public float njs = 16f;
        public float jumpDistance = 24f;

        public JDPref()
        {

        }

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

        public RTPref()
        {

        }

        public RTPref(float njs, float reactionTime)
        {
            this.njs = njs;
            this.reactionTime = reactionTime;
        }
    }
}
