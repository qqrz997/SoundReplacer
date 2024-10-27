using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SoundReplacer
{
    internal class SoundLoader : IDisposable
    {
        public const string NoSoundID = "None";
        public const string DefaultSoundID = "Default";
        public static readonly string[] DefaultSounds = { NoSoundID, DefaultSoundID };
        // Duration of 1 second as NoteCutSoundEffect could disable itself before the note is cut otherwise.
        public static readonly AudioClip Empty = AudioClip.Create("Empty", 44100, 1, 44100, false);

        private readonly PluginConfig _config;

        private readonly Dictionary<string, AudioClip> _soundCache = new();

        private SoundLoader(PluginConfig config)
        {
            _config = config;
        }

        private static AudioType GetAudioTypeFromPath(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return extension switch
            {
                _ when extension.Equals(".ogg", StringComparison.OrdinalIgnoreCase) => AudioType.OGGVORBIS,
                _ when extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase) => AudioType.MPEG,
                _ when extension.Equals(".wav", StringComparison.OrdinalIgnoreCase) => AudioType.WAV,
                _ => AudioType.UNKNOWN
            };
        }

        private AudioClip? LoadAudioClip(string fileName, SoundType soundType)
        {
            var filePath = Directory.EnumerateFiles(Path.Combine(UnityGame.UserDataPath, nameof(SoundReplacer)), fileName, SearchOption.AllDirectories).FirstOrDefault();

            if (filePath is null)
            {
                Plugin.Log.Error($"Could not find sound {fileName}");
                _config.SetToDefault(soundType);

                return null;
            }

            var request = UnityWebRequestMultimedia.GetAudioClip(FileHelpers.GetEscapedURLForFilePath(filePath), GetAudioTypeFromPath(filePath));
            var task = request.SendWebRequest();

            // while I would normally kill people for this
            // we are loading a local file, so it should be
            // basically instant success or error
            while (!task.isDone) { }

            if (request.result is not UnityWebRequest.Result.Success)
            {
                Plugin.Log.Error($"Failed to load file {filePath} with error {request.error}");
                _config.SetToDefault(soundType);

                return null;
            }

            return DownloadHandlerAudioClip.GetContent(request);
        }

        private string GetSoundFileName(SoundType soundType)
        {
            return soundType switch
            {
                SoundType.GoodHitSound => _config.GoodHitSound,
                SoundType.BadHitSound => _config.BadHitSound,
                SoundType.MenuMusic => _config.MenuMusic,
                SoundType.ClickSound => _config.ClickSound,
                SoundType.SuccessSound => _config.SuccessSound,
                SoundType.FailSound => _config.FailSound,
                _ => throw new ArgumentOutOfRangeException(nameof(soundType))
            };
        }

        public AudioClip Load(AudioClip? currentSound, SoundType soundType)
        {
            var fileName = GetSoundFileName(soundType);

            if (_soundCache.TryGetValue(fileName, out var cachedSound) && cachedSound == currentSound)
            {
                return cachedSound;
            }

            Object.Destroy(cachedSound);

            var customSound = LoadAudioClip(fileName, soundType);
            if (customSound == null)
            {
                return Empty;
            }

            return _soundCache[fileName] = customSound;
        }

        public void Unload(SoundType soundType)
        {
            if (_soundCache.TryGetValue(GetSoundFileName(soundType), out var cachedSound))
            {
                Object.Destroy(cachedSound);
            }
        }

        public void Dispose()
        {
            foreach (var audioClip in _soundCache.Values)
            {
                Object.Destroy(audioClip);
            }
        }
    }
}
