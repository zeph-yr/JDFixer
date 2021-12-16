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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapDefaultJDText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapMinJDText)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(jumpDisValue))); // necessary

            //PluginConfig.Instance.minReactionTime = CalculateReactionTime_2(PluginConfig.Instance.minJumpDistance);
            //PluginConfig.Instance.maxReactionTime = CalculateReactionTime_2(PluginConfig.Instance.maxJumpDistance);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue))); // necessary
            PostParse();
        }


        private string CalculateReactionTime()
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return "<#cc99ff>" + (jumpDisValue / (2 * _selectedBeatmap.NJS) * 1000).ToString("0.#") + " ms";

            return "<#cc99ff>0 ms";
        }

        public float Get_Min_RT()
        {
            return _selectedBeatmap.MinRTSlider; //PluginConfig.Instance.minReactionTime;
        }

        public float Get_Max_RT()
        {
            return _selectedBeatmap.MaxRTSlider; //PluginConfig.Instance.maxReactionTime;
        }

        public float CalculateReactionTime_2(float jd)
        {
            if (_selectedBeatmap.NJS > 0.002)
                return jd / (2 * _selectedBeatmap.NJS) * 1000;

            return 0f;
        }


        //=============================================================================================


        [UIValue("minRT")]
        public float minRT => Get_Min_RT();
        /*{
            get => /*PluginConfig.Instance.minReactionTime;
            set
            {
                //PluginConfig.Instance.minReactionTime = CalculateReactionTime_2(minJump);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
            }
        }*/

        [UIValue("maxRT")]
        public float maxRT => Get_Max_RT();
        /*{
            get => PluginConfig.Instance.maxReactionTime; //
            set
            {
                //PluginConfig.Instance.maxReactionTime = CalculateReactionTime_2(maxJump);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
            }
        }*/



        [UIValue("enabled")]
        public bool modEnabled
        {
            get => PluginConfig.Instance.enabled;
            set
            {
                PluginConfig.Instance.enabled = value;
            }
        }
        [UIAction("setEnabled")]
        public void SetEnabled(bool value)
        {
            modEnabled = value;
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


        [UIValue("mapDefaultJD")]
        public string MapDefaultJDText => GetMapDefaultJDText();
        //public string MapDefaultJDText => "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0.#") + " ms";

        public string GetMapDefaultJDText()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###") + "     <#8c1aff>" + _selectedBeatmap.ReactionTime.ToString("0.#") + " ms";

            return "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString("0.###");
        }

        [UIValue("mapMinJD")]
        public string MapMinJDText => GetMapMinJDText();
        //public string MapMinJDText => "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

        public string GetMapMinJDText()
        {
            if (PluginConfig.Instance.rt_display_enabled)
                return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###") + "     <#8c8c8c>" + _selectedBeatmap.MinReactionTime.ToString("0.#" + " ms");

            return "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString("0.###");
        }


        [UIValue("minJump")]
        private int minJump => PluginConfig.Instance.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jumpDisSlider")]
        public SliderSetting jumpDisSlider;

        public event PropertyChangedEventHandler PropertyChanged;

        [UIValue("jumpDisValue")]
        public float jumpDisValue
        {
            get => GetJumpDistance();//PluginConfig.Instance.jumpDistance;
            set
            {
                PluginConfig.Instance.jumpDistance = value;
                //PluginConfig.Instance.minReactionTime = Get_Min_RT();
                //PluginConfig.Instance.maxReactionTime = Get_Max_RT();
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));
                
                Logger.log.Debug("update jd. selected njs: " + _selectedBeatmap.NJS);
                Logger.log.Debug("update jd. selected njs: " + CalculateReactionTime_2(minJump));
                Logger.log.Debug("update jd. selected njs: " + CalculateReactionTime_2(maxJump));


                //NotifyPropertyChanged(nameof(ReactionTimeText));

                //Logger.log.Debug(value.ToString());
                //Logger.log.Debug(_selectedBeatmap.NJS.ToString());
            }
        }
        [UIAction("setJumpDis")]
        public void SetJumpDis(float value)
        {
            jumpDisValue = value;
            PostParse();
        }

        private float GetJumpDistance()
        {
            return PluginConfig.Instance.jumpDistance;
        }


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


        // Exp RT Slider

        [UIComponent("rtSlider")]
        public SliderSetting rtSlider;

        [UIValue("rtValue")]
        public float rtValue
        {
            get => CalculateReactionTime_2(PluginConfig.Instance.jumpDistance);
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));

                PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(jumpDisValue)));
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText))); // For validation               


                //NotifyPropertyChanged(nameof(ReactionTimeText));

                //Logger.log.Debug(value.ToString());
                //Logger.log.Debug(_selectedBeatmap.NJS.ToString());
            }
        }


        [UIAction("setrt")]
        public void SetRT(float value)
        {
            rtValue = value;
        }



        // New for BS 1.19.0
        //#################################
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

        //##############################################


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
        }


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



        [UIAction("prefButtonClicked")]
        public void PrefButtonClicked()
        {
            /* Kyle used to have a helper function which you also used (DeepestChildFlowCoordinator). 
             * Beat Games has added this to the game since, so we can just use something they helpfully provided us
             */
            FlowCoordinator currentFlow = _mainFlow.YoungestChildFlowCoordinatorOrSelf();
            // We need to give our current flow coordinator to the pref flow so it can exit
            _prefFlow._parentFlow = currentFlow;
            currentFlow.PresentFlowCoordinator(_prefFlow);
        }

        //###################################
        [UIValue("useHeuristic")]
        public bool heuristicEnabled
        {
            get => PluginConfig.Instance.use_heuristic;
            set
            {
                PluginConfig.Instance.use_heuristic = value;
            }
        }
        [UIAction("setUseHeuristic")]
        public void SetHeuristic(bool value)
        {
            heuristicEnabled = value;
        }


        [UIValue("thresholds")]
        public string thresholds
        {
            get => "≤ " + PluginConfig.Instance.lower_threshold.ToString() + " and " + PluginConfig.Instance.upper_threshold.ToString() + " ≤";
        }


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
        public HMUI.CustomFormatRangeValuesSlider minval;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            jd_slider_text = jumpDisSlider.slider.GetComponentInChildren<CurvedTextMeshPro>();

            if (jd_slider_text != null)
            {
                jd_slider_text.color = new UnityEngine.Color(1f, 1f, 0f);
            }

            rt_slider_text = rtSlider.slider.GetComponentInChildren<CurvedTextMeshPro>();

            if (rt_slider_text != null)
            {
                rt_slider_text.color = new UnityEngine.Color(204f/255f, 153f/255f, 1f);
            }

            minval = rtSlider.slider.GetComponentInChildren<HMUI.CustomFormatRangeValuesSlider>();

            minval.minValue = _selectedBeatmap.MinRTSlider;
            minval.maxValue = _selectedBeatmap.MaxRTSlider;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
        }
    }

    public enum PreferenceEnum
    {
        Off = 0,
        JumpDistance = 1,
        ReactionTime = 2
    }
}
