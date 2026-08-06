using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace AstroDroids.Audio
{
    public class SoundPool
    {
        List<SoundEffectInstance> instances = new List<SoundEffectInstance>();

        public SoundPool(SoundEffect soundEffect, int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                instances.Add(soundEffect.CreateInstance());
            }
        }

        public SoundEffectInstance GetAvailableInstance()
        {
            foreach (var instance in instances)
            {
                if (instance.State != SoundState.Playing)
                {
                    return instance;
                }
            }
            return null;
        }

        public SoundEffectInstance Play(float pitch = 1f)
        {
            var instance = GetAvailableInstance();
            if (instance != null)
            {
                instance.Pitch = pitch;
                instance.Play();
                return instance;
            }
            else
            {
                return null;
            }
        }
    }
}
