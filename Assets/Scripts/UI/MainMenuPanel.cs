using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.Core;
using Unstack.Audio;

namespace Unstack.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI highLevelText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;

        public event Action OnPlayClicked;
        public event Action OnSettingsClicked;

        private void Start()
        {
            if (playButton != null)
                playButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlayButtonClick();
                    OnPlayClicked?.Invoke();
                });
            if (settingsButton != null)
                settingsButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlayButtonClick();
                    OnSettingsClicked?.Invoke();
                });
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (titleText != null) titleText.text = "UNSTACK";
            if (highScoreText != null) highScoreText.text = $"High Score: {SaveManager.GetHighScore()}";
            if (highLevelText != null) highLevelText.text = $"Best Level: {SaveManager.GetHighestLevel()}";
        }
    }
}
