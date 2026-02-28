using UnityEngine;
using Unstack.Core;

namespace Unstack.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private HeartsDisplay heartsDisplay;
        [SerializeField] private LevelDisplay levelDisplay;
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private LevelClearPanel levelClearPanel;

        private void Start()
        {
            var state = GameManager.Instance.State;
            state.OnHeartsChanged += heartsDisplay.UpdateHearts;
            state.OnLevelChanged += levelDisplay.UpdateLevel;
            state.OnGameOver += ShowGameOver;
            state.OnLevelCleared += ShowLevelClear;

            gameOverPanel.gameObject.SetActive(false);
            levelClearPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            var state = GameManager.Instance.State;
            state.OnHeartsChanged -= heartsDisplay.UpdateHearts;
            state.OnLevelChanged -= levelDisplay.UpdateLevel;
            state.OnGameOver -= ShowGameOver;
            state.OnLevelCleared -= ShowLevelClear;
        }

        private void ShowGameOver()
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.Show();
        }

        private void ShowLevelClear()
        {
            levelClearPanel.gameObject.SetActive(true);
            levelClearPanel.Show();
        }

        public void HidePanels()
        {
            gameOverPanel.gameObject.SetActive(false);
            levelClearPanel.gameObject.SetActive(false);
        }
    }
}
