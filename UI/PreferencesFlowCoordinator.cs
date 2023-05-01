using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace JDFixer.UI
{
    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        public FlowCoordinator _parentFlow;
        private PreferencesListViewController _prefListView;
        private RTPreferencesListViewController _rtPrefListView;

        /* Since this is binded as a unity component, our "Constructor" is actually a method called Construct (with an inject attribute)
         * We would do the same for ViewControllers if we wanna ask for stuff from Zenject
         */
        [Inject]
        public void Construct(PreferencesListViewController preferencesListViewController, RTPreferencesListViewController rTPreferencesListViewController)
        {
            _prefListView = preferencesListViewController;
            _rtPrefListView = rTPreferencesListViewController;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            showBackButton = true;
            SetTitle("JDFixer Preferences");

            if (PluginConfig.Instance.use_rt_pref)
                ProvideInitialViewControllers(_rtPrefListView);
            else
                ProvideInitialViewControllers(_prefListView);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _parentFlow?.DismissFlowCoordinator(this);
        }
    }
}
