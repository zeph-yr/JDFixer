using HarmonyLib;
using IPA;
using System;
using System.Linq;
using UnityEngine;

namespace JDFixer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            var harmony = new Harmony("com.zephyr.BeatSaber.JDFixer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += BSEvents_lateMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.difficultySelected += BSEvents_difficultySelected;
            //BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;

            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.modifierUI.bsml", UI.ModifierUI.instance);
            //BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("JDFixerOnline", "JDFixer.UI.BSML.modifierOnlineUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Online);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            Config.Write();
        }

        [Init]
        public void Init(IPA.Logging.Logger logger)
        {
            Logger.log = logger;
        }

        // For when user selects a map with only 1 difficulty or selects a map but does not click a difficulty
        private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            var leveldetail = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();

            if (leveldetail != null)
            {
                leveldetail.didChangeContentEvent += Leveldetail_didChangeContentEvent;
            }
        }

        // For when user explicitly clicks on a difficulty (Doesn't work if map has only 1 diff)
        private void BSEvents_difficultySelected(StandardLevelDetailViewController arg1, IDifficultyBeatmap level)
        {
            BeatmapInfo.SetSelected(level);
        }

        // For when user selects a map with only 1 difficulty or selects a map but does not click a difficulty
        private void Leveldetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {
                BeatmapInfo.SetSelected(arg1.selectedDifficultyBeatmap);
            }
        }

        // Not necessary anymore:
        /*private void BSEvents_gameSceneLoaded()
        {
            bool WillOverride = BS_Utils.Plugin.LevelData.IsSet && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel
                && Config.UserConfig.enabled && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard && BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings == null;
            if(WillOverride && false) // false is from "!Config.User.Config.dontForceNJS"
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("JDFixer");
        }*/

        [OnExit]
        public void OnApplicationQuit()
        {
            Config.Write();
        }
    }
}
