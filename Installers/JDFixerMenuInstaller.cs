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
            Container.InstantiateComponentOnNewGameObject<PreferencesFlowCoordinator>();
        }
    }
}