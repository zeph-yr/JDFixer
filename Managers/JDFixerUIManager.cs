using JDFixer.Interfaces;
using System;
using System.Collections.Generic;
using Zenject;


namespace JDFixer.Managers
{
    class JDFixerUIManager : IInitializable, IDisposable
    {
        public static StandardLevelDetailViewController levelDetail;
        public static MissionSelectionMapViewController missionSelection;
        public static MainMenuViewController mainMenu;

        private readonly List<IBeatmapInfoUpdater> beatmapInfoUpdaters;


        [Inject]
        public JDFixerUIManager(StandardLevelDetailViewController standardLevelDetailViewController, MissionSelectionMapViewController missionSelectionMapViewController, MainMenuViewController mainMenuViewController, List<IBeatmapInfoUpdater> iBeatmapInfoUpdaters)
        {
            //Logger.log.Debug("JDFixerUIManager()");

            levelDetail = standardLevelDetailViewController;
            missionSelection = missionSelectionMapViewController;
            mainMenu = mainMenuViewController;

            beatmapInfoUpdaters = iBeatmapInfoUpdaters;
        }


        public void Initialize()
        {
            //Logger.log.Debug("Initialize()");

            levelDetail.didChangeDifficultyBeatmapEvent += LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent += LevelDetail_didChangeContentEvent;
            mainMenu.didActivateEvent += MainMenu_didActivateEvent;

            if (Plugin.CheckForCustomCampaigns())
            {
                missionSelection.didSelectMissionLevelEvent += MissionSelection_didSelectMissionLevelEvent_CC;
            }
            else
            {
                missionSelection.didSelectMissionLevelEvent += MissionSelection_didSelectMissionLevelEvent_Base;
            }
        }

        private void MainMenu_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            Logger.log.Debug("MainMenu_didActivate");
            UI.CustomOnlineUI.Instance.Refresh();
        }

        public void Dispose()
        {
            //Logger.log.Debug("Dispose()");

            levelDetail.didChangeDifficultyBeatmapEvent -= LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent -= LevelDetail_didChangeContentEvent;

            missionSelection.didSelectMissionLevelEvent -= MissionSelection_didSelectMissionLevelEvent_CC;
            missionSelection.didSelectMissionLevelEvent -= MissionSelection_didSelectMissionLevelEvent_Base;
        }


        private void LevelDetail_didChangeDifficultyBeatmapEvent(StandardLevelDetailViewController arg1, IDifficultyBeatmap arg2)
        {
            //Logger.log.Debug("LevelDetail_didChangeDifficultyBeatmapEvent()");

            if (arg1 != null && arg2 != null)
            {
                DiffcultyBeatmapUpdated(arg2);
            }
        }


        private void LevelDetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            //Logger.log.Debug("LevelDetail_didChangeContentEvent()");          
            
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {
                //Logger.log.Debug("NJS: " + arg1.selectedDifficultyBeatmap.noteJumpMovementSpeed);
                //Logger.log.Debug("Offset: " + arg1.selectedDifficultyBeatmap.noteJumpStartBeatOffset);

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
                Logger.log.Debug("In CC, MissionNode exists");

                //Logger.log.Debug("MissionNode - missionid: " + arg2.missionId); //"<color=#0a92ea>[STND]</color> Holdin' Oneb28Easy-1"
                //Logger.log.Debug("MissionNode - difficulty: " + arg2.missionData.beatmapDifficulty); // "Easy" etc
                //Logger.log.Debug("MissionNode - characteristic: " + arg2.missionData.beatmapCharacteristic.serializedName); //"Standard" etc


                if (MissionSelectionPatch.cc_level != null) // lol null check just to print?
                {
                    // If a map is not dled, this will be the previous selected node's map
                    Logger.log.Debug("CC Level: " + MissionSelectionPatch.cc_level.levelID);  // For cross check with arg2.missionId

                    IDifficultyBeatmap difficulty_beatmap = CustomCampaigns.Utils.BeatmapUtils.GetMatchingBeatmapDifficulty(MissionSelectionPatch.cc_level.levelID, arg2.missionData.beatmapCharacteristic, arg2.missionData.beatmapDifficulty);

                    if (difficulty_beatmap != null) // lol null check just to print?
                    {
                        Logger.log.Debug("MissionNode Diff: " + difficulty_beatmap.difficulty);  // For cross check with arg2.missionData.beatmapDifficulty
                        Logger.log.Debug("MissionNode Offset: " + difficulty_beatmap.noteJumpStartBeatOffset);
                        Logger.log.Debug("MissionNode NJS: " + difficulty_beatmap.noteJumpMovementSpeed);

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


        private void DiffcultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap)
        {
            //Logger.log.Debug("DiffcultyBeatmapUpdated()");

            foreach (var beatmapInfoUpdater in beatmapInfoUpdaters)
            {
                beatmapInfoUpdater.BeatmapInfoUpdated(new BeatmapInfo(difficultyBeatmap));
            }
        }
    }
}
