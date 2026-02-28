using UnityEngine;
using Unstack.Core;
using Unstack.Animation;

namespace Unstack.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private HeartsDisplay heartsDisplay;
        [SerializeField] private LevelDisplay levelDisplay;
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private LevelClearPanel levelClearPanel;
        [SerializeField] private ScoreDisplay scoreDisplay;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private MainMenuPanel mainMenuPanel;
        [SerializeField] private SettingsPanel settingsPanel;
        [SerializeField] private TutorialOverlay tutorialOverlay;
        [SerializeField] private PauseButton pauseButton;
        [SerializeField] private ScreenEffects screenEffects;

        private void Start()
        {
            var gm = GameManager.Instance;
            var state = gm.State;

            state.OnHeartsChanged += heartsDisplay.UpdateHearts;
            state.OnLevelChanged += levelDisplay.UpdateLevel;
            state.OnGameOver += ShowGameOver;
            state.OnLevelCleared += ShowLevelClear;

            // Score display
            if (scoreDisplay != null)
            {
                gm.ScoreManager.OnScoreChanged += scoreDisplay.UpdateScore;
                gm.ScoreManager.OnComboChanged += scoreDisplay.UpdateCombo;
            }

            // Progress bar
            if (progressBar != null)
            {
                state.OnShapesRemainingChanged += progressBar.UpdateRemaining;
                state.OnLevelChanged += (level) =>
                {
                    progressBar.SetTotal(gm.Config.GetShapeCount(level));
                };
            }

            // Main menu
            if (mainMenuPanel != null)
            {
                mainMenuPanel.OnPlayClicked += OnMenuPlay;
                mainMenuPanel.OnSettingsClicked += () => settingsPanel?.Show();
                mainMenuPanel.Show();
            }

            // Pause button
            if (pauseButton != null)
            {
                pauseButton.OnSettingsClicked += () => settingsPanel?.Show();
                pauseButton.SetVisible(false);
            }

            // Tutorial
            if (tutorialOverlay != null)
            {
                gm.OnFirstShapeRemoved += () => tutorialOverlay?.Dismiss();
            }

            // Screen effects
            if (screenEffects != null)
            {
                screenEffects.SetFlashDuration(gm.Config.screenFlashDuration);
            }

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

            if (scoreDisplay != null)
            {
                GameManager.Instance.ScoreManager.OnScoreChanged -= scoreDisplay.UpdateScore;
                GameManager.Instance.ScoreManager.OnComboChanged -= scoreDisplay.UpdateCombo;
            }
        }

        private void OnMenuPlay()
        {
            mainMenuPanel?.gameObject.SetActive(false);
            pauseButton?.SetVisible(true);
            GameManager.Instance.StartGame();

            // Show tutorial for first-time players
            if (!SaveManager.HasPlayedBefore() && tutorialOverlay != null)
            {
                tutorialOverlay.Show();
            }
        }

        private void ShowGameOver()
        {
            pauseButton?.SetVisible(false);
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.Show();
        }

        private void ShowLevelClear()
        {
            levelClearPanel.gameObject.SetActive(true);
            levelClearPanel.Show();
            screenEffects?.FlashWhite();
        }

        public void HidePanels()
        {
            gameOverPanel.gameObject.SetActive(false);
            levelClearPanel.gameObject.SetActive(false);
        }

        public void ShowMainMenu()
        {
            HidePanels();
            pauseButton?.SetVisible(false);
            mainMenuPanel?.Show();
        }
    }
}
