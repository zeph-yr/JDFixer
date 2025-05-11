using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace JDFixer.UI
{
    internal sealed class PreferencesFlowCoordinator : FlowCoordinator
    {
        internal FlowCoordinator _parentFlow;
        private JDPreferencesListViewController _prefListView;
        private RTPreferencesListViewController _rtPrefListView;

        /* Since this is binded as a unity component, our "Constructor" is actually a method called Construct (with an inject attribute)
         * We would do the same for ViewControllers if we wanna ask for stuff from Zenject
         */
        [Inject]
        private void Construct(JDPreferencesListViewController jdPreferencesListViewController, RTPreferencesListViewController rTPreferencesListViewController)
        {
            _prefListView = jdPreferencesListViewController;
            _rtPrefListView = rTPreferencesListViewController;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            showBackButton = true;
            SetTitle("JDFixer Preferences");
            
            // Handle PluginConfig.Instance.pref_selected == 0 case
            if (PluginConfig.Instance.use_jd_pref == -1 && PluginConfig.Instance.use_rt_pref == -1)
                PluginConfig.Instance.use_jd_pref = 0;
            
            if (PluginConfig.Instance.use_jd_pref != -1)
                ProvideInitialViewControllers(_prefListView);
            else
                ProvideInitialViewControllers(_rtPrefListView);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _parentFlow?.DismissFlowCoordinator(this);
        }
    }
}
