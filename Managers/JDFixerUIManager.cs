using JDFixer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace JDFixer.Managers
{
    class JDFixerUIManager : IInitializable, IDisposable
    {
        private readonly StandardLevelDetailViewController levelDetail;
        private readonly List<IBeatmapInfoUpdater> beatmapInfoUpdaters;

        public JDFixerUIManager(StandardLevelDetailViewController levelDetail, List<IBeatmapInfoUpdater> beatmapInfoUpdaters)
        {
            this.levelDetail = levelDetail;
            this.beatmapInfoUpdaters = beatmapInfoUpdaters;
        }

        public void Initialize()
        {
            levelDetail.didChangeDifficultyBeatmapEvent += LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent += LevelDetail_didChangeContentEvent;
        }

        public void Dispose()
        {
            levelDetail.didChangeDifficultyBeatmapEvent -= LevelDetail_didChangeDifficultyBeatmapEvent;
            levelDetail.didChangeContentEvent -= LevelDetail_didChangeContentEvent;
        }

        private void LevelDetail_didChangeDifficultyBeatmapEvent(StandardLevelDetailViewController arg1, IDifficultyBeatmap arg2)
        {
            DiffcultyBeatmapUpdated(arg2);
        }

        private void LevelDetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            DiffcultyBeatmapUpdated(arg1.selectedDifficultyBeatmap);
        }

        private void DiffcultyBeatmapUpdated(IDifficultyBeatmap difficultyBeatmap)
        {
            foreach (var beatmapInfoUpdater in beatmapInfoUpdaters)
            {
                beatmapInfoUpdater.BeatmapInfoUpdated(new BeatmapInfo(difficultyBeatmap));
            }
        }
    }
}
