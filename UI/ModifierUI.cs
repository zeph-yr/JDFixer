using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using Zenject;
using System;
using System.ComponentModel;
using JDFixer.Interfaces;
using UnityEngine;

namespace JDFixer.UI
{
    public class ModifierUI : IInitializable, IDisposable, INotifyPropertyChanged, IBeatmapInfoUpdater
    {
        internal static ModifierUI Instance { get; set; }
        private readonly MainFlowCoordinator _mainFlow;
        private readonly PreferencesFlowCoordinator _prefFlow;

        public event PropertyChangedEventHandler PropertyChanged;
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;


        public void Initialize()
        {
            //Logger.log.Debug("ModUI Init");

            GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.modifierUI.bsml", this, MenuType.Solo | MenuType.Campaign);
        }

        public void Dispose()
        {
            //Logger.log.Debug("ModUI Dispose");

            if (GameplaySetup.instance != null)
            {
                GameplaySetup.instance.RemoveTab("JDFixer");
            }
        }
        
        // To get the flow coordinators using zenject, we use a constructor
        public ModifierUI(MainFlowCoordinator mainFlowCoordinator, PreferencesFlowCoordinator preferencesFlowCoordinator)
        {
            Instance = this;
            _mainFlow = mainFlowCoordinator;
            _prefFlow = preferencesFlowCoordinator;
        }

