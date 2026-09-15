using AstroDroids.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoSound;
using MonoSound.Streaming;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AstroDroids.Managers
{
    public class SoundManager
    {
        static bool initialized;

        static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
        static Dictionary<string, SoundPool> soundPools = new Dictionary<string, SoundPool>();
        static Dictionary<string, Song> music = new Dictionary<string, Song>();
        static StreamPackage currentMusicPackage = null;

        static CoroutineManager coroutineManager = new CoroutineManager();

        public static string CurrentMusic { get; private set; } = string.Empty;
        static string targetMusic = string.Empty;
        static bool repeatingMusic = false;
        static bool stopped = true;

        public static float SoundVolume { get; set; } = 1f;
        public static float MusicVolume { get; set; } = 1f;
        public static bool IsMusicStopped => currentMusicPackage == null || currentMusicPackage.Disposed || currentMusicPackage.PlayingSound.State == SoundState.Stopped;
        public static TimeSpan MusicPlayPositionSeconds => currentMusicPackage?.CurrentDuration ?? TimeSpan.Zero;
        public static void Initialize(AstroDroidsGame game)
        {
            if (initialized) return;

            MonoSoundLibrary.Init(game);

            LoadAllSounds(game.Content);
            //LoadAllMusic(game.Content);

            coroutineManager.StartCoroutine(MusicCoroutine());

            initialized = true;
        }

        static IEnumerator MusicCoroutine()
        {
            while (true)
            {
                if (targetMusic == string.Empty && !stopped)
                {
                    if (currentMusicPackage != null)
                    {
                        while (!currentMusicPackage.Disposed && currentMusicPackage.Metrics.Volume > 0f)
                        {
                            currentMusicPackage.Metrics.Volume = MathHelper.Max(0f, currentMusicPackage.Metrics.Volume - 0.01f);
                            yield return null;
                        }

                        if (!currentMusicPackage.Disposed)
                        {
                            if(currentMusicPackage.PlayingSound.State != SoundState.Stopped)
                                currentMusicPackage.Stop();
                            currentMusicPackage.Dispose();
                        }
                        currentMusicPackage = null;
                    }
                    CurrentMusic = string.Empty;
                    stopped = true;
                }
                else if (CurrentMusic != targetMusic && !stopped)
                {
                    if (currentMusicPackage != null)
                    {
                        while (!currentMusicPackage.Disposed && currentMusicPackage.Metrics.Volume > 0f)
                        {
                            currentMusicPackage.Metrics.Volume = MathHelper.Max(0f, currentMusicPackage.Metrics.Volume - 0.01f);
                            yield return null;
                        }

                        if (!currentMusicPackage.Disposed)
                        {
                            if (currentMusicPackage.PlayingSound.State != SoundState.Stopped)
                                currentMusicPackage.Stop();
                            currentMusicPackage.Dispose();
                        }
                        currentMusicPackage = null;
                    }

                    if (!string.IsNullOrEmpty(targetMusic))
                    {
                        string musicPath = Path.Combine("Content", "Music", targetMusic + ".ogg");

                        if (File.Exists(musicPath))
                        {
                            currentMusicPackage = StreamLoader.GetStreamedSound(musicPath, repeatingMusic);
                            currentMusicPackage.IsLooping = repeatingMusic;
                            currentMusicPackage.Metrics.Volume = 0f;
                            currentMusicPackage.Play();

                            CurrentMusic = targetMusic;

                            while (!currentMusicPackage.Disposed && currentMusicPackage.Metrics.Volume < MusicVolume)
                            {
                                currentMusicPackage.Metrics.Volume = MathHelper.Min(MusicVolume, currentMusicPackage.Metrics.Volume + 0.01f);
                                yield return null;
                            }
                        }
                    }
                }
                else if (!stopped && currentMusicPackage != null && !currentMusicPackage.Disposed)
                {
                    if (currentMusicPackage.Metrics.Volume < MusicVolume)
                    {
                        currentMusicPackage.Metrics.Volume = MathHelper.Min(MusicVolume, currentMusicPackage.Metrics.Volume + 0.01f);
                    }
                    else if (currentMusicPackage.Metrics.Volume > MusicVolume)
                    {
                        currentMusicPackage.Metrics.Volume = MathHelper.Max(MusicVolume, currentMusicPackage.Metrics.Volume - 0.01f);
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
            if (currentMusicPackage != null)
            {
                if (!currentMusicPackage.Disposed)
                {
                    currentMusicPackage.Stop();
                    currentMusicPackage.Dispose();
                }
                currentMusicPackage = null;
            }
            CurrentMusic = string.Empty;
            stopped = true;
        }

        public static void FadeOutMusic()
        {
            targetMusic = string.Empty;
        }

        public static SoundEffectInstance PlaySound(string name, float pitch = 1f)
        {
            if (soundPools.ContainsKey(name))
            {
                SoundPool soundPool = soundPools[name];
                return soundPool.Play(pitch);
            }
            else
            {
                return null;
            }
        }

        static void LoadAllSounds(ContentManager content)
        {
            Directory.GetFiles("Content/Sounds", "*.wav", SearchOption.AllDirectories).ToList().ForEach(filePath =>
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

        //static void LoadAllMusic(ContentManager content)
        //{
        //    Directory.GetFiles("Content/Music", "*.xnb", SearchOption.AllDirectories).ToList().ForEach(filePath =>
        //    {
        //        string relativePath = filePath.Substring(8).Replace(".xnb", "").Replace("\\", "/");
        //        string musicName = Path.GetFileNameWithoutExtension(filePath);

        //        if (!music.ContainsKey(musicName))
        //        {
        //            Song song = content.Load<Song>(relativePath);
        //            music.Add(relativePath.Substring(6), song);
        //        }
        //    });
        //}
    }
}
