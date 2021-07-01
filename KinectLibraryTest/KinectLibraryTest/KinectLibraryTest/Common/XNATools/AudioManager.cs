using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNATools
{
    /// <summary>
    /// The AudioManager is designed for playing background music with a 
    /// variety of different options for easy playing of random or sequential songs.
    /// </summary>
    public class AudioManager
    {
        /// <summary>
        /// MusicMode types change the order of songs that are played by the AudioManager.
        /// </summary>
        public enum MusicMode { None, Sequence, Random, PlayOnce, Repeat };

        #region Instance variables
        /// <summary>
        /// A list of all the music files that have been loaded in.
        /// </summary>
        private List<Song> music;
        
        /// <summary>
        /// The current songID in the music array that is being played.
        /// </summary>
        private int curSongID;
        
        /// <summary>
        /// A random generator for the random sequencing when required.
        /// </summary>
        private Random gen;

        /// <summary>
        /// The current music mode that is used to deterine the next song to play.
        /// </summary>
        private MusicMode musicMode;

        /// <summary>
        /// A reference to the Game class to enable the easy loading of the Songs.
        /// </summary>
        private Game appRef;

        /// <summary>
        /// A reference to the inputManager as an optional extra to enable keyboard 
        /// based interation for muting, and volume changes.
        /// </summary>
        private InputManager inputManager;
        #endregion

        /// <summary>
        /// A basic constructor that will load the songs in to play them sequentially as ordered.
        /// </summary>
        /// <param name="songPaths">Strings with the resource paths to the song files.</param>
        /// <param name="appRef">A reference to the Game class.</param>
        public AudioManager(List<string> songPaths, Game appRef)
            : this(songPaths, MusicMode.Sequence, -1, appRef)
        {
        }

        /// <summary>
        /// A constructor that allows specification of the songs to be loaded
        /// with the additional option to specify the order they will be played 
        /// in via the MusicMode type.
        /// </summary>
        /// <param name="songPaths">Strings with the resource paths to the song files.</param>
        /// <param name="mode">The mode to use for determining the next song.</param>
        /// <param name="appRef">A reference to the Game class.</param>
        public AudioManager(List<string> songPaths, MusicMode mode, Game appRef)
            : this(songPaths, mode, -1, appRef)
        {
        }

        /// <summary>
        /// A constructor that allows specification of the songs to be loaded 
        /// with options for the next song mode and the initial song that should
        /// be started on. 
        /// </summary>
        /// <param name="songPaths">Strings with the resource paths to the song files.</param>
        /// <param name="mode">The mode to use for determining the next song.</param>
        /// <param name="startSongID">The first song to play once loaded.</param>
        /// <param name="appRef">A reference to the Game class.</param>
        public AudioManager(List<string> songPaths, MusicMode mode, int startSongID, Game appRef)
        {
            this.appRef = appRef;
            this.inputManager = null;//appRef.getInputManager();
            music = new List<Song>();

            foreach (string file in songPaths)
            {
                music.Add(loadSong(file));
            }

            curSongID = startSongID;
            this.musicMode = mode;

            gen = new Random();
            if (mode == MusicMode.Random)
            {
                curSongID = gen.Next(music.Count);
            }
        }

        /// <summary>
        /// Controls the automatic starting of the next song, and when an input manager
        /// has been set this method also provides mute options.
        /// </summary>
        public void update(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                startNextSong();
            }
            else if(inputManager != null)
            {
                if (inputManager.isKeyPressed(Keys.M))
                {
                    setMute(!MediaPlayer.IsMuted);// MediaPlayer.Pause();
                }

                if (inputManager.isKeyPressed(Keys.OemMinus))
                {
                    if (MediaPlayer.Volume >= 0.0)
                        MediaPlayer.Volume -= 0.05f;
                }
                else if (inputManager.isKeyPressed(Keys.OemPlus))
                {
                    if (MediaPlayer.Volume <= 1.0)
                        MediaPlayer.Volume += 0.05f;
                }
            }
        }

        public void setMute(bool ismute)
        {
            MediaPlayer.IsMuted = ismute;
        }

        public bool getMuted()
        {
            return MediaPlayer.IsMuted;
        }

        public void setVolume(float volume)
        {
            MediaPlayer.Volume = volume / 100;
        }

        public float getVolume()
        {
            return MediaPlayer.Volume * 100;
        }

        /// <summary>
        /// Chooses a new song and starts playing it based on 
        /// the musicMode.
        /// </summary>
        public void startNextSong()
        {
            if(musicMode == MusicMode.Random)
            {
                // fix for when there is only one song
                if (music.Count == 1)
                {
                    setTrackAndPlay(0);
                    return;
                }

                int nextSongID;
                do
                {
                    nextSongID = gen.Next(music.Count);
                } while (nextSongID == curSongID);
                setTrackAndPlay(nextSongID);
            }
            else if (musicMode == MusicMode.Sequence)
            {
                curSongID++;
                if (curSongID >= music.Count)
                    curSongID = 0;
                setTrackAndPlay(curSongID);
            }
            else if (musicMode == MusicMode.Repeat)
            {
                setTrackAndPlay(curSongID);
            }
        }

        /// <summary>
        /// Sets the mode that is used to determine the next song that should 
        /// be played. 
        /// </summary>
        /// <param name="mode">The new mode.</param>
        public void setMode(MusicMode mode)
        {
            this.musicMode = mode;
        }

        /// <summary>
        /// Stops any existing song that is playing and sets the audio manager to 
        /// play the newly selected track instead.
        /// </summary>
        /// <param name="trackID">The track index to play in the music array.</param>
        public void setTrackAndPlay(int trackID)
        {
            if (trackID < 0 || trackID >= music.Count)
                return;

            curSongID = trackID;
            MediaPlayer.Stop();
            MediaPlayer.Play(music[curSongID]);
        }

        /// <summary>
        /// Gets the current song.
        /// </summary>
        public int getCurSongID()
        {
            return curSongID;
        }

        /// <summary>
        /// Sets the input manager. This will enable the default keyboard interaction.
        /// </summary>
        public void setInputManager(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        /// <summary>
        /// Helper method to allow easy loading of SoundEffect type files.
        /// </summary>
        /// <param name="file">The sound effect file.</param>
        /// <returns>The loaded sound object.</returns>
        public SoundEffect loadSound(string file)
        {
            return appRef.Content.Load<SoundEffect>(file);
        }

        /// <summary>
        /// Helper method to allow easy loading of Song type files.
        /// </summary>
        /// <param name="file">The song file.</param>
        /// <returns>The loaded song object.</returns>
        public Song loadSong(string file)
        {
            return appRef.Content.Load<Song>(file);
        }
    }
}
