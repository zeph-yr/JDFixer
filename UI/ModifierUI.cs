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
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;
        private readonly MainFlowCoordinator _mainFlow;
        private readonly PreferencesFlowCoordinator _prefFlow;

        // To get the flow coordinators using zenject, we use a constructor
        public ModifierUI(MainFlowCoordinator mainFlowCoordinator, PreferencesFlowCoordinator preferencesFlowCoordinator)
        {
            _mainFlow = mainFlowCoordinator;
            _prefFlow = preferencesFlowCoordinator;
        }

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

        public void BeatmapInfoUpdated(BeatmapInfo beatmapInfo)
        {
            _selectedBeatmap = beatmapInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapDefaultJDText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapMinJDText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(jumpDisValue))); // necessary

            PluginConfig.Instance.maxReactionTime = CalculateReactionTime_2(PluginConfig.Instance.maxJumpDistance);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue))); // necessary
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



        [UIValue("minJump")]
        private int minJump => PluginConfig.Instance.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => PluginConfig.Instance.maxJumpDistance;


        [UIValue("minRT")]
        public float minRT //=> Get_Min_RT();
        {
            get => /*PluginConfig.Instance.minReactionTime;*/ CalculateReactionTime_2(minJump);
            set
            {
                //PluginConfig.Instance.minReactionTime = CalculateReactionTime_2(minJump);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
            }
        }

        [UIValue("maxRT")]
        public float maxRT //=> Get_Max_RT();
        {
            get => PluginConfig.Instance.maxReactionTime; //
            set
            {
                //PluginConfig.Instance.maxReactionTime = CalculateReactionTime_2(maxJump);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rtValue)));
            }
        }



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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));
                
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
        }

        private float GetJumpDistance()
        {
            return PluginConfig.Instance.jumpDistance;
        }


        [UIValue("reactionTime")]
        public string ReactionTimeText => CalculateReactionTime();


        //#####################################################
        // Exp RT Slider

        [UIComponent("rtSlider")]
        public SliderSetting rtSlider;

        [UIValue("rtValue")]
        public float rtValue
        {
            get => CalculateReactionTime_2(PluginConfig.Instance.jumpDistance);
            set
            {
                PluginConfig.Instance.jumpDistance = value / 1000 * (2 * _selectedBeatmap.NJS);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(jumpDisValue)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(minRT)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(maxRT)));

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

        private float Get_Min_RT()
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return PluginConfig.Instance.minJumpDistance / (2 * _selectedBeatmap.NJS) * 1000;

            return 0f;
        }

        private float Get_Max_RT()
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return PluginConfig.Instance.maxJumpDistance / (2 * _selectedBeatmap.NJS) * 1000;

            return 0f;
        }


        private float CalculateReactionTime_2(float jd)
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return jd / (2 * _selectedBeatmap.NJS) * 1000;

            return 0f; // jd / (2 * 10) * 1000;
        }

        //#####################################################



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


        // Replaced with Increment Setting:

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


        //=============================================================
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


        public CurvedTextMeshPro slider_text;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            slider_text = jumpDisSlider.slider.GetComponentInChildren<CurvedTextMeshPro>();

            if (slider_text != null)
            {
                slider_text.color = UnityEngine.Color.red;
            }

            //minRT = CalculateReactionTime_2(PluginConfig.Instance.minJumpDistance);
            //maxRT = CalculateReactionTime_2(PluginConfig.Instance.maxJumpDistance);
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
