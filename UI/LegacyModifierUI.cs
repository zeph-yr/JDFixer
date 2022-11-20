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
    internal class LegacyModifierUI : IInitializable, IDisposable, INotifyPropertyChanged, IBeatmapInfoUpdater
    {
        internal static LegacyModifierUI Instance { get; set; }
        private readonly MainFlowCoordinator _mainFlow;
        private readonly PreferencesFlowCoordinator _prefFlow;

        public event PropertyChangedEventHandler PropertyChanged;
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;


        public void Initialize()
        {
            //Logger.log.Debug("Legacy Init");

            GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.legacyModifierUI.bsml", this, MenuType.Solo | MenuType.Campaign);
        }

        public void Dispose()
        {
            //Logger.log.Debug("Legacy Dispose");

            if (GameplaySetup.instance != null)
            {
                GameplaySetup.instance.RemoveTab("JDFixer");
            }
        }

        // To get the flow coordinators using zenject, we use a constructor
        public LegacyModifierUI(MainFlowCoordinator mainFlowCoordinator, PreferencesFlowCoordinator preferencesFlowCoordinator)
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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Display)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Display)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_JD_Slider)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_RT_Slider)));

            Logger.log.Debug("Map JD: " + _selectedBeatmap.JumpDistance + " " + _selectedBeatmap.MinJDSlider + " " + _selectedBeatmap.MaxJDSlider);

            BeatmapUtils.Create_JD_Snap_Points(_selectedBeatmap.JumpDistance, _selectedBeatmap.UnitJDOffset, _selectedBeatmap.MinJDSlider, _selectedBeatmap.MaxJDSlider);
            BeatmapUtils.Create_RT_Snap_Points(_selectedBeatmap.ReactionTime, _selectedBeatmap.UnitRTOffset, _selectedBeatmap.MinRTSlider, _selectedBeatmap.MaxRTSlider);

            PostParse();
        }

        internal void Refresh()
        {
            //Logger.log.Debug("LegacyUI Refresh");

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Slider_Setting_Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Increment_Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pref_Button)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Heuristic_Increment_Value)));
        }


        //=============================================================================================


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

        private string Get_Map_Default_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.##");
        }


        [UIValue("map_min_jd")]
        private string Map_Min_JD => Get_Map_Min_JD();

        private string Get_Map_Min_JD()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.##");
        }


        [UIValue("snapped_jd")]
        private string Snapped_JD => Get_Snapped_JD();

        private string Get_Snapped_JD()
        {
            return "<#ffff00>" + BeatmapUtils.Calculate_JumpDistance_Nearest_Offset(JD_Value).ToString("0.##");
        }
        [UIValue("show_snapped_jd")]
        private bool Show_Snapped_JD => PluginConfig.Instance.use_offset && PluginConfig.Instance.slider_setting == 0;


        [UIValue("snapped_rt")]
        private string Snapped_RT => Get_Snapped_RT();

        private string Get_Snapped_RT()
        {
            return "<#8c8c8c>" + BeatmapUtils.Calculate_ReactionTime_Nearest_Offset(RT_Value).ToString("0") + " ms";
        }
        [UIValue("show_snapped_rt")]
        private bool Show_Snapped_RT => PluginConfig.Instance.use_offset && PluginConfig.Instance.slider_setting == 1;


        //=============================================================================================


        [UIValue("min_jd_slider")]
        private float Min_JD_Slider => PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private float Max_JD_Slider => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        private SliderSetting JD_Slider;

        [UIValue("jd_value")]
        private float JD_Value
        {
            get => PluginConfig.Instance.jumpDistance;
            set
            {
                PluginConfig.Instance.jumpDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Display)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_JD)));
            }
        }
        [UIAction("set_jd_value")]
        private void Set_JD_Value(float value)
        {
            JD_Value = value;
        }
        [UIAction("jd_slider_formatter")]
        private string JD_Slider_Formatter(float value) => value.ToString("0.##");


        [UIValue("jd_display")]
        private string JD_Display => BeatmapUtils.Calculate_JumpDistance_Setpoint_String(RT_Value, _selectedBeatmap.NJS); //"<#ffff00>" + (PluginConfig.Instance.reactionTime * (2 * _selectedBeatmap.NJS) / 1000).ToString("0.##");
        


        [UIValue("min_rt_slider")]
        private float Min_RT_Slider => PluginConfig.Instance.minReactionTime;

        [UIValue("max_rt_slider")]
        private float Max_RT_Slider => PluginConfig.Instance.maxReactionTime;

        [UIComponent("rt_slider")]
        private SliderSetting RT_Slider;

        [UIValue("rt_value")]
        private float RT_Value
        {
            get => PluginConfig.Instance.reactionTime;
            set
            {
                PluginConfig.Instance.reactionTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Display)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_RT)));
            }
        }
        [UIAction("set_rt_value")]
        private void Set_RT_Value(float value)
        {
            RT_Value = value;
        }
        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";


        [UIValue("rt_display")]
        public string RT_Display => BeatmapUtils.Calculate_ReactionTime_Setpoint_String(JD_Value, _selectedBeatmap.NJS);



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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_JD)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_RT)));
        }


        //1.20.0 Feature update
        [UIValue("slider_setting_value")]
        private int Slider_Setting_Value
        {
            get => PluginConfig.Instance.slider_setting;
            set
            {
                PluginConfig.Instance.slider_setting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Slider_Setting_Value)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Value)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Value)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JD_Display)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RT_Display)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_JD_Slider)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_RT_Slider)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_JD)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Snapped_RT)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_Snapped_JD)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Show_Snapped_RT)));
            }
        }
        [UIAction("slider_setting_increment_formatter")]
        private string Slider_Setting_Increment_Formatter(int value) => ((SliderSettingEnum)value).ToString();


        [UIValue("show_jd_slider")]
        private bool Show_JD_Slider => Get_JD_Slider();

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
        private bool Show_RT_Slider => Get_RT_Slider();

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