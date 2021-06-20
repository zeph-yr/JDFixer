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

        [UIValue("practiceEnabled")]
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
        }


        //#####################################################
        // Jump Distance Refresh Button
        // Yes, i tried everything make an autoupdating text tag lol
        /*
        [UIComponent("jdbutton")]
        private string jdbutton;
        [UIValue("button-text")]
        public string ButtonText
        {
            get => "<#ebbd52>" + Config.UserConfig.selected_mapJumpDistance.ToString();
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIAction("refresh")]
        void Refresh()
        {
            ButtonText = "something changed"; // This is a hack. Without this the button text doesn't update when clicked lol
            //Logger.log.Debug($"After click {ButtonText}");
        }
        */

        [UIValue("mapJDText")]
        private string MapJDText
        {
            get => "Map Jump Distance\nMin. JD";
        }

        //[UIComponent("mapJDButton")]
        //private string MapJDButton;
        [UIValue("mapJDButton")]
        public string MapJDButton
        {
            get => "<#ffff00>" + Config.UserConfig.selected_mapJumpDistance.ToString() + "\n" + "<#c4c4c4>" + Config.UserConfig.selected_mapLowest.ToString();
            set
            {
                NotifyPropertyChanged();
            }
        }

        [UIAction("mapJD-refresh")]
        void MapJDRefresh()
        {
            MapJDButton = "something changed"; // This is a hack. Without an assignment button doesn't update when clicked lol
            //Logger.log.Debug($"After click {MapJDButton}");
        }
        //#####################################################


        [UIComponent("jumpDisSlider")]
        private SliderSetting jumpDisSlider;
        [UIValue("jumpDisValue")]
        public float jumpDisValue
        {
            get => Config.UserConfig.jumpDistance;
            set
            {
                Config.UserConfig.jumpDistance = value;
            }
        }
        [UIAction("setJumpDis")]
        void SetJumpDis(float value)
        {
            jumpDisValue = value;
        }

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


        // Thresholds Display
        [UIValue("upperthreshold")]
        public float upperthreshold
        {
            get => Config.UserConfig.upper_threshold;
        }

        [UIValue("lowerthreshold")]
        public float lowerthreshold
        {
            get => Config.UserConfig.lower_threshold;
        }
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

        // For when user selects a map with only 1 difficulty or selects a map but does not click a difficulty
        public static void Leveldetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {               
                float map_bpm = arg1.selectedDifficultyBeatmap.level.beatsPerMinute;
                float map_njs = arg1.selectedDifficultyBeatmap.noteJumpMovementSpeed;
                //float map_halfjump = 4f;
                float map_offset = arg1.selectedDifficultyBeatmap.noteJumpStartBeatOffset;

                // NOTE THESE DONT WORK: SONG LOADS FOREVER
                //float bpm = arg1.beatmapLevel.beatsPerMinute;
                //float offset = arg1.beatmapLevel.songTimeOffset;
                //String songname = arg1.beatmapLevel.songName;

                float map_jumpdistance = JDFixer.Plugin.Calculate_JD(map_bpm, map_njs, map_offset);
                float min_map_jumpdistance = JDFixer.Plugin.Calculate_JD(map_bpm, map_njs, -50f);

                Logger.log.Debug($"UI: BPM: {map_bpm} | NJS: {map_njs} | Offset: {map_offset} | Jump Distance: { map_jumpdistance} | Minimum: { min_map_jumpdistance}");

                Config.UserConfig.selected_mapJumpDistance = map_jumpdistance;
                Config.UserConfig.selected_mapLowest = min_map_jumpdistance;
                Config.Write();
            }
        }
    }
}
