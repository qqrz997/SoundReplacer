using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(IPA.Config.Stores.GeneratedStore.AssemblyVisibilityTarget)]

namespace SoundReplacer
{
    internal class PluginConfig
    {
        public string GoodHitSound { get; set; } = SoundLoader.DefaultSoundID;
        public string BadHitSound { get; set; } = SoundLoader.DefaultSoundID;
        public string MenuMusic { get; set; } = SoundLoader.DefaultSoundID;
        public string ClickSound { get; set; } = SoundLoader.DefaultSoundID;
        public string SuccessSound { get; set; } = SoundLoader.DefaultSoundID;
        public string FailSound { get; set; } = SoundLoader.DefaultSoundID;

        public void SetToDefault(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.GoodHitSound:
                    GoodHitSound = SoundLoader.DefaultSoundID;
                    break;
                case SoundType.BadHitSound:
                    BadHitSound = SoundLoader.DefaultSoundID;
                    break;
                case SoundType.MenuMusic:
                    MenuMusic = SoundLoader.DefaultSoundID;
                    break;
                case SoundType.ClickSound:
                    ClickSound = SoundLoader.DefaultSoundID;
                    break;
                case SoundType.SuccessSound:
                    SuccessSound = SoundLoader.DefaultSoundID;
                    break;
                case SoundType.FailSound:
                    FailSound = SoundLoader.DefaultSoundID;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType));
            }
        }
    }
}
