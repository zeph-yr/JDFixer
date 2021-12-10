using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using JDFixer.Installers;
using SiraUtil.Zenject;

namespace JDFixer
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static Harmony harmony;
        public static StandardLevelDetailViewController leveldetail;
        public static MissionSelectionMapViewController missionselection;

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
            harmony = new Harmony("com.zephyr.BeatSaber.JDFixer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            //BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("JDFixerOnline", "JDFixer.UI.BSML.modifierOnlineUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Online);
        }

        [OnDisable]
        public void OnApplicationQuit()
        {
            harmony.UnpatchAll("com.zephyr.BeatSaber.JDFixer");
        }
    }
}
