using JDFixer.Interfaces;
using System;
using System.Collections.Generic;
using Zenject;


namespace JDFixer.Managers
{
    internal class JDFixerUIManager : IInitializable, IDisposable
    {
        private static StandardLevelDetailViewController levelDetail;
        private static MissionSelectionMapViewController missionSelection;
        private static MainMenuViewController mainMenu;

        private readonly List<IBeatmapInfoUpdater> beatmapInfoUpdaters;


        [Inject]
        private JDFixerUIManager(StandardLevelDetailViewController standardLevelDetailViewController, MissionSelectionMapViewController missionSelectionMapViewController, MainMenuViewController mainMenuViewController, List<IBeatmapInfoUpdater> iBeatmapInfoUpdaters)
        {
            //Plugin.Log.Debug("JDFixerUIManager()");

            levelDetail = standardLevelDetailViewController;
            missionSelection = missionSelectionMapViewController;
            mainMenu = mainMenuViewController;

            beatmapInfoUpdaters = iBeatmapInfoUpdaters;
        }


        public void Initialize()
        {
            //Plugin.Log.Debug("Initialize()");

            levelDetail.didChangeDifficultyBeatmapEvent += LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent += LevelDetail_didChangeContentEvent;

            if (Plugin.CheckForCustomCampaigns())
            {
                missionSelection.didSelectMissionLevelEvent += MissionSelection_didSelectMissionLevelEvent_CC;
            }
            else
            {
                missionSelection.didSelectMissionLevelEvent += MissionSelection_didSelectMissionLevelEvent_Base;
            }

            mainMenu.didDeactivateEvent += MainMenu_didDeactivateEvent; ;
        }


        public void Dispose()
        {
            //Plugin.Log.Debug("Dispose()");

            levelDetail.didChangeDifficultyBeatmapEvent -= LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent -= LevelDetail_didChangeContentEvent;

            missionSelection.didSelectMissionLevelEvent -= MissionSelection_didSelectMissionLevelEvent_CC;
            missionSelection.didSelectMissionLevelEvent -= MissionSelection_didSelectMissionLevelEvent_Base;

            mainMenu.didDeactivateEvent -= MainMenu_didDeactivateEvent;
        }


        private void LevelDetail_didChangeDifficultyBeatmapEvent(StandardLevelDetailViewController arg1, IDifficultyBeatmap arg2)
        {
            //Plugin.Log.Debug("LevelDetail_didChangeDifficultyBeatmapEvent()");

            if (arg1 != null && arg2 != null)
            {
                DiffcultyBeatmapUpdated(arg2);
            }
        }


        private void LevelDetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            //Plugin.Log.Debug("LevelDetail_didChangeContentEvent()");          
            
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {
                //Plugin.Log.Debug("NJS: " + arg1.selectedDifficultyBeatmap.noteJumpMovementSpeed);
                //Plugin.Log.Debug("Offset: " + arg1.selectedDifficultyBeatmap.noteJumpStartBeatOffset);

                DiffcultyBeatmapUpdated(arg1.selectedDifficultyBeatmap);
            }
        }


        private void MissionSelection_didSelectMissionLevelEvent_CC(MissionSelectionMapViewController arg1, MissionNode arg2)
        {
            // Yes, we must check for both arg2.missionData and arg2.missionData.beatmapCharacteristic:
            // If a map is not dled, missionID and beatmapDifficulty will be correct, but beatmapCharacteristic will be null
            // Accessing any null values of arg1 or arg2 will crash CC horribly

            if (arg2.missionData != null && arg2.missionData.beatmapCharacteristic != null)
            {
                Plugin.Log.Debug("In CC, MissionNode exists");

                //Plugin.Log.Debug("MissionNode - missionid: " + arg2.missionId); //"<color=#0a92ea>[STND]</color> Holdin' Oneb28Easy-1"
                //Plugin.Log.Debug("MissionNode - difficulty: " + arg2.missionData.beatmapDifficulty); // "Easy" etc
                //Plugin.Log.Debug("MissionNode - characteristic: " + arg2.missionData.beatmapCharacteristic.serializedName); //"Standard" etc


                if (MissionSelectionPatch.cc_level != null) // lol null check just to print?
                {
                    // If a map is not dled, this will be the previous selected node's map
                    Plugin.Log.Debug("CC Level: " + MissionSelectionPatch.cc_level.levelID);  // For cross check with arg2.missionId

                    IDifficultyBeatmap difficulty_beatmap = CustomCampaigns.Utils.BeatmapUtils.GetMatchingBeatmapDifficulty(MissionSelectionPatch.cc_level.levelID, arg2.missionData.beatmapCharacteristic, arg2.missionData.beatmapDifficulty);

                    if (difficulty_beatmap != null) // lol null check just to print?
                    {
                        //Plugin.Log.Debug("MissionNode Diff: " + difficulty_beatmap.difficulty);  // For cross check with arg2.missionData.beatmapDifficulty
                        //Plugin.Log.Debug("MissionNode Offset: " + difficulty_beatmap.noteJumpStartBeatOffset);
                        //Plugin.Log.Debug("MissionNode NJS: " + difficulty_beatmap.noteJumpMovementSpeed);

                        DiffcultyBeatmapUpdated(difficulty_beatmap);
                    }
                }
            }
            else // Map not dled
            {
                DiffcultyBeatmapUpdated(null);
            }
        }


        private void MissionSelection_didSelectMissionLevelEvent_Base(MissionSelectionMapViewController arg1, MissionNode arg2)
        {
            // Base campaign
            if (arg2 != null)
            {
                DiffcultyBeatmapUpdated(arg2.missionData.level.GetDifficultyBeatmap(arg2.missionData.beatmapCharacteristic, arg2.missionData.beatmapDifficulty));
            }
        }


        private void MainMenu_didDeactivateEvent(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            //Plugin.Log.Debug("MainMenu_didDeactivate");

            if (UI.LegacyModifierUI.Instance != null)
            {
                UI.LegacyModifierUI.Instance.Refresh();
            }

            if (UI.ModifierUI.Instance != null)
            {
                UI.ModifierUI.Instance.Refresh();
            }

            if (UI.CustomOnlineUI.Instance != null)
            {
                UI.CustomOnlineUI.Instance.Refresh();
            }
        }


        private void DiffcultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap)
        {
            //Plugin.Log.Debug("DiffcultyBeatmapUpdated()");

            foreach (var beatmapInfoUpdater in beatmapInfoUpdaters)
            {
                beatmapInfoUpdater.BeatmapInfoUpdated(new BeatmapInfo(difficultyBeatmap));
            }
        }
    }
}