using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Logging;
using System;
using Zenject;

/*namespace JDFixer.UI
{
    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        private MainFlowCoordinator _mainFlowCoordinator;
        private PreferencesListViewController _menuPointerSelectView;
        private RTPreferencesListViewController _cmpSettingsView;
        private SiraLog _siraLog;

        [Inject]
        public void Construct(MainFlowCoordinator mainFlowCoordinator, PreferencesListViewController cmpSettingsView, SiraLog siraLog)
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
}*/

namespace JDFixer.UI
{
    public class PreferencesFlowCoordinator : FlowCoordinator
    {
        public static FlowCoordinator _mainFlow;
        public static PreferencesListViewController _prefListView;
        public static RTPreferencesListViewController _rtPrefListView;

        [Inject]
        public PreferencesFlowCoordinator(FlowCoordinator mainFlow, PreferencesListViewController prefListView, RTPreferencesListViewController rtPrefListView)
        {
            Logger.log.Debug("Pref Flow constructor");

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
                    Logger.log.Debug("First activation");

                    showBackButton = true;
                    SetTitle("JDFixer Preferences");
                    ProvideInitialViewControllers(_prefListView);
                }

                else
                {
                    Logger.log.Debug("Not first activation");
                }

                //if (PluginConfig.Instance.rt_enabled)
                //    ProvideInitialViewControllers(_rtPrefListView);

                //else
                //    ProvideInitialViewControllers(_prefListView);
            }
            catch(Exception e)
            {
                Logger.log.Debug("Catch");
                //_siraLog.Error(e);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            Logger.log.Debug("Back pressed");

            _mainFlow.DismissFlowCoordinator(this);
        }
    }
}
