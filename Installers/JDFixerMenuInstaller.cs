using JDFixer.Managers;
using JDFixer.UI;
using Zenject;

namespace JDFixer.Installers
{
    internal class JDFixerMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<JDFixerUIManager>().AsSingle();
            Container.BindInterfacesTo<ModifierUI>().AsSingle();

            // Flow Coordinators need to binded like this, as a component since it is a Unity Component
            Container.Bind<PreferencesFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();

            // Even though ViewControllers are also Unity Components, we bind them with this helper method provided by SiraUtil (FromNewComponentAsViewController)
            Container.Bind<PreferencesListViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<RTPreferencesListViewController>().FromNewComponentAsViewController().AsSingle();
        }
    }
}