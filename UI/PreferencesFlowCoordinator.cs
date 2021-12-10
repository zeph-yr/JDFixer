using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Logging;
using System;
using Zenject;

namespace JDFixer.UI
{
    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;
        private PreferencesListViewController _menuPointerSelectView;
        private RTPreferencesListViewController _cmpSettingsView;
        private SiraLog _siraLog;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlowCoordinator, /*MenuPointerSelectView menuPointerSelectView,*/ PreferencesListViewController cmpSettingsView, SiraLog siraLog)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            //_menuPointerSelectView = menuPointerSelectView;
            //_cmpSettingsView = cmpSettingsView;
            _siraLog = siraLog;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    SetTitle("Menu Pointers");
                    showBackButton = true;
                    ProvideInitialViewControllers(_menuPointerSelectView);
                }
            }
            catch (Exception ex)
            {
                _siraLog.Error(ex);
            }

        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}





/*namespace JDFixer.UI
{
    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlow;
        private PreferencesListViewController _prefListView;
        private RTPreferencesListViewController _rtPrefListView;

        [Inject] private SiraLog _siraLog;

        public PreferencesFlowCoordinator(MainFlowCoordinator mainFlow, PreferencesListViewController prefListView,
            RTPreferencesListViewController rtPrefListView)
        {
            _mainFlow = mainFlow;
            _prefListView = prefListView;
            _rtPrefListView = rtPrefListView;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    showBackButton = true;
                    SetTitle("JDFixer Preferences");
                    ProvideInitialViewControllers(_prefListView);
                }

                //if (PluginConfig.Instance.rt_enabled)
                //    ProvideInitialViewControllers(_rtPrefListView);

                //else
                //    ProvideInitialViewControllers(_prefListView);
            }
            catch(Exception e)
            {
                _siraLog.Error(e);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            _mainFlow.DismissFlowCoordinator(this);
        }
    }
}*/
