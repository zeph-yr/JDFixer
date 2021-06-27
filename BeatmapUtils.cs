using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDFixer
{
    static class BeatmapUtils
    {
        public static float CalculateJumpDistance(float bpm, float njs, float offset)
        {
            float jumpdistance = 0f; // In case
            float halfjump = 4f;
            float num = 60f / bpm;

            while (njs * num * halfjump > 18)
                halfjump /= 2;

            halfjump += offset;
            if (halfjump < 1) halfjump = 1f;

            jumpdistance = njs * num * halfjump * 2;

            return jumpdistance;
        }
    }
}
