using AstroDroids.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
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

        static CoroutineManager coroutineManager = new CoroutineManager();

        public static string CurrentMusic { get; private set; } = string.Empty;
        static string targetMusic = string.Empty;
        static bool repeatingMusic = false;
        static bool stopped = true;
        public static void Initialize(AstroDroidsGame game)
        {
            if (initialized) return;

            LoadAllSounds(game.Content);
            LoadAllMusic(game.Content);

            coroutineManager.StartCoroutine(MusicCoroutine());

            initialized = true;
        }

        static IEnumerator MusicCoroutine()
        {
            while(true)
            {
                if(targetMusic == string.Empty && !stopped)
                {
                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        while (MediaPlayer.Volume > 0f)
                        {
                            MediaPlayer.Volume -= 0.01f;
                            yield return null;
                        }
                        stopped = true;
                        MediaPlayer.Stop();
                    }
                }
                else if ((CurrentMusic != targetMusic) && !stopped)
                {
                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        while (MediaPlayer.Volume > 0f)
                        {
                            MediaPlayer.Volume -= 0.01f;
                            yield return null;
                        }

                        MediaPlayer.Stop();
                    }

                    MediaPlayer.Volume = 0f;

                    if (!string.IsNullOrEmpty(targetMusic) && music.ContainsKey(targetMusic))
                    {
                        MediaPlayer.Play(music[targetMusic]);
                        MediaPlayer.IsRepeating = repeatingMusic;
                        CurrentMusic = targetMusic;
                    }

                    while (MediaPlayer.Volume < 1f)
                    {
                        MediaPlayer.Volume += 0.01f;
                        yield return null;
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
            MediaPlayer.Stop();
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
            Directory.GetFiles("Content/Sounds", "*.xnb", SearchOption.AllDirectories).ToList().ForEach(filePath =>
            {
                string relativePath = filePath.Substring(8).Replace(".xnb", "").Replace("\\", "/");
                string soundName = Path.GetFileNameWithoutExtension(filePath);
                if (!sounds.ContainsKey(soundName))
                {
                    SoundEffect sound = content.Load<SoundEffect>(relativePath);
                    sound.Name = soundName;
                    sounds.Add(relativePath.Substring(7), sound);

                    soundPools.Add(relativePath.Substring(7), new SoundPool(sound, 16));
                }
            });
        }

        static void LoadAllMusic(ContentManager content)
        {
            Directory.GetFiles("Content/Music", "*.xnb", SearchOption.AllDirectories).ToList().ForEach(filePath =>
            {
                string relativePath = filePath.Substring(8).Replace(".xnb", "").Replace("\\", "/");
                string musicName = Path.GetFileNameWithoutExtension(filePath);
                if (!music.ContainsKey(musicName))
                {
                    Song song = content.Load<Song>(relativePath);
                    music.Add(relativePath.Substring(6), song);
                }
            });
        }
    }
}
