using AstroDroids.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoSound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AstroDroids.Managers
{
    public static class Sounds
    {
        public const string UI_Accept = "Coin01";
        public const string UI_ButtonFocus = "Btn";
    }

    public class SoundManager
    {
        static bool initialized;

        static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        static Dictionary<string, SoundPool> soundPools = new Dictionary<string, SoundPool>();
        static Dictionary<string, SoundEffect> musics = new Dictionary<string, SoundEffect>();
        //static StreamPackage currentMusicPackage = null;

        static CoroutineManager coroutineManager = new CoroutineManager();

        public static string CurrentMusic { get; private set; } = string.Empty;
        static string targetMusic = string.Empty;
        static bool repeatingMusic = false;
        static bool stopped = true;

        static SoundEffectInstance musicInstance;

        public static float SoundVolume { get; set; } = 1f;
        public static float MusicVolume { get; set; } = 1f;
        public static bool IsMusicStopped => musicInstance == null || musicInstance.IsDisposed || musicInstance.State == SoundState.Stopped;
        //public static TimeSpan MusicPlayPositionSeconds => musicInstance?.CurrentDuration ?? TimeSpan.Zero;
        public static void Initialize(AstroDroidsGame game)
        {
            if (initialized) return;

            MonoSoundLibrary.Init(game);

            LoadAllSounds(game.Content);
            LoadAllMusic(game.Content);

            coroutineManager.StartCoroutine(MusicCoroutine());

            initialized = true;
        }

        public static async Task InitializeAsync(AstroDroidsGame game, Action<float> progress, CancellationToken cancellationToken)
        {
            if (initialized) return;

            MonoSoundLibrary.Init(game);

            string[] musicFiles = Directory.GetFiles("Content/Music", "*.ogg", SearchOption.AllDirectories);
            string[] soundFiles = Directory.GetFiles("Content/Sounds", "*.ogg", SearchOption.AllDirectories);

            int totalFiles = musicFiles.Length + soundFiles.Length;

            if (totalFiles == 0)
            {
                progress?.Invoke(1f);
                initialized = true;
                return;
            }

            int filesLoaded = 0;
            object progressLock = new object();

            Action onFileLoaded = () =>
            {
                lock (progressLock)
                {
                    filesLoaded++;
                    progress?.Invoke((float)filesLoaded / totalFiles);
                }
            };

            await Task.WhenAll(LoadAllSoundsAsync(soundFiles, onFileLoaded, cancellationToken), LoadAllMusicAsync(musicFiles, onFileLoaded, cancellationToken));

            if (!cancellationToken.IsCancellationRequested)
            {
                coroutineManager.StartCoroutine(MusicCoroutine());
                initialized = true;
            }
        }

        static IEnumerator MusicCoroutine()
        {
            while (true)
            {
                if (targetMusic == string.Empty && !stopped)
                {
                    if (musicInstance != null)
                    {
                        while (!musicInstance.IsDisposed && musicInstance.Volume > 0f)
                        {
                            musicInstance.Volume = MathHelper.Max(0f, musicInstance.Volume - 0.01f);
                            yield return null;
                        }

                        if (!musicInstance.IsDisposed)
                        {
                            if (musicInstance.State != SoundState.Stopped)
                                musicInstance.Stop();
                            musicInstance.Dispose();
                        }
                        musicInstance = null;
                    }
                    CurrentMusic = string.Empty;
                    stopped = true;
                }
                else if (CurrentMusic != targetMusic && !stopped)
                {
                    if (musicInstance != null)
                    {
                        while (!musicInstance.IsDisposed && musicInstance.Volume > 0f)
                        {
                            musicInstance.Volume = MathHelper.Max(0f, musicInstance.Volume - 0.01f);
                            yield return null;
                        }

                        if (!musicInstance.IsDisposed)
                        {
                            if (musicInstance.State != SoundState.Stopped)
                                musicInstance.Stop();
                            musicInstance.Dispose();
                        }
                        musicInstance = null;
                    }

                    if (!string.IsNullOrEmpty(targetMusic) && musics.ContainsKey(targetMusic))
                    {
                        SoundEffect music = musics[targetMusic];
                        musicInstance = music.CreateInstance();
                        musicInstance.IsLooped = repeatingMusic;
                        musicInstance.Volume = 0f;
                        musicInstance.Play();

                        CurrentMusic = targetMusic;

                        while (musicInstance != null && !musicInstance.IsDisposed && musicInstance.Volume < MusicVolume && !stopped)
                        {
                            musicInstance.Volume = MathHelper.Min(MusicVolume, musicInstance.Volume + 0.01f);
                            yield return null;
                        }
                    }
                }
                else if (!stopped && musicInstance != null && !musicInstance.IsDisposed)
                {
                    if (musicInstance.Volume < MusicVolume)
                    {
                        musicInstance.Volume = MathHelper.Min(MusicVolume, musicInstance.Volume + 0.01f);
                    }
                    else if (musicInstance.Volume > MusicVolume)
                    {
                        musicInstance.Volume = MathHelper.Max(MusicVolume, musicInstance.Volume - 0.01f);
                    }
                }
                else if (stopped)
                {
                    if (musicInstance != null)
                    {
                        if (!musicInstance.IsDisposed)
                        {
                            musicInstance.Stop();
                            musicInstance.Dispose();
                            musicInstance = null;
                        }
                    }
                }

                yield return null;
            }
        }

        public static void Update(GameTime gameTime)
        {
            coroutineManager.Update(gameTime);
        }

        public static void PlayMusic(string name, bool isRepeating = true)
        {
            repeatingMusic = isRepeating;
            targetMusic = name;
            stopped = false;
        }

        public static void StopMusic()
        {
            CurrentMusic = string.Empty;
            stopped = true;
        }

        public static void FadeOutMusic()
        {
            targetMusic = string.Empty;
        }

        public static SoundEffectInstance PlaySound(string name, float pitch = 0f)
        {
            if (soundPools.ContainsKey(name))
            {
                SoundPool soundPool = soundPools[name];
                return soundPool.Play(pitch, SoundVolume);
            }
            else
            {
                return null;
            }
        }

        static void LoadAllSounds(ContentManager content)
        {
            Directory.GetFiles("Content/Sounds", "*.ogg", SearchOption.AllDirectories).ToList().ForEach(filePath =>
            {
                string relativePath = filePath.Replace("\\", "/");
                string soundName = Path.GetFileNameWithoutExtension(filePath);
                if (!sounds.ContainsKey(soundName))
                {
                    //SoundEffect sound = content.Load<SoundEffect>(relativePath);
                    SoundEffect sound = EffectLoader.GetEffect(relativePath);
                    sound.Name = soundName;
                    string key = Path.GetFileNameWithoutExtension(relativePath.Substring(7));
                    sounds.Add(key, sound);

                    soundPools.Add(key, new SoundPool(sound, 16));
                }
            });
        }

        static async Task LoadAllSoundsAsync(string[] files, Action onFileLoaded, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var filePath in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    string relativePath = filePath.Replace("\\", "/");
                    string soundName = Path.GetFileNameWithoutExtension(filePath);
                    if (!sounds.ContainsKey(soundName))
                    {
                        SoundEffect sound = EffectLoader.GetEffect(relativePath);
                        sound.Name = soundName;
                        string key = Path.GetFileNameWithoutExtension(relativePath.Substring(7));
                        sounds.Add(key, sound);

                        soundPools.Add(key, new SoundPool(sound, 16));
                    }

                    onFileLoaded();
                }
            });
        }

        static void LoadAllMusic(ContentManager content)
        {
            Directory.GetFiles("Content/Music", "*.ogg", SearchOption.AllDirectories).ToList().ForEach(filePath =>
            {
                string relativePath = filePath.Replace("\\", "/");
                string musicName = Path.GetFileNameWithoutExtension(filePath);

                if (!musics.ContainsKey(musicName))
                {
                    SoundEffect music = EffectLoader.GetEffect(relativePath);
                    music.Name = musicName;
                    string key = Path.GetFileNameWithoutExtension(relativePath.Substring(6));
                    musics.Add(key, music);

                    //soundPools.Add(key, new SoundPool(sound, 16));

                    //Song song = content.Load<Song>(relativePath);
                    //music.Add(relativePath.Substring(6), song);
                }
            });
        }

        static async Task LoadAllMusicAsync(string[] files, Action onFileLoaded, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                foreach (var filePath in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    string relativePath = filePath.Replace("\\", "/");
                    string musicName = Path.GetFileNameWithoutExtension(filePath);

                    if (!musics.ContainsKey(musicName))
                    {
                        SoundEffect music = EffectLoader.GetEffect(relativePath);
                        music.Name = musicName;
                        string key = Path.GetFileNameWithoutExtension(relativePath.Substring(6));
                        musics.Add(key, music);
                    }

                    onFileLoaded();
                }
            });
        }
    }
}
