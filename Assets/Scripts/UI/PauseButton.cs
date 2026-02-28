using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.Audio;

namespace Unstack.UI
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private Button pauseBtn;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;

        public event System.Action OnSettingsClicked;

        private bool _isPaused;

        private void Start()
        {
            if (pauseBtn != null)
                pauseBtn.onClick.AddListener(TogglePause);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlayButtonClick();
                    Resume();
                });

            if (settingsButton != null)
                settingsButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlayButtonClick();
                    OnSettingsClicked?.Invoke();
                });
        }

        public void TogglePause()
        {
            AudioManager.Instance?.PlayButtonClick();
            if (_isPaused)
                Resume();
            else
                Pause();
        }

        private void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        public void SetVisible(bool visible)
        {
            if (pauseBtn != null)
                pauseBtn.gameObject.SetActive(visible);
        }
    }
}
