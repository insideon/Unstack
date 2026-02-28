using UnityEngine;
using UnityEngine.UI;
using Unstack.Core;
using Unstack.Audio;

namespace Unstack.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Button closeButton;

        private bool _listenersRegistered;

        private void Start()
        {
            RegisterListeners();
        }

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void RegisterListeners()
        {
            if (_listenersRegistered) return;
            if (closeButton == null) return;

            _listenersRegistered = true;
            closeButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayButtonClick();
                gameObject.SetActive(false);
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (sfxSlider != null)
            {
                sfxSlider.value = SaveManager.GetSfxVolume();
                sfxSlider.onValueChanged.RemoveAllListeners();
                sfxSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetSfxVolume(v));
            }

            if (musicSlider != null)
            {
                musicSlider.value = SaveManager.GetMusicVolume();
                musicSlider.onValueChanged.RemoveAllListeners();
                musicSlider.onValueChanged.AddListener(v => AudioManager.Instance?.SetMusicVolume(v));
            }
        }
    }
}
