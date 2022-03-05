using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using JDFixer.Installers;
using SiraUtil.Zenject;
using System;
using System.Linq;

namespace JDFixer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Harmony harmony;

        internal static bool cc_installed = false;
        internal static string game_version = "";


        [Init]
        public Plugin(IPA.Logging.Logger logger, Config conf, Zenjector zenjector)
        {
            Logger.log = logger;
            PluginConfig.Instance = conf.Generated<PluginConfig>();

            zenjector.Install<JDFixerMenuInstaller>(Location.Menu);
            TimeSetup.Inject(zenjector);
            //zenjector.Install<JDFixerTimeInstaller>(Location.GameCore);
        }


        [OnEnable]
        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart()");

            game_version = IPA.Utilities.UnityGame.GameVersion.ToString();
            Logger.log.Debug(game_version);

            harmony = new Harmony("com.zephyr.BeatSaber.JDFixer");
            TimeSetup.Patch();
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            CheckForCustomCampaigns();
        }


        [OnDisable]
        public void OnApplicationQuit()
        {
            harmony.UnpatchSelf();
        }


        private void CheckForCustomCampaigns()
        {
            try
            {
                var metadatas = PluginManager.EnabledPlugins.Where(x => x.Id == "CustomCampaigns");
                cc_installed = metadatas.Count() > 0;
            }
            catch (Exception e)
            {
                cc_installed = false;
            }

            Logger.log.Debug("CC installed: " + cc_installed);
        }
    }
}
