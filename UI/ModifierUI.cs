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

        private PreferencesFlowCoordinator _prefFlow;

        [UIValue("minJump")]
        private int minJump => PluginConfig.Instance.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => PluginConfig.Instance.maxJumpDistance;


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
            get => PluginConfig.Instance.jumpDistance;
            set
            {
                PluginConfig.Instance.jumpDistance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReactionTimeText)));

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

        [UIValue("reactionTime")]
        public string ReactionTimeText => CalculateReactionTime();
       

        [UIValue("usePrefJumpValues")]
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

            /*if (value)
            {
                PluginConfig.Instance.rt_enabled = false;
                NotifyPropertyChanged(nameof(RTEnabled));
            }*/
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

            /*if (value)
            {
                PluginConfig.Instance.usePreferredJumpDistanceValues = false;
                NotifyPropertyChanged(nameof(usePrefJumpValues));
            }*/
        }
        //=============================================================



        [UIAction("prefButtonClicked")]
        public void PrefButtonClicked()
        {
            Logger.log.Debug("Pref Clicked");

            if (_prefFlow == null)
            {
                Logger.log.Debug("Pref flow is null");
                _prefFlow = BeatSaberUI.CreateFlowCoordinator<PreferencesFlowCoordinator>();

            }

            Logger.log.Debug("After If");
            
            //var ActiveFlowCoordinator = DeepestChildFlowCoordinator(BeatSaberUI.MainFlowCoordinator);
            //Logger.log.Debug("ActiveFlowCoordinator: " + ActiveFlowCoordinator.ToString());

            //_prefFlow._mainFlowCoordinator = ActiveFlowCoordinator;
            Logger.log.Debug("_prefFlow._mainFlowCoordinator: " + _prefFlow._mainFlowCoordinator.ToString());

            // It fails somewhere here
            Logger.log.Debug("_prefFlow: " + _prefFlow.ToString());
            //Logger.log.Debug("ViewController.AnimationDirection.Horizontal: " + ViewController.AnimationDirection.Horizontal);

            //ActiveFlowCoordinator.PresentFlowCoordinator(_prefFlow, null, ViewController.AnimationDirection.Horizontal, true);
            _prefFlow._mainFlowCoordinator.PresentFlowCoordinator(_prefFlow, null, ViewController.AnimationDirection.Horizontal, true);
            Logger.log.Debug("After PresentFlowCoordinator");
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

        public static FlowCoordinator DeepestChildFlowCoordinator(FlowCoordinator root)
        {
            var flow = root.childFlowCoordinator;
            if (flow == null) return root;
            if (flow.childFlowCoordinator == null || flow.childFlowCoordinator == flow)
            {
                return flow;
            }
            return DeepestChildFlowCoordinator(flow);
        }
    }
}