        public void BeatmapInfoUpdated(BeatmapInfo beatmapInfo)
        {
            _selectedBeatmap = beatmapInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Default_JD)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Min_JD)));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_JD_Slider)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_RT_Slider)));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For old RT Display

            //Logger.log.Debug("Map JD: " + _selectedBeatmap.JumpDistance + " " + _selectedBeatmap.MinJDSlider + " " + _selectedBeatmap.MaxJDSlider);
            //BeatmapUtils.Create_RT_Snap_Points(_selectedBeatmap.JumpDistance, _selectedBeatmap.JDOffsetQuantum, _selectedBeatmap.MinJDSlider, _selectedBeatmap.MaxJDSlider);

            PostParse();
        }

        internal void Refresh()
        {
            //Logger.log.Debug("ModUI Refresh");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Slider_Setting_Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Increment_Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pref_Button)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Heuristic_Increment_Value)));
        }


        //=============================================================================================

        #region UI_Common
        [UIValue("enabled")]
        private bool Enabled
        {
            get => PluginConfig.Instance.enabled;
            set
            {
                PluginConfig.Instance.enabled = value;
            }
        }
        [UIAction("set_enabled")]
        private void SetEnabled(bool value)
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


        [UIValue("map_jd_rt")]
        private string Map_JD_RT => Get_Map_JD_RT();

        private string Get_Map_JD_RT()
        {
            if (PluginConfig.Instance.rt_display_enabled)
            {
                return "Map JD and RT";
            }
            return "Map JD";
        }


        [UIValue("map_default_jd")]
        private string Map_Default_JD => Get_Map_Default_JD();
        //public string MapDefaultJDText => "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0.#") + " ms";

        private string Get_Map_Default_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##");
        }


        [UIValue("map_min_jd")]
        private string Map_Min_JD => Get_Map_Min_JD();
        //public string MapMinJDText => "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

        private string Get_Map_Min_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##");
        }
        #endregion

        
        #region UI_JD
        /*[UIValue("step_jd_slider")]
        private float Step_JD_Slider => Get_Step_JD();

        private float Get_Step_JD()
        {
            if (PluginConfig.Instance.use_offset)
            {
                Logger.log.Debug("JD:" + _selectedBeatmap.JDOffsetQuantum);
                return _selectedBeatmap.JDOffsetQuantum;
            }

            else if (PluginConfig.Instance.slider_setting == 0)
            {
                Logger.log.Debug("step JD: " + PluginConfig.Instance.stepJumpDistance);
                return PluginConfig.Instance.stepJumpDistance;
            }

            // slider_setting == 1:
            return BeatmapUtils.Get_JD_Increment_For_RT(PluginConfig.Instance.reactionTime, _selectedBeatmap.MinRTSlider, _selectedBeatmap.MaxRTSlider, _selectedBeatmap.MinJDSlider, _selectedBeatmap.MaxJDSlider);
        }

        private int Get_JD_Num_Steps()
        {
            if (PluginConfig.Instance.use_offset)
            {
                return BeatmapUtils.Get_Num_Snap_Points();
            }

            return BeatmapUtils.Get_JD_NumberOfSteps(Step_JD_Slider, _selectedBeatmap.MinJDSlider, _selectedBeatmap.MaxJDSlider);
        }*/

        [UIValue("min_jd_slider")]
        private float Min_JD_Slider => _selectedBeatmap.MinJDSlider; //PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private float Max_JD_Slider => _selectedBeatmap.MaxJDSlider; //PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        private SliderSetting JD_Slider;

        [UIValue("jd_value")]
        private float JD_Value
        {
            get => Get_Jump_Distance(); //PluginConfig.Instance.jumpDistance; //GetJumpDistance();
            set
            {
                /*if (PluginConfig.Instance.use_offset)
                {
                    PluginConfig.Instance.jumpDistance = BeatmapUtils.Calculate_ReactionTime_Nearest_Offset(value);
                }

                else*/
                if (PluginConfig.Instance.slider_setting == 0)
                {
                    PluginConfig.Instance.jumpDistance = value;
                }
                else
                {
                    if (_selectedBeatmap.NJS > 0.002)
                    {
                        PluginConfig.Instance.reactionTime = value / (2 * _selectedBeatmap.NJS) * 1000;
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For old RT Display                
            }
        }

        /*private float GetJumpDistance()
        {
            return PluginConfig.Instance.jumpDistance;
        }*/

        // 1.19.1
        private float Get_Jump_Distance()
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

        [UIAction("set_jd_value")]
        private void Set_JD_Value(float value)
        {
            JD_Value = value;
        }

        [UIAction("jd_slider_formatter")]
        private string JD_Slider_Formatter(float value) => value.ToString("0.##");
        #endregion


        #region RT_UI
        /*[UIValue("step_rt_slider")]
        private float Step_RT_Slider => Get_Step_RT();

        private float Get_Step_RT()
        {
            try
            {
                if (PluginConfig.Instance.use_offset)
                {
                    Logger.log.Debug("RT: " + _selectedBeatmap.RTOffsetQuantum);
                    return _selectedBeatmap.RTOffsetQuantum;
                }
                Logger.log.Debug("RT:" + PluginConfig.Instance.stepReactionTime);
                return PluginConfig.Instance.stepReactionTime;
            }
            catch (Exception)
            {
                Logger.log.Debug("RT:" + PluginConfig.Instance.stepReactionTime);
                return PluginConfig.Instance.stepReactionTime;
            }*/


            /*if (PluginConfig.Instance.slider_setting == 1)
            {
                Logger.log.Debug("Step RT: " + PluginConfig.Instance.stepReactionTime);
                return PluginConfig.Instance.stepReactionTime;
            }
            else
            {
                Logger.log.Debug("Step RT: " + (Max_RT_Slider - Min_RT_Slider) * PluginConfig.Instance.stepJumpDistance / (Max_JD_Slider - Min_JD_Slider));
                //return BeatmapUtils.Calculate_ReactionTime_Setpoint_Float(PluginConfig.Instance.stepJumpDistance, _selectedBeatmap.NJS);
                return (Max_RT_Slider - Min_RT_Slider) * PluginConfig.Instance.stepJumpDistance / (Max_JD_Slider - Min_JD_Slider);
            }*/
        //}

        [UIValue("min_rt_slider")]
        private float Min_RT_Slider => _selectedBeatmap.MinRTSlider; //Get_Min_RT();

        [UIValue("max_rt_slider")]
        private float Max_RT_Slider => _selectedBeatmap.MaxRTSlider; //Get_Max_RT();

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
        private SliderSetting RT_Slider;

        [UIValue("rt_value")]
        private float RT_Value
        {
            get => Get_Reaction_Time(); //CalculateReactionTime_Float(PluginConfig.Instance.jumpDistance);
            set
            {
                if (PluginConfig.Instance.slider_setting == 0) // Fixed JD
                {
                    if (_selectedBeatmap.NJS > 0.002)
                    {
                        PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS);
                    }
                }
                else
                {
                    PluginConfig.Instance.reactionTime = value;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For validation               
            }
        }

        // 1.19.1
        private float Get_Reaction_Time()
        {
            if (PluginConfig.Instance.slider_setting == 0)
            {
                return BeatmapUtils.Calculate_ReactionTime_Setpoint_Float(PluginConfig.Instance.jumpDistance, _selectedBeatmap.NJS);
            }
            else
            {
                return PluginConfig.Instance.reactionTime;
            }
        }

        [UIAction("set_rt_value")]
        private void Set_RT_Value(float value)
        {
            RT_Value = value;
        }

        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";
        #endregion

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
                PluginConfig.Instance.usePreferredReactionTimeValues = true;
            }
            else if (PluginConfig.Instance.pref_selected == 1)
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = true;
                PluginConfig.Instance.usePreferredReactionTimeValues = false;
            }
            else
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = false;
                PluginConfig.Instance.usePreferredReactionTimeValues = false;
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
        private string Pref_Button => Get_Pref_Button();

        private string Get_Pref_Button()
        {
            if (PluginConfig.Instance.pref_selected == 2)
            {
                return "<#00000000>----<#cc99ff>Configure  RT  Preferences<#00000000>----"; //#8c1aff
            }
            else if (PluginConfig.Instance.pref_selected == 1)
            {
                return "<#00000000>----<#ffff00>Configure  JD  Preferences<#00000000>----";
            }
            else
            {
                return "Configure  JD  and  RT  Preferences";
            }
        }

        [UIAction("pref_button_clicked")]
        private void Pref_Button_Clicked()
        {
            /* Kyle used to have a helper function which you also used (DeepestChildFlowCoordinator). 
             * Beat Games has added this to the game since, so we can just use something they helpfully provided us
             */
            FlowCoordinator currentFlow = _mainFlow.YoungestChildFlowCoordinatorOrSelf();
            // We need to give our current flow coordinator to the pref flow so it can exit
            _prefFlow._parentFlow = currentFlow;
            currentFlow.PresentFlowCoordinator(_prefFlow);
        }


        // Changed to Increment Setting for 1.26.0
        // <checkbox-setting value='use_heuristic' on-change='set_use_heuristic' text='Play at Map JD and RT If Lower' hover-hint='If original JD and RT is lower than the matching preference, map will run at original JD and RT. You MUST set base game to Dynamic Default for this to work properly!'></checkbox-setting>

        /*[UIValue("use_heuristic")]
        private bool Use_Heuristic
        {
            get => PluginConfig.Instance.use_heuristic;
            set
            {
                PluginConfig.Instance.use_heuristic = value;
            }
        }

        [UIAction("set_use_heuristic")]
        private void Set_Use_Heuristic(bool value)
        {
            Use_Heuristic = value;
        }*/

        [UIValue("heuristic_increment_value")]
        private int Heuristic_Increment_Value
        {
            get => PluginConfig.Instance.use_heuristic;
            set
            {
                PluginConfig.Instance.use_heuristic = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Heuristic_Increment_Value)));

                PostParse();
            }
        }

        [UIAction("heuristic_increment_formatter")]
        private string Heuristic_Increment_Formatter(int value) => ((HeuristicEnum)value).ToString();


        [UIValue("thresholds")]
        private string Thresholds
        {
            get => "≤ " + PluginConfig.Instance.lower_threshold.ToString() + " or  ≥ " + PluginConfig.Instance.upper_threshold.ToString();
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


        private CurvedTextMeshPro jd_slider_text;
        private CurvedTextMeshPro rt_slider_text;

        private HMUI.CustomFormatRangeValuesSlider rt_slider_range;
        private HMUI.CustomFormatRangeValuesSlider jd_slider_range;

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


            /*if (PluginConfig.Instance.use_offset)
            {
                JD_Slider.increments = Step_JD_Slider;
                RT_Slider.increments = Step_RT_Slider;

                rt_slider_range.numberOfSteps = Get_JD_Num_Steps();
                jd_slider_range.numberOfSteps = Get_JD_Num_Steps();
            }*/


            // These are critical:
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
        }


        //1.19.1 Feature update
        [UIValue("slider_setting_value")]
        private int Slider_Setting_Value
        {
            get => PluginConfig.Instance.slider_setting;
            set
            {
                PluginConfig.Instance.slider_setting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Slider_Setting_Value)));

                // This doesnt work because the MinRTSlider etc can't be publically set, crashes
                //BeatmapUtils.RefreshSliderMinMax(_selectedBeatmap.NJS);

                // This is critcal!
                RefreshSliderMinMax();
            }
        }

        [UIAction("slider_setting_increment_formatter")]
        private string Slider_Setting_Increment_Formatter(int value) => ((SliderSettingEnum)value).ToString();


        // This function is critical:
        // Without this function, when slider setting is flipped, the slider min maxes will be wrong because they are/were set in BeatmapInfo
        // Ex: When JD flips to RT, sliders will be draw as if set to JD (with JD min-max) until a new map is clicked that triggers BeatmapInfo
        // and PostParse to run again with the new setting.
        // Must "recalculate" them here then trigger everything to update
        private void RefreshSliderMinMax()
        {
            Logger.log.Debug("Refresh Slider Min Max");
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

            /*if (PluginConfig.Instance.use_offset)
            {
                JD_Slider.increments = Step_JD_Slider;
                RT_Slider.increments = Step_RT_Slider;

                rt_slider_range.numberOfSteps = Get_JD_Num_Steps();
                jd_slider_range.numberOfSteps = Get_JD_Num_Steps();
            }*/

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_RT_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Step_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Min_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Max_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));            
        }
    }


    internal enum SliderSettingEnum
    {
        JumpDistance = 0,
        ReactionTime = 1
    }


    internal enum PreferenceEnum
    {
        Off = 0,
        JumpDistance = 1,
        ReactionTime = 2
    }

    internal enum HeuristicEnum
    {
        Off = 0,
        On = 1
    }
}

/*		<vertical vertical-fit='PreferredSize' preferred-height='14'>
			<slider-setting id='jd_slider' value ='jd_value' show-buttons='true' on-change='set_jd_value' apply-on-change='true' bind-value='true' text='Desired Jump Distance' increment='~step_jd_slider' min='~min_jd_slider' max='~max_jd_slider' formatter='jd_slider_formatter'></slider-setting>
			<slider-setting id='rt_slider' value ='rt_value' show-buttons='true' on-change='set_rt_value' apply-on-change='true' bind-value='true' text='Reaction Time' increment='~step_rt_slider' min='~min_rt_slider' max='~max_rt_slider' formatter='rt_slider_formatter'></slider-setting>
		</vertical>*/
