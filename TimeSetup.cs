using HarmonyLib;
using JDFixer.Installers;
using SiraUtil.Zenject;

namespace JDFixer
{
    class TimeSetup
    {
        internal static void Inject(Zenjector zenjector)
        {
            /*if (PluginConfig.Instance.enabled && 
            DateTime.Compare(DateTime.Now, new DateTime(2022, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 2)) < 0 && 
            PluginConfig.Instance.af_enabled)*/
            if (true)
            {
                zenjector.Install<JDFixerTimeInstaller>(Location.GameCore);
            }
        }

        internal static void Patch()
        {
            /*if (PluginConfig.Instance.enabled && 
            DateTime.Compare(DateTime.Now, new DateTime(2022, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(2022, 4, 2)) < 0 && 
            PluginConfig.Instance.af_enabled)*/
            if (true)
            {
                var original = AccessTools.Method(typeof(BeatmapObjectSpawnMovementData), nameof(BeatmapObjectSpawnMovementData.GetJumpingNoteSpawnData));
                var postfix = AccessTools.Method(typeof(TimeControllerPatch), nameof(TimeControllerPatch.Postfix));
                Plugin.harmony.Patch(original, null, new HarmonyMethod(postfix));
            }
        }
    }
}
