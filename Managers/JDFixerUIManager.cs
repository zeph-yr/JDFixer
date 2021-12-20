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
        private readonly List<IBeatmapInfoUpdater> beatmapInfoUpdaters;

        [Inject]
        public JDFixerUIManager(StandardLevelDetailViewController standardLevelDetailViewController, MissionSelectionMapViewController missionSelectionMapViewController, List<IBeatmapInfoUpdater> iBeatmapInfoUpdaters)
        {
            //Logger.log.Debug("JDFixerUIManager()");

            levelDetail = standardLevelDetailViewController;
            missionSelection = missionSelectionMapViewController;
            beatmapInfoUpdaters = iBeatmapInfoUpdaters;
        }

        public void Initialize()
        {
            //Logger.log.Debug("Initialize()");

            levelDetail.didChangeDifficultyBeatmapEvent += LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent += LevelDetail_didChangeContentEvent;

            missionSelection.didSelectMissionLevelEvent += MissionSelection_didSelectMissionLevelEvent;
        }

        public void Dispose()
        {
            //Logger.log.Debug("Dispose()");

            levelDetail.didChangeDifficultyBeatmapEvent -= LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent -= LevelDetail_didChangeContentEvent;

            missionSelection.didSelectMissionLevelEvent -= MissionSelection_didSelectMissionLevelEvent;
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

        // QOL: When in Campaigns, set Map JD and Reaction Time displays to show zeroes to prevent misleading player
        private void MissionSelection_didSelectMissionLevelEvent(MissionSelectionMapViewController arg1, MissionNode arg2)
        {
            DiffcultyBeatmapUpdated(null);
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
