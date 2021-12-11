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
        public FlowCoordinator _mainFlowCoordinator;
        public PreferencesListViewController _prefListView;
        public RTPreferencesListViewController _rtPrefListView;

        [Inject]
        public PreferencesFlowCoordinator(FlowCoordinator mainFlowCoordinator, PreferencesListViewController preferencesListViewController, RTPreferencesListViewController rTPreferencesListViewController)
        {
            Logger.log.Debug("PreferenceFlowCoordinator constructor");

            _mainFlowCoordinator = mainFlowCoordinator;
            _prefListView = preferencesListViewController;
            _rtPrefListView = rTPreferencesListViewController;

            /*
            if (_rtPrefListView == null)
                _rtPrefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<RTPreferencesListViewController>();

            if (_prefListView == null)
                _prefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<PreferencesListViewController>();
            */
        }

        /*public void Awake()
        {
            Logger.log.Debug("Awake");

            if (_rtPrefListView == null)
            {
                Logger.log.Debug("rtpref null");
                _rtPrefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<RTPreferencesListViewController>();

            }

            if (_prefListView == null)
            {
                Logger.log.Debug("pref null");
                _prefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<PreferencesListViewController>();

            }
        }*/

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    Logger.log.Debug("First activation");

                    showBackButton = true;
                    SetTitle("JDFixer Preferences");

                    if (_rtPrefListView == null)
                        _rtPrefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<RTPreferencesListViewController>();

                    if (_prefListView == null)
                        _prefListView = BeatSaberMarkupLanguage.BeatSaberUI.CreateViewController<PreferencesListViewController>();

                    ProvideInitialViewControllers(_prefListView);

                    Logger.log.Debug("After activation");
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

            //_mainFlow.DismissFlowCoordinator(this);
            _mainFlowCoordinator.InvokeMethod("DismissFlowCoordinator", this, ViewController.AnimationDirection.Horizontal, null, false); ;
        }
    }
}
