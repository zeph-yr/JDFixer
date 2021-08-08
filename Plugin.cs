using HarmonyLib;
using IPA;
using System;
using System.Linq;
using UnityEngine;
//using TournamentAssistant;

namespace JDFixer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        //private static IPA.Loader.PluginMetadata hasTA;

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            var harmony = new Harmony("com.zephyr.BeatSaber.JDFixer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += BSEvents_lateMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.difficultySelected += BSEvents_difficultySelected;

            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("JDFixer", "JDFixer.UI.BSML.modifierUI.bsml", UI.ModifierUI.instance);
            //BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("JDFixerOnline", "JDFixer.UI.BSML.modifierOnlineUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Online);
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

            //hasTA = IPA.Loader.PluginManager.GetPluginFromId("TournamentAssistant");
            //Logger.log.Debug(hasTA.Name);
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            Config.Write();

            //Logger.log.Debug("Prev: " + arg0.name + " Next: " + arg1.name);
        }

        [Init]
        public void Init(IPA.Logging.Logger logger)
        {
            Logger.log = logger;
        }

        // For when user selects a map with only 1 difficulty or selects a map but does not click a difficulty
        private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            //Logger.log.Debug(obj.scenes[0].sceneName);

            // Note: Doesn't work if you put a return here cuz TA is always enabled even when youre not in TA lol...
            // Seems like leveldetail is always existing even in TA but not after a song ends in Solo
            /*if (hasTA != null && IPA.Loader.PluginManager.IsEnabled(hasTA))
            {
                Logger.log.Debug("TA ENABLED");

                if (TournamentAssistant.Plugin.client != null && TournamentAssistant.Plugin.client.Connected)
                {
                    Logger.log.Debug("INSIDE TA LOBBY");
                    //BeatmapInfo.SetSelected(null);
                }
            }*/

            var leveldetail = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
            //StandardLevelDetailViewController leveldetail = UnityEngine.Object.FindObjectOfType<StandardLevelDetailViewController>();
            if (leveldetail != null)
            {
                leveldetail.didChangeContentEvent += Leveldetail_didChangeContentEvent;
            }

            // When in Campaigns, set Map JD and Reaction Time displays to show zeroes
            var missionselection = Resources.FindObjectsOfTypeAll<MissionSelectionMapViewController>().FirstOrDefault();
            if (missionselection != null)
            {
                missionselection.didSelectMissionLevelEvent += Missionselection_didSelectMissionLevelEvent;
            }

            // Note: Doesnt work here
            /*else
            {
                BeatmapInfo.SetSelected(null);
            }*/
        }

        // When in Campaigns, set Map JD and Reaction Time displays to show zeroes
        private void Missionselection_didSelectMissionLevelEvent(MissionSelectionMapViewController arg1, MissionNode arg2)
        {
            BeatmapInfo.SetSelected(null);
        }


        // For when user selects a map with only 1 difficulty or selects a map but does not click a difficulty
        private void Leveldetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {
                BeatmapInfo.SetSelected(arg1.selectedDifficultyBeatmap);
            }

            // Note: Works to disable in Campaigns but causes it to reset to 0 after a map in Solo
            /*else if (arg1 == null)
            {
                BeatmapInfo.SetSelected(null);
            }*/
        }

        // For when user explicitly clicks on a difficulty (Doesn't work if map has only 1 diff)
        private void BSEvents_difficultySelected(StandardLevelDetailViewController arg1, IDifficultyBeatmap level)
        {
            BeatmapInfo.SetSelected(level);
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Config.Write();
        }
    }
}
