using HellVillage.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace HellVillage.Data {
    public enum AudioVolumeType {
        None,
        Master,
        Music,
        SFX
    }

    /// <summary>
    /// This class manages audio settings for the game, allowing for volume control and muting.
    /// It provides methods to set and get audio volumes, reset volumes to default values, and mute/unmute audio.
    /// The audio settings are saved and loaded using PlayerPrefs, ensuring that user preferences persist across game sessions.
    /// </summary>
    public class AudioOptions : MonoBehaviour {
        public event System.Action OnResetVolumes;

        [SerializeField] private AudioMixer _audioMixer;

        private const string _mixerPath = "DefaultMixer";
        private const float _defaultVolume = 0.8f;

        private bool _isMuted = false;

        private void Awake() {
            if (_audioMixer == null) _audioMixer = Resources.Load<AudioMixer>(_mixerPath);
        }

        private void Start() {
            LoadConfigurations();
        }

        #region Private Methods

        private void LoadConfigurations() {
            foreach (AudioVolumeType audioVolumeType in System.Enum.GetValues(typeof(AudioVolumeType))) {
                if (audioVolumeType == AudioVolumeType.None) continue;
                float volume = DataManager.Instance.TryLoadFromPlayerPrefs(audioVolumeType.ToString(), _defaultVolume);
                SetVolume(audioVolumeType, volume);
            }
        }

        private float LoadConfiguratioAsDecibel(AudioVolumeType audioVolumeType) {
            float volume = DataManager.Instance.TryLoadFromPlayerPrefs(audioVolumeType.ToString(), _defaultVolume);
            volume = Helpers.LinearToDecibel(volume);
            return volume;
        }

        #endregion

        #region Public Methods

        public void SetAudioMixerMute(bool mute) {
            _isMuted = mute;
            _audioMixer.SetFloat(
                AudioVolumeType.Master.ToString(),
                mute ? -80f : LoadConfiguratioAsDecibel(AudioVolumeType.Master)
            );
        }

        public void SetVolume(AudioVolumeType audioVolumeType, float volume) {
            float decibelValue = Helpers.LinearToDecibel(volume);
            DataManager.Instance.SaveToPlayerPrefs(audioVolumeType.ToString(), volume);

            if (!_isMuted) {
                _audioMixer.SetFloat(audioVolumeType.ToString(), decibelValue);
            }
        }

        public float GetVolume(AudioVolumeType audioVolumeType) {
            _audioMixer.GetFloat(audioVolumeType.ToString(), out float decibelValue);
            return Helpers.DecibelToLinear(decibelValue);
        }

        public void ResetVolume(AudioVolumeType audioVolumeType) {
            SetVolume(audioVolumeType, _defaultVolume);
            OnResetVolumes?.Invoke();
        }

        public void ResetAllVolumes() {
            foreach (AudioVolumeType audioVolumeType in System.Enum.GetValues(typeof(AudioVolumeType))) {
                if (audioVolumeType == AudioVolumeType.None) continue;
                ResetVolume(audioVolumeType);
            }
        }

        public bool IsMuted() {
            return _isMuted;
        }

        #endregion
    }
}
