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
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##");
        }


        [UIValue("map_min_jd")]
        public string Map_Min_JD => Get_Map_Min_JD();
        //public string MapMinJDText => "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

        public string Get_Map_Min_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##");
        }


        [UIValue("min_jd_slider")]
        private float Min_JD_Slider => _selectedBeatmap.MinJDSlider; //PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private float Max_JD_Slider => _selectedBeatmap.MaxJDSlider; //PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        public SliderSetting JD_Slider;

        [UIValue("jd_value")]
        public float JD_Value
        {
            get => Get_Jump_Distance(); //PluginConfig.Instance.jumpDistance; //GetJumpDistance();
            set
            {
                if (PluginConfig.Instance.slider_setting == 0)
                {
                    PluginConfig.Instance.jumpDistance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));
                }
                else
                {
                    //PluginConfig.Instance.jumpDistance = value; // Must set here too or it will not run in patch
                    PluginConfig.Instance.reactionTime = value / (2 * _selectedBeatmap.NJS) * 1000;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));
                }

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


        // 1.19.1
        public float Get_Jump_Distance()
        {
            if (PluginConfig.Instance.slider_setting == 0)
            {
                return PluginConfig.Instance.jumpDistance;
            }
            else
            {
                return PluginConfig.Instance.reactionTime * (2 * _selectedBeatmap.NJS) / 1000;
            }
        }

        [UIAction("jd_slider_formatter")]
        private string JD_Slider_Formatter(float value) => value.ToString("0.##");
        [UIAction("rt_slider_formatter")]


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
            get => Get_Reaction_Time(); //CalculateReactionTime_Float(PluginConfig.Instance.jumpDistance);
            set
            {
                if (PluginConfig.Instance.slider_setting == 0) // Fixed JD
                {
                    if (_selectedBeatmap.NJS > 0.002)
                    {
                        PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
                    }
                }
                else
                {
                    //PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS); // Must set here or will not run in patch
                    PluginConfig.Instance.reactionTime = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
                }

                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For validation               
            }
        }

        // 1.19.1
        public float Get_Reaction_Time()
        {
            if (PluginConfig.Instance.slider_setting == 0)
            {
                return CalculateReactionTime_Float(PluginConfig.Instance.jumpDistance);
            }
            else
            {
                return PluginConfig.Instance.reactionTime;
            }
        }


        [UIAction("set_rt_value")]
        public void Set_RT_Value(float value)
        {
            RT_Value = value;
        }

        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";


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
        public HMUI.CustomFormatRangeValuesSlider jd_slider_range;

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


            jd_slider_range = JD_Slider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();

            jd_slider_range.minValue = _selectedBeatmap.MinJDSlider;
            jd_slider_range.maxValue = _selectedBeatmap.MaxJDSlider;

            
            // These are critical:
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));

            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
        }


        //1.19.1 Feature update
        [UIValue("fixed_slider_value")]
        private int Fixed_Slider_Value
        {
            get => PluginConfig.Instance.slider_setting;
            set
            {
                PluginConfig.Instance.slider_setting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fixed_Slider_Value)));

                //BeatmapUtils.RefreshSliderMinMax(_selectedBeatmap.NJS);
                PostParse();

                RefreshSliderMinMax();

            }
        }

        [UIAction("fixed_slider_increment_formatter")]
        private string Fixed_Slider_Increment_Formatter(int value) => ((FixedSliderEnum)value).ToString();



        public void RefreshSliderMinMax()
        {
            rt_slider_range = RT_Slider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();
            jd_slider_range = JD_Slider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();


            if (PluginConfig.Instance.slider_setting == 0)
            {
                rt_slider_range.minValue = PluginConfig.Instance.minJumpDistance * 500 / _selectedBeatmap.NJS;
                rt_slider_range.maxValue = PluginConfig.Instance.maxJumpDistance * 500 / _selectedBeatmap.NJS;

                jd_slider_range.minValue = PluginConfig.Instance.minJumpDistance;
                jd_slider_range.maxValue = PluginConfig.Instance.maxJumpDistance;
            }
            else
            {
                rt_slider_range.minValue = PluginConfig.Instance.minReactionTime;
                rt_slider_range.maxValue = PluginConfig.Instance.maxReactionTime;

                jd_slider_range.minValue = PluginConfig.Instance.minReactionTime * _selectedBeatmap.NJS / 500;
                jd_slider_range.maxValue = PluginConfig.Instance.maxReactionTime * _selectedBeatmap.NJS / 500;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));


            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
        }



    }

    public enum FixedSliderEnum
    {
        JD = 0,
        RT = 1
    }


    public enum PreferenceEnum
    {
        Off = 0,
        JumpDistance = 1,
        ReactionTime = 2
    }
}
