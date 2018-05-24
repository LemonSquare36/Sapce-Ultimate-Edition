using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Ultimate_Sapce
{
    class SoundManager
    {
        Song currentSong;
        TimeSpan songStartTime, songEndTime;
        float MasterVolume = 1;
        bool done = false, effect = false;
        SoundEffect Effect;
        SoundEffectInstance EffectInstance;
        public void Load(string choice)
        {
            currentSong = Main.GameContent.Load<Song>("Music/"+ choice);

            if (choice == "Solar Empress")
            {
                songStartTime = new TimeSpan(0, 0, 20); songEndTime = new TimeSpan(0, 3, 7);
                MediaPlayer.Volume = 0.2f;
            }
        }
        public void Load(string choice, bool isSoundEffect)
        {
            Effect = Main.GameContent.Load<SoundEffect>("SFX/" + choice);
            effect = isSoundEffect;
            EffectInstance = Effect.CreateInstance();
        }

        public void Play()
        {
            if (!effect)
            {
                MediaPlayer.Play(currentSong);
            }
            else
            {
                EffectInstance.Volume = MasterVolume;
                EffectInstance.Play();
            }
        }
        public void Stop()
        {
            if (!effect)
                MediaPlayer.Stop();
            else
                EffectInstance.Stop();
           
        }
        public void FadeIn()
        {
            if (!done)
            {
                if (MasterVolume > MediaPlayer.Volume)
                {
                    MediaPlayer.Volume += 0.02f * MasterVolume;
                }
                if (MasterVolume <= MediaPlayer.Volume)
                {
                    done = true;
                }
            }

        }
        public void Repeat()
        {
            if (MediaPlayer.PlayPosition >= songEndTime)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(currentSong, songStartTime);
            }
        }

    }
}
