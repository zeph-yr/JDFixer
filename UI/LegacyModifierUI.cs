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
    public class LegacyModifierUI : IInitializable, IDisposable, INotifyPropertyChanged, IBeatmapInfoUpdater
    {
        private readonly MainFlowCoordinator _mainFlow;
        private readonly PreferencesFlowCoordinator _prefFlow;

        public event PropertyChangedEventHandler PropertyChanged;
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;


        public void Initialize()
        {
            GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.legacyModifierUI.bsml", this, MenuType.Solo | MenuType.Campaign);
        }

        public void Dispose()
        {
            if (GameplaySetup.instance != null)
            {
                GameplaySetup.instance.RemoveTab("JDFixer");
            }
        }

        // To get the flow coordinators using zenject, we use a constructor
        public LegacyModifierUI(MainFlowCoordinator mainFlowCoordinator, PreferencesFlowCoordinator preferencesFlowCoordinator)
        {
            _mainFlow = mainFlowCoordinator;
            _prefFlow = preferencesFlowCoordinator;
        }

        public void BeatmapInfoUpdated(BeatmapInfo beatmapInfo)
        {
            _selectedBeatmap = beatmapInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Default_JD)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map_Min_JD)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Display)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Display)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_RT_Slider)));

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


        [UIValue("map_default_jd")]
        public string Map_Default_JD => Get_Map_Default_JD();

        public string Get_Map_Default_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##");
        }


        [UIValue("map_min_jd")]
        public string Map_Min_JD => Get_Map_Min_JD();

        public string Get_Map_Min_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##");
        }

        //=============================================================================================


        [UIValue("min_jd_slider")]
        private float Min_JD_Slider => PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private float Max_JD_Slider => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        public SliderSetting JD_Slider;

        [UIValue("jd_value")]
        public float JD_Value
        {
            get => PluginConfig.Instance.jumpDistance;
            set
            {
                PluginConfig.Instance.jumpDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Display)));
            }
        }
        [UIAction("set_jd_value")]
        public void Set_JD_Value(float value)
        {
            JD_Value = value;
        }
        [UIAction("jd_slider_formatter")]
        private string JD_Slider_Formatter(float value) => value.ToString("0.##");


        [UIValue("jd_display")]
        public string JD_Display => "<#ffff00>" + (PluginConfig.Instance.reactionTime * (2 * _selectedBeatmap.NJS) / 1000).ToString("0.##");
        


        [UIValue("min_rt_slider")]
        public float Min_RT_Slider => PluginConfig.Instance.minReactionTime;

        [UIValue("max_rt_slider")]
        public float Max_RT_Slider => PluginConfig.Instance.maxReactionTime;

        [UIComponent("rt_slider")]
        public SliderSetting RT_Slider;

        [UIValue("rt_value")]
        public float RT_Value
        {
            get => PluginConfig.Instance.reactionTime;
            set
            {
                PluginConfig.Instance.reactionTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Display)));
            }
        }
        [UIAction("set_rt_value")]
        public void Set_RT_Value(float value)
        {
            RT_Value = value;
        }
        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";


        [UIValue("rt_display")]
        public string RT_Display => "<#cc99ff>" + CalculateReactionTime_Float(PluginConfig.Instance.jumpDistance).ToString("0");



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
            {
                return "JD and RT Preferences";
            }
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
                rt_slider_text.color = new UnityEngine.Color(204f / 255f, 153f / 255f, 1f);
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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_RT_Slider)));
        }

        [UIValue("show_jd_slider")]
        bool Show_JD_Slider => Get_JD_Slider();

        private bool Get_JD_Slider()
        {
            if (PluginConfig.Instance.slider_setting == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [UIValue("show_rt_slider")]
        bool Show_RT_Slider => Get_RT_Slider();

        private bool Get_RT_Slider()
        {
            if (PluginConfig.Instance.slider_setting == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}