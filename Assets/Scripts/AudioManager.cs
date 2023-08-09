using UnityEngine;

namespace MatchThreeEngine
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioSource music;
        public AudioSource sfx;
        public AudioClip matchSound;
        public AudioClip[] songs;

        public void PlayBGM()
        {
            if (music != null)
            {
                AudioClip clip = songs[Random.Range(0,songs.Length)];
                if (clip == null) return;
                music.clip = clip;
                music.loop = true;
                music.volume = 1;
                music.Play();
            }
        }
    }
}