using SoundReplacer.Managers;
using SoundReplacer.Patches;
using SoundReplacer.UI;
using Zenject;

namespace SoundReplacer.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<SettingsViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<UI.SettingsFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();

            Container.BindInterfacesTo<MenuSoundsPatches>().AsSingle();
            Container.BindInterfacesTo<LevelClearedSoundPatches>().AsSingle();
        }
    }
}
