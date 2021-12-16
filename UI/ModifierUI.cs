using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using Zenject;
using System;
using System.ComponentModel;
using JDFixer.Interfaces;


namespace JDFixer.UI
{
    public class ModifierUI : IInitializable, IDisposable, INotifyPropertyChanged, IBeatmapInfoUpdater
    {
        private readonly MainFlowCoordinator _mainFlow;
        private readonly PreferencesFlowCoordinator _prefFlow;

        public event PropertyChangedEventHandler PropertyChanged;
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;


        public void Initialize()
        {
            GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.modifierUI.bsml", this);
        }

        public void Dispose()
        {
            if (GameplaySetup.instance != null)
            {
                GameplaySetup.instance.RemoveTab("JDFixer");
            }
        }
        
        // To get the flow coordinators using zenject, we use a constructor
        public ModifierUI(MainFlowCoordinator mainFlowCoordinator, PreferencesFlowCoordinator preferencesFlowCoordinator)
        {
            _mainFlow = mainFlowCoordinator;
            _prefFlow = preferencesFlowCoordinator;
        }

        public void BeatmapInfoUpdated(BeatmapInfo beatmapInfo)
        {
            _selectedBeatmap = beatmapInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Default_JD)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Min_JD)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For old RT Display

            PostParse();
        }

        private string CalculateReactionTime_String()
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return "<#cc99ff>" + (JD_Value / (2 * _selectedBeatmap.NJS) * 1000).ToString("0.#") + " ms";

            return "<#cc99ff>0 ms";
        }

        public float CalculateReactionTime_Float(float jd)
        {
            if (_selectedBeatmap.NJS > 0.002)
                return jd / (2 * _selectedBeatmap.NJS) * 1000;

            return 0f;
        }

        //=============================================================================================


        [UIValue("enabled")]
        public bool Enabled
        {
            get => PluginConfig.Instance.enabled;
            set
            {
                PluginConfig.Instance.enabled = value;
            }
        }
        [UIAction("set_enabled")]
        public void SetEnabled(bool value)
        {
            Enabled = value;
        }


        //----------------------------------------------------
        // KEEP: In case users want this back

        /*[UIValue("practiceEnabled")]
        public bool practiceEnabled
        {
            get => PluginConfig.Instance.enabledInPractice;
            set
            {
                PluginConfig.Instance.enabledInPractice = value;
            }
        }
        [UIAction("setPracticeEnabled")]
        void SetPracticeEnabled(bool value)
        {
            practiceEnabled = value;
        }*/
        //----------------------------------------------------


        [UIValue("map_default_jd")]
        public string Map_Default_JD => Get_Map_Default_JD();
        //public string MapDefaultJDText => "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0.#") + " ms";

        public string Get_Map_Default_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0.#") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###");
        }


        [UIValue("map_min_jd")]
        public string Map_Min_JD => Get_Map_Min_JD();
        //public string MapMinJDText => "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

        public string Get_Map_Min_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###");
        }


        [UIValue("min_jd_slider")]
        private int Min_JD_Slider => PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private int Max_JD_Slider => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        public SliderSetting JD_Slider;

        [UIValue("jd_value")]
        public float JD_Value
        {
            get => PluginConfig.Instance.jumpDistance; //GetJumpDistance();
            set
            {
                PluginConfig.Instance.jumpDistance = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For old RT Display
            }
        }
        [UIAction("set_jd_value")]
        public void Set_JD_Value(float value)
        {
            JD_Value = value;
            //PostParse();
        }

        /*private float GetJumpDistance()
        {
            return PluginConfig.Instance.jumpDistance;
        }*/



        [UIValue("min_rt_slider")]
        public float Min_RT_Slider => _selectedBeatmap.MinRTSlider; //Get_Min_RT();

        [UIValue("max_rt_slider")]
        public float Max_RT_Slider => _selectedBeatmap.MaxRTSlider; //Get_Max_RT();

        /*public float Get_Min_RT()
        {
            return _selectedBeatmap.MinRTSlider;
        }

        public float Get_Max_RT()
        {
            return _selectedBeatmap.MaxRTSlider;
        }*/


        //=============================================================
        // Old Reaction Time Display: Replaced by RT Slider (KEEP THIS)

        //[UIValue("reactionTime")]
        //public string ReactionTimeText => CalculateReactionTime();

        //<horizontal>
        //  <grid cell-size-y='5' cell-size-x='28' spacing-x='2' align='Right'>
        //	  <text text='Reaction Time' align='Left'/>
        //	  <text text='----------------' align='Left' rich-text='true' font-color='#00000000'/>
        //	  <text text='~reactionTime' min-width='29' align='Right'/>
        //  </grid>
        //</horizontal>

        //=============================================================

        [UIComponent("rt_slider")]
        public SliderSetting RT_Slider;

        [UIValue("rt_value")]
        public float RT_Value
        {
            get => CalculateReactionTime_Float(PluginConfig.Instance.jumpDistance);
            set
            {
                PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));

                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For validation               
            }
        }

        [UIAction("set_rt_value")]
        public void Set_RT_Value(float value)
        {
            RT_Value = value;
        }


        //##############################################
        // New for BS 1.19.0

        [UIValue("increment_value")]
        private int Increment_Value
        {
            get => PluginConfig.Instance.pref_selected;
            set
            {
                PluginConfig.Instance.pref_selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Increment_Value)));
                Set_Preference_Mode();
            }
        }

        [UIAction("increment_formatter")]
        private string Increment_Formatter(int value) => ((PreferenceEnum)value).ToString();


        private void Set_Preference_Mode()
        {
            if (PluginConfig.Instance.pref_selected == 2)
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = false;
                PluginConfig.Instance.rt_enabled = true;
            }
            else if (PluginConfig.Instance.pref_selected == 1)
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = true;
                PluginConfig.Instance.rt_enabled = false;
            }
            else
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = false;
                PluginConfig.Instance.rt_enabled = false;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pref_Button)));
        }
        //##############################################


        //=============================================================
        // Old JD Preferences and RT Preferences Toggles: Replaced with Increment Setting

        //<checkbox-setting value = 'usePrefJumpValues' on-change='setUsePrefJumpValues' text='Use JD Preferences'></checkbox-setting>
        //<checkbox-setting value = 'rtEnabled' on-change='setRTEnabled' text='Use RT Preferences' hover-hint='Overrides JD Preferences if enabled'></checkbox-setting>

        /*[UIValue("usePrefJumpValues")]
        public bool usePrefJumpValues
        {
            get => PluginConfig.Instance.usePreferredJumpDistanceValues;
            set
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = value;
            }
        }
        [UIAction("setUsePrefJumpValues")]
        public void SetUsePrefJumpValues(bool value)
        {
            usePrefJumpValues = value;

            //if (value)
            //{
            //    PluginConfig.Instance.rt_enabled = false;
            //    NotifyPropertyChanged(nameof(RTEnabled));
            //}
        }


        // Reaction Time Mode
        [UIValue("rtEnabled")]
        public bool RTEnabled
        {
            get => PluginConfig.Instance.rt_enabled;
            set
            {
                PluginConfig.Instance.rt_enabled = value;
            }
        }
        [UIAction("setRTEnabled")]
        public void SetRTEnabled(bool value)
        {
            RTEnabled = value;

            //if (value)
            //{
            //    PluginConfig.Instance.usePreferredJumpDistanceValues = false;
            //    NotifyPropertyChanged(nameof(usePrefJumpValues));
            //}
        }*/
        //=============================================================

        [UIValue("pref_button")]
        public string Pref_Button => Get_Pref_Button();

        public string Get_Pref_Button()
        {
            if (PluginConfig.Instance.pref_selected == 2)
            {
                return "<#cc99ff>JD and RT Preferences"; //#8c1aff
            }

            else if (PluginConfig.Instance.pref_selected == 1)
            {
                return "<#ffff00>JD and RT Preferences";
            }

            else
                return "JD and RT Preferences";
        }


        [UIAction("pref_button_clicked")]
        public void Pref_Button_Clicked()
        {
            /* Kyle used to have a helper function which you also used (DeepestChildFlowCoordinator). 
             * Beat Games has added this to the game since, so we can just use something they helpfully provided us
             */
            FlowCoordinator currentFlow = _mainFlow.YoungestChildFlowCoordinatorOrSelf();
            // We need to give our current flow coordinator to the pref flow so it can exit
            _prefFlow._parentFlow = currentFlow;
            currentFlow.PresentFlowCoordinator(_prefFlow);
        }

        
        [UIValue("use_heuristic")]
        public bool Use_Heuristic
        {
            get => PluginConfig.Instance.use_heuristic;
            set
            {
                PluginConfig.Instance.use_heuristic = value;
            }
        }
        [UIAction("set_use_heuristic")]
        public void Set_Use_Heuristic(bool value)
        {
            Use_Heuristic = value;
        }


        [UIValue("thresholds")]
        public string Thresholds
        {
            get => "≤ " + PluginConfig.Instance.lower_threshold.ToString() + " and " + PluginConfig.Instance.upper_threshold.ToString() + " ≤";
        }


        //###################################
        // KEEP: In case
        /*[UIValue("lowerthreshold")]
        public string lowerthreshold
        {
            get => PluginConfig.Instance.lower_threshold.ToString();
        }

        // Thresholds Display
        [UIValue("upperthreshold")]
        public string upperthreshold
        {
            get => PluginConfig.Instance.upper_threshold.ToString();
        }*/
        //###################################


        public CurvedTextMeshPro jd_slider_text;
        public CurvedTextMeshPro rt_slider_text;

        public HMUI.CustomFormatRangeValuesSlider rt_slider_range;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            jd_slider_text = JD_Slider.slider.GetComponentInChildren<CurvedTextMeshPro>();

            if (jd_slider_text != null)
            {
                jd_slider_text.color = new UnityEngine.Color(1f, 1f, 0f);
            }

            rt_slider_text = RT_Slider.slider.GetComponentInChildren<CurvedTextMeshPro>();

            if (rt_slider_text != null)
            {
                rt_slider_text.color = new UnityEngine.Color(204f/255f, 153f/255f, 1f);
            }


            rt_slider_range = RT_Slider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();
            
            rt_slider_range.minValue = _selectedBeatmap.MinRTSlider;
            rt_slider_range.maxValue = _selectedBeatmap.MaxRTSlider;
            
            // These are critical:
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));
        }
    }

    public enum PreferenceEnum
    {
        Off = 0,
        JumpDistance = 1,
        ReactionTime = 2
    }
}
