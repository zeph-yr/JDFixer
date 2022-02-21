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
        public static bool cc_installed = false;


        [Init]
        public Plugin(IPA.Logging.Logger logger, Config conf, Zenjector zenjector)
        {
            Logger.log = logger;
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            zenjector.Install<JDFixerMenuInstaller>(Location.Menu);
        }


        [OnEnable]
        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart()");

            harmony = new Harmony("com.zephyr.BeatSaber.JDFixer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            Logger.log.Debug(IPA.Utilities.UnityGame.GameVersion.ToString());

            CheckForCustomCampaigns();
        }


        [OnDisable]
        public void OnApplicationQuit()
        {
            harmony.UnpatchSelf();
        }


        private void CheckForCustomCampaigns()
        {
            Logger.log.Debug("Check for CC");

            try
            {
                var metadatas = PluginManager.EnabledPlugins.Where(x => x.Id == "CustomCampaigns");
                cc_installed = metadatas.Count() > 0;
            }
            catch (Exception e)
            {
                cc_installed = false;
            }
        }
    }
}
