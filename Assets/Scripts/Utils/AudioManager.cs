using System.Collections.Generic;
using UnityEngine;

namespace Perspective.Utils
{
    [System.Serializable]
    public struct SoundEntry
    {
        public string key;
        public AudioClip clip;
    }

    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")] [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Library")] [SerializeField] private List<SoundEntry> soundEntries = new List<SoundEntry>();

        private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();

        protected override void OwnAwake()
        {
            DontDestroyOnLoad(gameObject);

            musicSource.loop = true;
            sfxSource.loop = false;

            foreach (var entry in soundEntries)
            {
                if (!string.IsNullOrEmpty(entry.key) && entry.clip != null && !soundLibrary.ContainsKey(entry.key))
                {
                    soundLibrary.Add(entry.key, entry.clip);
                }
            }
        }

        #region Music Controls

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void PlayMusic(string key, bool loop = true)
        {
            if (soundLibrary.TryGetValue(key, out var clip))
                PlayMusic(clip, loop);
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        #endregion

        #region SFX Controls

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public void PlaySFX(string key)
        {
            if (soundLibrary.TryGetValue(key, out var clip))
                PlaySFX(clip);
        }

        #endregion
    }
}