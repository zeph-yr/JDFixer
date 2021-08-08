using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using UnityEngine;

namespace JDFixer.UI
{
    public class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        private BeatmapInfo _selectedBeatmap = BeatmapInfo.Empty;
        public ModifierUI()
        {
            BeatmapInfo.SelectedChanged += beatmapInfo =>
            {
                _selectedBeatmap = beatmapInfo;
                NotifyPropertyChanged(nameof(MapDefaultJDText));
                NotifyPropertyChanged(nameof(MapMinJDText));
                NotifyPropertyChanged(nameof(ReactionTimeText));
            };
        }

        private string CalculateReactionTime()
        {
            // Super hack way to prevent divide by zero and showing as "infinity" in Campaign
            // Realistically how many maps will have less than 0.002 NJS, and if a map does...
            // it wouldn't matter if you display 10^6 or 0 reaction time anyway
            // 0.002 gives a margin: BeatmapInfo sets null to 0.001
            if (_selectedBeatmap.NJS > 0.002)
                return "<#8c1aff>" + (jumpDisValue / (2 * _selectedBeatmap.NJS) * 1000).ToString() + " ms";

            return "<#8c1aff>0 ms";
        }

        private PreferencesFlowCoordinator _prefFlow;
        [UIValue("minJump")]
        private int minJump => Config.UserConfig.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => Config.UserConfig.maxJumpDistance;


        [UIValue("enabled")]
        public bool modEnabled
        {
            get => Config.UserConfig.enabled;
            set
            {
                Config.UserConfig.enabled = value;
            }
        }
        [UIAction("setEnabled")]
        void SetEnabled(bool value)
        {
            modEnabled = value;
        }


        //----------------------------------------------------
        // KEEP: In case users want this back

        /*[UIValue("practiceEnabled")]
        public bool practiceEnabled
        {
            get => Config.UserConfig.enabledInPractice;
            set
            {
                Config.UserConfig.enabledInPractice = value;
            }
        }
        [UIAction("setPracticeEnabled")]
        void SetPracticeEnabled(bool value)
        {
            practiceEnabled = value;
        }*/
        //----------------------------------------------------


        [UIValue("mapDefaultJD")]
        public string MapDefaultJDText => "<#ffff00>" + _selectedBeatmap.JumpDistance.ToString();

        [UIValue("mapMinJD")]
        public string MapMinJDText => "<#8c8c8c>" + _selectedBeatmap.MinJumpDistance.ToString();

        [UIComponent("jumpDisSlider")]
        private SliderSetting jumpDisSlider;
        [UIValue("jumpDisValue")]
        public float jumpDisValue
        {
            get => Config.UserConfig.jumpDistance;
            set
            {
                Config.UserConfig.jumpDistance = value;
                NotifyPropertyChanged(nameof(ReactionTimeText));

                //Logger.log.Debug(value.ToString());
                //Logger.log.Debug(_selectedBeatmap.NJS.ToString());
            }
        }
        [UIAction("setJumpDis")]
        void SetJumpDis(float value)
        {
            jumpDisValue = value;
        }

        [UIValue("reactionTime")]
        public string ReactionTimeText => CalculateReactionTime();
       

        [UIValue("usePrefJumpValues")]
        public bool usePrefJumpValues
        {
            get => Config.UserConfig.usePreferredJumpDistanceValues;
            set
            {
                Config.UserConfig.usePreferredJumpDistanceValues = value;
            }
        }
        [UIAction("setUsePrefJumpValues")]
        void SetUsePrefJumpValues(bool value)
        {
            usePrefJumpValues = value;
        }

        [UIAction("prefButtonClicked")]
        void PrefButtonClicked()
        {
            if (_prefFlow == null)
                _prefFlow = BeatSaberUI.CreateFlowCoordinator<PreferencesFlowCoordinator>();
            var ActiveFlowCoordinator = DeepestChildFlowCoordinator(BeatSaberUI.MainFlowCoordinator);
            _prefFlow.ParentFlow = ActiveFlowCoordinator;
            ActiveFlowCoordinator.PresentFlowCoordinator(_prefFlow, null, ViewController.AnimationDirection.Horizontal, true);
        }

        //###################################
        [UIValue("useHeuristic")]
        public bool heuristicEnabled
        {
            get => Config.UserConfig.use_heuristic;
            set
            {
                Config.UserConfig.use_heuristic = value;
            }
        }
        [UIAction("setUseHeuristic")]
        void SetHeuristic(bool value)
        {
            heuristicEnabled = value;
        }


        [UIValue("thresholds")]
        public string thresholds
        {
            get => "≤ " + Config.UserConfig.lower_threshold.ToString() + " and " + Config.UserConfig.upper_threshold.ToString() + " ≤";
        }


        // KEEP: In case
        /*[UIValue("lowerthreshold")]
        public string lowerthreshold
        {
            get => Config.UserConfig.lower_threshold.ToString();
        }

        // Thresholds Display
        [UIValue("upperthreshold")]
        public string upperthreshold
        {
            get => Config.UserConfig.upper_threshold.ToString();
        }*/
        //###################################


        [UIComponent("leftButton")]
        private RectTransform leftButton;
        [UIComponent("rightButton")]
        private RectTransform rightButton;

        [UIAction("#post-parse")]
        void PostParse() // Arrow buttons on the slider
        {
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), jumpDisSlider, 0.1f);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }

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
