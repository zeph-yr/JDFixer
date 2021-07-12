using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDFixer
{
    public delegate void BeatmapInfoEventHandler(BeatmapInfo e);
    public class BeatmapInfo
    {
        public static event BeatmapInfoEventHandler SelectedChanged;

        public static void SetSelected(IDifficultyBeatmap diff)
        {
            var updatedMapInfo = diff == null ? Empty : new BeatmapInfo(diff);
            Selected = updatedMapInfo;

            SelectedChanged?.Invoke(updatedMapInfo);
        }

        public static BeatmapInfo Selected { get; private set; } = Empty;

        public static BeatmapInfo Empty { get; } = new BeatmapInfo();

        private BeatmapInfo()
        {

        }
        public BeatmapInfo(IDifficultyBeatmap diff)
        {

            float bpm = diff.level.beatsPerMinute;
            float njs = diff.noteJumpMovementSpeed;
            float offset = diff.noteJumpStartBeatOffset;


            JumpDistance = BeatmapUtils.CalculateJumpDistance(bpm, njs, offset);
            MinJumpDistance = BeatmapUtils.CalculateJumpDistance(bpm, njs, -50f);
        }

        public float JumpDistance { get; }
        public float MinJumpDistance { get; }
    }
}
