using UnityEngine;

namespace HellVillage.AudioManager {
    [CreateAssetMenu(fileName = "SoundsSO", menuName = "HellVillage/SoundsSO", order = 1)]
    public class SoundsSO : ScriptableObject {
        public MusicList[] musicLists;
        public SoundList[] soundLists;
    }
}