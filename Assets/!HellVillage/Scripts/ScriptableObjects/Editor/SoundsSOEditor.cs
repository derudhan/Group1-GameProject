#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace HellVillage.AudioManager {
    [CustomEditor(typeof(SoundsSO))]
    public class AudioSOEditor : Editor {
        private void OnEnable() {
            ref MusicList[] musicList = ref ((SoundsSO)target).musicLists;
            ref SoundList[] soundList = ref ((SoundsSO)target).soundLists;

            if (soundList == null || musicList == null) return;

            string[] musicNames = Enum.GetNames(typeof(MusicType));
            string[] audioNames = Enum.GetNames(typeof(SoundType));
            bool differentSize = audioNames.Length != soundList.Length || musicNames.Length != musicList.Length;

            Dictionary<string, MusicList> musics = new();
            Dictionary<string, SoundList> sounds = new();

            if (differentSize) {
                for (int i = 0; i < musicList.Length; ++i) {
                    musics.Add(musicList[i].name, musicList[i]);
                }

                for (int i = 0; i < soundList.Length; ++i) {
                    sounds.Add(soundList[i].name, soundList[i]);
                }
            }


            Array.Resize(ref musicList, musicNames.Length);
            for (int i = 0; i < musicList.Length; i++) {
                string currentName = musicNames[i];
                musicList[i].name = currentName;
                if (musicList[i].volume == 0) musicList[i].volume = 1;

                if (differentSize) {
                    if (musics.ContainsKey(currentName)) {
                        MusicList current = musics[currentName];
                        UpdateElement(ref musicList[i], current.volume, current.audioClip, current.audioMixerGroup);
                    } else
                        UpdateElement(ref musicList[i], 1, null, null);

                    static void UpdateElement(ref MusicList element, float volume, AudioClip sound, AudioMixerGroup mixer) {
                        element.volume = volume;
                        element.audioClip = sound;
                        element.audioMixerGroup = mixer;
                    }
                }
            }

            Array.Resize(ref soundList, audioNames.Length);
            for (int i = 0; i < soundList.Length; i++) {
                string currentName = audioNames[i];
                soundList[i].name = currentName;
                if (soundList[i].volume == 0) soundList[i].volume = 1;

                if (differentSize) {
                    if (sounds.ContainsKey(currentName)) {
                        SoundList current = sounds[currentName];
                        UpdateElement(ref soundList[i], current.volume, current.audioClips, current.audioMixerGroup);
                    } else
                        UpdateElement(ref soundList[i], 1, new AudioClip[0], null);

                    static void UpdateElement(ref SoundList element, float volume, AudioClip[] sounds, AudioMixerGroup mixer) {
                        element.volume = volume;
                        element.audioClips = sounds;
                        element.audioMixerGroup = mixer;
                    }
                }
            }
        }
    }
}
#endif