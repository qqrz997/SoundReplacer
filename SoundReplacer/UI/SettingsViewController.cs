using System;
using System.IO;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage;
using IPA.Utilities;
using Zenject;

namespace SoundReplacer.UI
{
    [ViewDefinition("SoundReplacer.SettingsView.bsml")]
    [HotReload(RelativePathToLayout = "SettingsView.bsml")]
    internal class SettingsViewController : BSMLAutomaticViewController
    {
        private SongPreviewPlayer _songPreviewPlayer = null!;
        private PluginConfig _config = null!;
        private readonly BasicUIAudioManager _basicUIAudioManager = BeatSaberUI.BasicUIAudioManager;

        private static string[] _sounds = SoundLoader.DefaultSounds;

        [Inject]
        private void Construct(SongPreviewPlayer songPreviewPlayer, PluginConfig config)
        {
            _songPreviewPlayer = songPreviewPlayer;
            _config = config;
        }

        public void RefreshSoundList()
        {
            try
            {
                var directoryInfo = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, nameof(SoundReplacer)));
                directoryInfo.Create();
                _sounds = SoundLoader.DefaultSounds
                    .Concat(directoryInfo
                        .EnumerateFiles("*", SearchOption.AllDirectories)
                        .Where(f => f.Extension is ".ogg" or ".mp3" or ".wav")
                        .Select(f => f.Name))
                    .ToArray();

                NotifyPropertyChanged(nameof(SoundList));
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"Could not load sounds. {ex}");
            }
        }

        [UIValue("sound-list")]
        protected string[] SoundList => _sounds;

        [UIValue("good-hitsound")]
        protected string SettingCurrentGoodHitSound
        {
            get => _config.GoodHitSound;
            set => _config.GoodHitSound = value;
        }

        [UIValue("bad-hitsound")]
        protected string SettingCurrentBadHitSound
        {
            get => _config.BadHitSound;
            set => _config.BadHitSound = value;
        }

        [UIValue("menu-music")]
        protected string SettingCurrentMenuMusic
        {
            get => _config.MenuMusic;
            set
            {
                _config.MenuMusic = value;
                _songPreviewPlayer.Start();
                _songPreviewPlayer.CrossfadeToDefault();
            }
        }

        [UIValue("click-sound")]
        protected string SettingCurrentClickSound
        {
            get => _config.ClickSound;
            set
            {
                _config.ClickSound = value;
                _basicUIAudioManager.Start();
            }
        }

        [UIValue("success-sound")]
        protected string SettingCurrentSuccessSound
        {
            get => _config.SuccessSound;
            set => _config.SuccessSound = value;
        }

        [UIValue("fail-sound")]
        protected string SettingCurrentFailSound
        {
            get => _config.FailSound;
            set => _config.FailSound = value;
        }
    }
}
