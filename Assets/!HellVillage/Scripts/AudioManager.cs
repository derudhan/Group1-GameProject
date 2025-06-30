using UnityEngine;
using UnityEngine.Audio;

namespace HellVillage.AudioManager {
    public enum MusicType {
        None,
        TestMusic,
        tost
    }

    public enum SoundType {
        None,
        TestAudio,
        test
    }

    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private SoundsSO soundsSO;

        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioSource soundAudioSource;

        private void Awake() {
            if (Instance == null) Instance = this;
        }

        public void PlayMusic(MusicType music, float volume = 1f) {
            MusicList musicList = soundsSO.musicLists[(int)music];
            AudioClip audioClip = musicList.audioClip;

            musicAudioSource.outputAudioMixerGroup = musicList.audioMixerGroup;
            if (!musicAudioSource.loop) musicAudioSource.loop = true;
            musicAudioSource.PlayOneShot(audioClip, musicList.volume * volume);
        }

        public void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f) {
            SoundList soundList = soundsSO.soundLists[(int)sound];
            AudioClip[] audioClips = soundList.audioClips;
            AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];

            if (source != null) {
                source.outputAudioMixerGroup = soundList.audioMixerGroup;
                source.clip = randomClip;
                source.volume = soundList.volume * volume;
                source.Play();
            } else {
                soundAudioSource.outputAudioMixerGroup = soundList.audioMixerGroup;
                soundAudioSource.PlayOneShot(randomClip, soundList.volume * volume);
            }
        }
    }

    [System.Serializable]
    public struct MusicList {
        [HideInInspector] public string name;
        [Range(0f, 1f)] public float volume;
        public AudioMixerGroup audioMixerGroup;
        public AudioClip audioClip;
    }

    [System.Serializable]
    public struct SoundList {
        [HideInInspector] public string name;
        [Range(0f, 1f)] public float volume;
        public AudioMixerGroup audioMixerGroup;
        public AudioClip[] audioClips;
    }
}
