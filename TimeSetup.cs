using HarmonyLib;
using JDFixer.Installers;
using SiraUtil.Zenject;
using System;

namespace JDFixer
{
    internal class TimeSetup
    {
        internal static void Inject(Zenjector zenjector)
        {
            if (PluginConfig.Instance.enabled && (
                (DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 2)) < 0 && PluginConfig.Instance.af_enabled) || 
                (DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 21)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 23)) < 0)))
            
            //if (true)
            {
                //Plugin.Log.Debug("TimeSetup Inject");

                zenjector.Install<JDFixerTimeInstaller>(Location.GameCore);
            }
        }

        internal static void Patch()
        {
            if (PluginConfig.Instance.enabled && 
                DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 3, 31)) >= 0 && DateTime.Compare(DateTime.Now, new DateTime(DateTime.Now.Year, 4, 2)) < 0 && 
                PluginConfig.Instance.af_enabled)
            
            //if (true)
            {
                //Plugin.Log.Debug("TimeSetup Patch");

                var original = AccessTools.Method(typeof(BeatmapObjectSpawnMovementData), nameof(BeatmapObjectSpawnMovementData.GetJumpingNoteSpawnData));
                var postfix = AccessTools.Method(typeof(TimeControllerPatch), nameof(TimeControllerPatch.Postfix));
                Plugin.harmony.Patch(original, null, new HarmonyMethod(postfix));
            }
        }
    }
}
