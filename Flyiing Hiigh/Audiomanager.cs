using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;

using Plugin.SimpleAudioPlayer;

namespace Flyiing_Hiigh
{
    public class Audiomanager
    {
        ISimpleAudioPlayer player;
           

        public static void Play(String name, double volume)
        {
            ISimpleAudioPlayer playerStatic;
            playerStatic = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            playerStatic.Load(name);
            playerStatic.Volume = volume;
            playerStatic.Play();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Resume()
        {
            player.Play();
        }

        public void play(String name)
        {
            player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            player.Load(name);
            player.Volume = 50;
            player.Loop = true;
            player.Play();

        }

        public void stopPlaying()
        {
            player.Stop();
        }

       
    }
}