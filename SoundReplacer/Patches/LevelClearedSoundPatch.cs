using System;
using SiraUtil.Affinity;
using UnityEngine;

namespace SoundReplacer.Patches
{
    internal class LevelClearedSoundPatch : IAffinity, IDisposable
    {
        private readonly ResultsViewController _resultsViewController;
        private readonly SongPreviewPlayer _songPreviewPlayer;
        private readonly SoundLoader _soundLoader;
        private readonly PluginConfig _config;

        private readonly AudioClip _originalLevelClearedSound;
        private AudioClip? _levelClearedSound;
        private AudioClip? _levelFailedSound;

        private LevelClearedSoundPatch(ResultsViewController resultsViewController, SongPreviewPlayer songPreviewPlayer, SoundLoader soundLoader, PluginConfig config)
        {
            _resultsViewController = resultsViewController;
            _songPreviewPlayer = songPreviewPlayer;
            _soundLoader = soundLoader;
            _config = config;
            _originalLevelClearedSound = resultsViewController._levelClearedAudioClip;
        }

        [AffinityPatch(typeof(ResultsViewController), nameof(ResultsViewController.DidActivate))]
        [AffinityPrefix]
        public void PlayCustomLevelFinishedSound()
        {
            // This changes the sound that gets played when there's a new personal best
            // It may be preferable to instead play the custom sound separately
            _resultsViewController._levelClearedAudioClip = _config.SuccessSound switch
            {
                SoundLoader.NoSoundID => SoundLoader.Empty,
                SoundLoader.DefaultSoundID => _originalLevelClearedSound,
                _ => _levelClearedSound = _soundLoader.Load(_levelClearedSound, SoundType.SuccessSound)
            };

            if (_resultsViewController._levelCompletionResults.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed
                && _config.FailSound != SoundLoader.DefaultSoundID)
            {
                var failSound = _config.FailSound switch
                {
                    SoundLoader.NoSoundID => SoundLoader.Empty,
                    _ => _levelFailedSound = _soundLoader.Load(_levelFailedSound, SoundType.FailSound)
                };
                _songPreviewPlayer.CrossfadeTo(failSound, -4f, 0f, failSound.length, null);
            }
        }

        public void Dispose()
        {
            _soundLoader.Unload(SoundType.SuccessSound);
            _soundLoader.Unload(SoundType.FailSound);
        }
    }
}
