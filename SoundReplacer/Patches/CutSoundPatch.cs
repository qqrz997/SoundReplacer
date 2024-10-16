﻿using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace SoundReplacer.Patches
{
    internal class CutSoundPatch : IInitializable, IDisposable
    {
        private readonly NoteCutSoundEffectManager _noteCutSoundEffectManager;

        private readonly AudioClip _emptySound = SoundLoader.GetEmptyAudioClip();
        private readonly AudioClip[] _customCutSound = new AudioClip[1];

        private readonly AudioClip[] _originalLongCutSounds;
        private readonly AudioClip[] _originalShortCutSounds;

        private string? _lastCutSoundSelected;

        private CutSoundPatch(NoteCutSoundEffectManager noteCutSoundEffectManager)
        {
            _noteCutSoundEffectManager = noteCutSoundEffectManager;
            _originalShortCutSounds = noteCutSoundEffectManager._shortCutEffectsAudioClips;
            _originalLongCutSounds = noteCutSoundEffectManager._longCutEffectsAudioClips;
            _customCutSound[0] = _emptySound;
        }

        public void Initialize()
        {
            if (Plugin.Config.GoodHitSound == SoundLoader.NoSoundID)
            {
                _customCutSound[0] = _emptySound;
                _noteCutSoundEffectManager._shortCutEffectsAudioClips = _customCutSound;
                _noteCutSoundEffectManager._longCutEffectsAudioClips = _customCutSound;
            }
            else if (Plugin.Config.GoodHitSound == SoundLoader.DefaultSoundID)
            {
                _noteCutSoundEffectManager._shortCutEffectsAudioClips = _originalShortCutSounds;
                _noteCutSoundEffectManager._longCutEffectsAudioClips = _originalLongCutSounds;
            }
            else
            {
                _customCutSound[0] = GetCustomCutSound();
                _noteCutSoundEffectManager._shortCutEffectsAudioClips = _customCutSound;
                _noteCutSoundEffectManager._longCutEffectsAudioClips = _customCutSound;
            }
        }

        private AudioClip GetCustomCutSound()
        {
            if (_lastCutSoundSelected == Plugin.Config.GoodHitSound)
            {
                return _customCutSound[0];
            }
            _lastCutSoundSelected = Plugin.Config.GoodHitSound;

            var cutSound = SoundLoader.LoadAudioClip(Plugin.Config.GoodHitSound);
            return cutSound != null ? cutSound : _emptySound;
        }

        public void Dispose()
        {
            if (_customCutSound[0] != null && _customCutSound[0] != _emptySound)
            {
                Object.Destroy(_customCutSound[0]);
            }
        }
    }
}
