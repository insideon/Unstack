using UnityEngine;

namespace Unstack.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource _sfxSource;
        private AudioSource _musicSource;

        private AudioClip _correctTap;
        private AudioClip _wrongTap;
        private AudioClip _levelClear;
        private AudioClip _gameOver;
        private AudioClip _buttonClick;
        private AudioClip _bgm;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Initialize()
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

            _correctTap = ProceduralAudioClipFactory.CreateCorrectTap();
            _wrongTap = ProceduralAudioClipFactory.CreateWrongTap();
            _levelClear = ProceduralAudioClipFactory.CreateLevelClear();
            _gameOver = ProceduralAudioClipFactory.CreateGameOver();
            _buttonClick = ProceduralAudioClipFactory.CreateButtonClick();
            _bgm = ProceduralAudioClipFactory.CreateBGM();

            // Load saved volumes
            _sfxSource.volume = Core.SaveManager.GetSfxVolume();
            _musicSource.volume = Core.SaveManager.GetMusicVolume();
        }

        public void PlayCorrectTap() => _sfxSource?.PlayOneShot(_correctTap);
        public void PlayWrongTap() => _sfxSource?.PlayOneShot(_wrongTap);
        public void PlayLevelClear() => _sfxSource?.PlayOneShot(_levelClear);
        public void PlayGameOver() => _sfxSource?.PlayOneShot(_gameOver);
        public void PlayButtonClick() => _sfxSource?.PlayOneShot(_buttonClick);

        public void StartMusic()
        {
            if (_musicSource != null && !_musicSource.isPlaying)
            {
                _musicSource.clip = _bgm;
                _musicSource.Play();
            }
        }

        public void StopMusic()
        {
            _musicSource?.Stop();
        }

        public void SetSfxVolume(float volume)
        {
            if (_sfxSource != null) _sfxSource.volume = volume;
            Core.SaveManager.SetSfxVolume(volume);
        }

        public void SetMusicVolume(float volume)
        {
            if (_musicSource != null) _musicSource.volume = volume;
            Core.SaveManager.SetMusicVolume(volume);
        }

        public float GetSfxVolume() => _sfxSource != null ? _sfxSource.volume : 1f;
        public float GetMusicVolume() => _musicSource != null ? _musicSource.volume : 1f;
    }
}
