using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.Core;
using Unstack.Audio;

namespace Unstack.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button retryButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        private void Start()
        {
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetry);
        }

        public void Show()
        {
            if (titleText != null)
                titleText.text = "Game Over";

            var scoreManager = GameManager.Instance?.ScoreManager;
            if (scoreText != null && scoreManager != null)
                scoreText.text = $"Score: {scoreManager.CurrentScore}";
            if (highScoreText != null)
                highScoreText.text = $"Best: {SaveManager.GetHighScore()}";
        }

        private void OnRetry()
        {
            AudioManager.Instance?.PlayButtonClick();
            gameObject.SetActive(false);
            GameManager.Instance.StartGame();
        }
    }
}
