using JDFixer.Managers;
using JDFixer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace JDFixer.Installers
{
    internal class JDFixerMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<JDFixerUIManager>().AsSingle();
            Container.BindInterfacesTo<ModifierUI>().AsSingle();
        }
    }
}