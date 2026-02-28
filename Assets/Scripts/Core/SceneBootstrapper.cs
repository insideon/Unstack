using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.UI;
using Unstack.Audio;
using Unstack.Animation;
using Unstack.InputHandling;

namespace Unstack.Core
{
    public class SceneBootstrapper : MonoBehaviour
    {
        private static readonly System.Reflection.BindingFlags Flags =
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        private static GameConfig _config;
        private static Transform _canvasTransform;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (GameManager.Instance != null) return;

            _config = ScriptableObject.CreateInstance<GameConfig>();

            BootstrapCore();
            var canvas = BootstrapCanvas();
            _canvasTransform = canvas.transform;
            BootstrapAudio();
            var visualRefs = BootstrapVisualEffects();
            var uiRefs = BootstrapUI();
            var menuRefs = BootstrapMenus();
            WireUIManager(uiRefs, menuRefs, visualRefs);
            BootstrapEventSystem();
        }

        private static void BootstrapCore()
        {
            var gmGO = new GameObject("GameManager");
            gmGO.AddComponent<LevelManager>();

            var configField = typeof(GameManager).GetField("config", Flags);
            var gm = gmGO.AddComponent<GameManager>();
            configField.SetValue(gm, _config);

            var inputGO = new GameObject("InputHandler");
            inputGO.AddComponent<TouchInputHandler>();
        }

        private static GameObject BootstrapCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();
            return canvasGO;
        }

        private static void BootstrapAudio()
        {
            var audioGO = new GameObject("AudioManager");
            var audioManager = audioGO.AddComponent<AudioManager>();
            audioManager.Initialize();
        }

        private struct VisualRefs
        {
            public ScreenEffects screenEffects;
            public LevelTransitionAnimator levelTransitionAnimator;
        }

        private static VisualRefs BootstrapVisualEffects()
        {
            // Background
            var bgGO = new GameObject("Background");
            var bgController = bgGO.AddComponent<BackgroundController>();
            bgController.Initialize(_config.backgroundTopColor, _config.backgroundBottomColor);

            // Flash overlay (fullscreen Image on canvas for screen effects)
            var flashGO = new GameObject("FlashOverlay");
            flashGO.transform.SetParent(_canvasTransform, false);
            var flashRT = flashGO.AddComponent<RectTransform>();
            flashRT.anchorMin = Vector2.zero;
            flashRT.anchorMax = Vector2.one;
            flashRT.offsetMin = Vector2.zero;
            flashRT.offsetMax = Vector2.zero;
            var flashImage = flashGO.AddComponent<Image>();
            flashImage.color = new Color(0, 0, 0, 0);
            flashImage.raycastTarget = false;
            flashGO.SetActive(false);

            var screenEffectsGO = new GameObject("ScreenEffects");
            var screenEffects = screenEffectsGO.AddComponent<ScreenEffects>();
            typeof(ScreenEffects).GetField("flashOverlay", Flags).SetValue(screenEffects, flashImage);

            // Fade overlay (for level transitions)
            var fadeGO = new GameObject("FadeOverlay");
            fadeGO.transform.SetParent(_canvasTransform, false);
            var fadeRT = fadeGO.AddComponent<RectTransform>();
            fadeRT.anchorMin = Vector2.zero;
            fadeRT.anchorMax = Vector2.one;
            fadeRT.offsetMin = Vector2.zero;
            fadeRT.offsetMax = Vector2.zero;
            var fadeImage = fadeGO.AddComponent<Image>();
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false;
            fadeGO.SetActive(false);

            var transitionGO = new GameObject("LevelTransitionAnimator");
            var transitionAnimator = transitionGO.AddComponent<LevelTransitionAnimator>();
            typeof(LevelTransitionAnimator).GetField("fadeOverlay", Flags).SetValue(transitionAnimator, fadeImage);
            transitionAnimator.SetTransitionDuration(_config.levelTransitionDuration);

            GameManager.Instance.SetLevelTransitionAnimator(transitionAnimator);

            return new VisualRefs
            {
                screenEffects = screenEffects,
                levelTransitionAnimator = transitionAnimator
            };
        }

        private struct UIRefs
        {
            public HeartsDisplay heartsDisplay;
            public LevelDisplay levelDisplay;
            public GameOverPanel gameOverPanel;
            public LevelClearPanel levelClearPanel;
            public ScoreDisplay scoreDisplay;
            public ProgressBar progressBar;
        }

        private static UIRefs BootstrapUI()
        {
            // --- Hearts Display (top-left) ---
            var heartsGO = new GameObject("HeartsDisplay");
            heartsGO.transform.SetParent(_canvasTransform, false);
            var heartsRT = heartsGO.AddComponent<RectTransform>();
            heartsRT.anchorMin = new Vector2(0, 1);
            heartsRT.anchorMax = new Vector2(0, 1);
            heartsRT.pivot = new Vector2(0, 1);
            heartsRT.anchoredPosition = new Vector2(40, -40);
            heartsRT.sizeDelta = new Vector2(300, 80);

            var hlg = heartsGO.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            var heartsDisplay = heartsGO.AddComponent<HeartsDisplay>();
            Image[] heartImages = new Image[_config.maxHearts];
            for (int i = 0; i < _config.maxHearts; i++)
            {
                var heartGO = new GameObject($"Heart_{i}");
                heartGO.transform.SetParent(heartsGO.transform, false);
                var heartRT = heartGO.AddComponent<RectTransform>();
                heartRT.sizeDelta = new Vector2(60, 60);
                var heartImg = heartGO.AddComponent<Image>();
                heartImg.color = new Color(0.91f, 0.30f, 0.24f);
                heartImages[i] = heartImg;
            }
            typeof(HeartsDisplay).GetField("heartImages", Flags).SetValue(heartsDisplay, heartImages);

            // --- Level Display (top-center) ---
            var levelGO = new GameObject("LevelDisplay");
            levelGO.transform.SetParent(_canvasTransform, false);
            var levelRT = levelGO.AddComponent<RectTransform>();
            levelRT.anchorMin = new Vector2(0.5f, 1);
            levelRT.anchorMax = new Vector2(0.5f, 1);
            levelRT.pivot = new Vector2(0.5f, 1);
            levelRT.anchoredPosition = new Vector2(0, -40);
            levelRT.sizeDelta = new Vector2(300, 80);

            var levelText = levelGO.AddComponent<TextMeshProUGUI>();
            levelText.text = "Level 1";
            levelText.fontSize = 48;
            levelText.alignment = TextAlignmentOptions.Center;
            levelText.color = Color.white;

            var levelDisplay = levelGO.AddComponent<LevelDisplay>();
            typeof(LevelDisplay).GetField("levelText", Flags).SetValue(levelDisplay, levelText);

            // --- Score Display (top-right) ---
            var scoreGO = new GameObject("ScoreDisplay");
            scoreGO.transform.SetParent(_canvasTransform, false);
            var scoreRT = scoreGO.AddComponent<RectTransform>();
            scoreRT.anchorMin = new Vector2(1, 1);
            scoreRT.anchorMax = new Vector2(1, 1);
            scoreRT.pivot = new Vector2(1, 1);
            scoreRT.anchoredPosition = new Vector2(-40, -40);
            scoreRT.sizeDelta = new Vector2(300, 80);

            var scoreTextComp = scoreGO.AddComponent<TextMeshProUGUI>();
            scoreTextComp.text = "0";
            scoreTextComp.fontSize = 48;
            scoreTextComp.alignment = TextAlignmentOptions.Right;
            scoreTextComp.color = Color.white;

            // Combo text (below score)
            var comboGO = new GameObject("ComboText");
            comboGO.transform.SetParent(scoreGO.transform, false);
            var comboRT = comboGO.AddComponent<RectTransform>();
            comboRT.anchorMin = new Vector2(1, 0);
            comboRT.anchorMax = new Vector2(1, 0);
            comboRT.pivot = new Vector2(1, 1);
            comboRT.anchoredPosition = new Vector2(0, -5);
            comboRT.sizeDelta = new Vector2(300, 50);

            var comboText = comboGO.AddComponent<TextMeshProUGUI>();
            comboText.text = "";
            comboText.fontSize = 32;
            comboText.alignment = TextAlignmentOptions.Right;
            comboText.color = new Color(1f, 0.84f, 0f); // Gold
            comboGO.SetActive(false);

            var scoreDisplay = scoreGO.AddComponent<ScoreDisplay>();
            typeof(ScoreDisplay).GetField("scoreText", Flags).SetValue(scoreDisplay, scoreTextComp);
            typeof(ScoreDisplay).GetField("comboText", Flags).SetValue(scoreDisplay, comboText);

            // --- Progress Bar (bottom) ---
            var progressGO = new GameObject("ProgressBar");
            progressGO.transform.SetParent(_canvasTransform, false);
            var progressRT = progressGO.AddComponent<RectTransform>();
            progressRT.anchorMin = new Vector2(0, 0);
            progressRT.anchorMax = new Vector2(1, 0);
            progressRT.pivot = new Vector2(0.5f, 0);
            progressRT.anchoredPosition = new Vector2(0, 20);
            progressRT.sizeDelta = new Vector2(-80, 20);

            // Background bar
            var barBg = progressGO.AddComponent<Image>();
            barBg.color = new Color(1, 1, 1, 0.2f);

            // Fill bar
            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(progressGO.transform, false);
            var fillRT = fillGO.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = Vector2.zero;
            fillRT.offsetMax = Vector2.zero;

            var fillImage = fillGO.AddComponent<Image>();
            fillImage.color = new Color(0.2f, 0.8f, 0.4f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 0f;

            // "N left" text
            var remainGO = new GameObject("RemainingText");
            remainGO.transform.SetParent(progressGO.transform, false);
            var remainRT = remainGO.AddComponent<RectTransform>();
            remainRT.anchorMin = Vector2.zero;
            remainRT.anchorMax = Vector2.one;
            remainRT.offsetMin = Vector2.zero;
            remainRT.offsetMax = new Vector2(0, 30);

            var remainText = remainGO.AddComponent<TextMeshProUGUI>();
            remainText.text = "";
            remainText.fontSize = 24;
            remainText.alignment = TextAlignmentOptions.Center;
            remainText.color = new Color(1, 1, 1, 0.7f);

            var progressBar = progressGO.AddComponent<ProgressBar>();
            typeof(ProgressBar).GetField("fillImage", Flags).SetValue(progressBar, fillImage);
            typeof(ProgressBar).GetField("remainingText", Flags).SetValue(progressBar, remainText);

            // --- Game Over Panel ---
            var gameOverPanelGO = CreatePanel(_canvasTransform, "GameOverPanel", "Game Over");
            var gameOverPanel = gameOverPanelGO.AddComponent<GameOverPanel>();
            var retryBtn = CreateButton(gameOverPanelGO.transform, "RetryButton", "Retry", 0.25f);
            retryBtn.GetComponent<Image>().color = new Color(0.18f, 0.78f, 0.44f);
            var retryBtnRT = retryBtn.GetComponent<RectTransform>();
            retryBtnRT.sizeDelta = new Vector2(350, 90);

            // Score text in game over panel
            var goScoreGO = new GameObject("ScoreText");
            goScoreGO.transform.SetParent(gameOverPanelGO.transform, false);
            var goScoreRT = goScoreGO.AddComponent<RectTransform>();
            goScoreRT.anchorMin = new Vector2(0.5f, 0.5f);
            goScoreRT.anchorMax = new Vector2(0.5f, 0.5f);
            goScoreRT.pivot = new Vector2(0.5f, 0.5f);
            goScoreRT.anchoredPosition = Vector2.zero;
            goScoreRT.sizeDelta = new Vector2(600, 60);
            var goScoreText = goScoreGO.AddComponent<TextMeshProUGUI>();
            goScoreText.fontSize = 48;
            goScoreText.alignment = TextAlignmentOptions.Center;
            goScoreText.color = Color.white;

            // High score text in game over panel
            var goHighGO = new GameObject("HighScoreText");
            goHighGO.transform.SetParent(gameOverPanelGO.transform, false);
            var goHighRT = goHighGO.AddComponent<RectTransform>();
            goHighRT.anchorMin = new Vector2(0.5f, 0.45f);
            goHighRT.anchorMax = new Vector2(0.5f, 0.45f);
            goHighRT.pivot = new Vector2(0.5f, 0.5f);
            goHighRT.anchoredPosition = Vector2.zero;
            goHighRT.sizeDelta = new Vector2(600, 50);
            var goHighText = goHighGO.AddComponent<TextMeshProUGUI>();
            goHighText.fontSize = 36;
            goHighText.alignment = TextAlignmentOptions.Center;
            goHighText.color = new Color(1f, 0.84f, 0f);

            InjectPanelFields(gameOverPanel, gameOverPanelGO, retryBtn, "titleText", "retryButton");
            typeof(GameOverPanel).GetField("scoreText", Flags).SetValue(gameOverPanel, goScoreText);
            typeof(GameOverPanel).GetField("highScoreText", Flags).SetValue(gameOverPanel, goHighText);
            gameOverPanelGO.SetActive(false);

            // --- Level Clear Panel ---
            var levelClearPanelGO = CreatePanel(_canvasTransform, "LevelClearPanel", "Level Clear!");
            var levelClearPanel = levelClearPanelGO.AddComponent<LevelClearPanel>();
            var nextBtn = CreateButton(levelClearPanelGO.transform, "NextButton", "Next", 0.35f);
            InjectPanelFields(levelClearPanel, levelClearPanelGO, nextBtn, "titleText", "nextButton");
            levelClearPanelGO.SetActive(false);

            return new UIRefs
            {
                heartsDisplay = heartsDisplay,
                levelDisplay = levelDisplay,
                gameOverPanel = gameOverPanel,
                levelClearPanel = levelClearPanel,
                scoreDisplay = scoreDisplay,
                progressBar = progressBar
            };
        }

        private struct MenuRefs
        {
            public MainMenuPanel mainMenuPanel;
            public SettingsPanel settingsPanel;
            public TutorialOverlay tutorialOverlay;
            public PauseButton pauseButton;
        }

        private static MenuRefs BootstrapMenus()
        {
            // --- Main Menu Panel ---
            var menuGO = new GameObject("MainMenuPanel");
            menuGO.transform.SetParent(_canvasTransform, false);
            var menuRT = menuGO.AddComponent<RectTransform>();
            menuRT.anchorMin = Vector2.zero;
            menuRT.anchorMax = Vector2.one;
            menuRT.offsetMin = Vector2.zero;
            menuRT.offsetMax = Vector2.zero;

            var menuBg = menuGO.AddComponent<Image>();
            menuBg.color = new Color(0.08f, 0.08f, 0.2f, 0.95f);

            // Title
            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(menuGO.transform, false);
            var titleRT = titleGO.AddComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.5f, 0.7f);
            titleRT.anchorMax = new Vector2(0.5f, 0.7f);
            titleRT.pivot = new Vector2(0.5f, 0.5f);
            titleRT.anchoredPosition = Vector2.zero;
            titleRT.sizeDelta = new Vector2(800, 150);
            var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "UNSTACK";
            titleTMP.fontSize = 96;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = Color.white;
            titleTMP.fontStyle = FontStyles.Bold;

            // High score
            var hsGO = new GameObject("HighScore");
            hsGO.transform.SetParent(menuGO.transform, false);
            var hsRT = hsGO.AddComponent<RectTransform>();
            hsRT.anchorMin = new Vector2(0.5f, 0.58f);
            hsRT.anchorMax = new Vector2(0.5f, 0.58f);
            hsRT.pivot = new Vector2(0.5f, 0.5f);
            hsRT.anchoredPosition = Vector2.zero;
            hsRT.sizeDelta = new Vector2(600, 50);
            var hsTMP = hsGO.AddComponent<TextMeshProUGUI>();
            hsTMP.fontSize = 32;
            hsTMP.alignment = TextAlignmentOptions.Center;
            hsTMP.color = new Color(1f, 0.84f, 0f);

            // Highest level
            var hlGO = new GameObject("HighLevel");
            hlGO.transform.SetParent(menuGO.transform, false);
            var hlRT = hlGO.AddComponent<RectTransform>();
            hlRT.anchorMin = new Vector2(0.5f, 0.53f);
            hlRT.anchorMax = new Vector2(0.5f, 0.53f);
            hlRT.pivot = new Vector2(0.5f, 0.5f);
            hlRT.anchoredPosition = Vector2.zero;
            hlRT.sizeDelta = new Vector2(600, 50);
            var hlTMP = hlGO.AddComponent<TextMeshProUGUI>();
            hlTMP.fontSize = 28;
            hlTMP.alignment = TextAlignmentOptions.Center;
            hlTMP.color = new Color(0.8f, 0.8f, 0.8f);

            // Play button (large, green, prominent)
            var playBtn = CreateButton(menuGO.transform, "PlayButton", "PLAY", 0.40f);
            var playBtnRT = playBtn.GetComponent<RectTransform>();
            playBtnRT.sizeDelta = new Vector2(400, 100);
            playBtn.GetComponent<Image>().color = new Color(0.18f, 0.78f, 0.44f);
            var playLabel = playBtn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (playLabel != null) playLabel.fontSize = 52;

            // Settings button (smaller, subtle)
            var settingsBtn = CreateButton(menuGO.transform, "SettingsButton", "Settings", 0.28f);
            var settingsBtnRT = settingsBtn.GetComponent<RectTransform>();
            settingsBtnRT.sizeDelta = new Vector2(240, 60);
            settingsBtn.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.5f);
            var settingsLabel = settingsBtn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
            if (settingsLabel != null) settingsLabel.fontSize = 32;

            var mainMenuPanel = menuGO.AddComponent<MainMenuPanel>();
            typeof(MainMenuPanel).GetField("titleText", Flags).SetValue(mainMenuPanel, titleTMP);
            typeof(MainMenuPanel).GetField("highScoreText", Flags).SetValue(mainMenuPanel, hsTMP);
            typeof(MainMenuPanel).GetField("highLevelText", Flags).SetValue(mainMenuPanel, hlTMP);
            typeof(MainMenuPanel).GetField("playButton", Flags).SetValue(mainMenuPanel, playBtn);
            typeof(MainMenuPanel).GetField("settingsButton", Flags).SetValue(mainMenuPanel, settingsBtn);

            // --- Settings Panel ---
            var settingsGO = new GameObject("SettingsPanel");
            settingsGO.transform.SetParent(_canvasTransform, false);
            var settingsRT = settingsGO.AddComponent<RectTransform>();
            settingsRT.anchorMin = Vector2.zero;
            settingsRT.anchorMax = Vector2.one;
            settingsRT.offsetMin = Vector2.zero;
            settingsRT.offsetMax = Vector2.zero;

            var settingsBg = settingsGO.AddComponent<Image>();
            settingsBg.color = new Color(0, 0, 0, 0.85f);

            // Settings title
            var sTitleGO = new GameObject("SettingsTitle");
            sTitleGO.transform.SetParent(settingsGO.transform, false);
            var sTitleRT = sTitleGO.AddComponent<RectTransform>();
            sTitleRT.anchorMin = new Vector2(0.5f, 0.7f);
            sTitleRT.anchorMax = new Vector2(0.5f, 0.7f);
            sTitleRT.pivot = new Vector2(0.5f, 0.5f);
            sTitleRT.anchoredPosition = Vector2.zero;
            sTitleRT.sizeDelta = new Vector2(600, 80);
            var sTitleTMP = sTitleGO.AddComponent<TextMeshProUGUI>();
            sTitleTMP.text = "Settings";
            sTitleTMP.fontSize = 60;
            sTitleTMP.alignment = TextAlignmentOptions.Center;
            sTitleTMP.color = Color.white;

            // SFX slider
            var sfxSlider = CreateSlider(settingsGO.transform, "SFX Volume", 0.55f);
            // Music slider
            var musicSlider = CreateSlider(settingsGO.transform, "Music Volume", 0.45f);

            // Close button
            var closeBtn = CreateButton(settingsGO.transform, "CloseButton", "Close", 0.30f);

            var settingsPanel = settingsGO.AddComponent<SettingsPanel>();
            typeof(SettingsPanel).GetField("sfxSlider", Flags).SetValue(settingsPanel, sfxSlider);
            typeof(SettingsPanel).GetField("musicSlider", Flags).SetValue(settingsPanel, musicSlider);
            typeof(SettingsPanel).GetField("closeButton", Flags).SetValue(settingsPanel, closeBtn);
            settingsGO.SetActive(false);

            // --- Tutorial Overlay ---
            var tutGO = new GameObject("TutorialOverlay");
            tutGO.transform.SetParent(_canvasTransform, false);
            var tutRT = tutGO.AddComponent<RectTransform>();
            tutRT.anchorMin = Vector2.zero;
            tutRT.anchorMax = Vector2.one;
            tutRT.offsetMin = Vector2.zero;
            tutRT.offsetMax = Vector2.zero;

            var tutBg = tutGO.AddComponent<Image>();
            tutBg.color = new Color(0, 0, 0, 0.4f);
            tutBg.raycastTarget = false;

            // Instruction text
            var instrGO = new GameObject("InstructionText");
            instrGO.transform.SetParent(tutGO.transform, false);
            var instrRT = instrGO.AddComponent<RectTransform>();
            instrRT.anchorMin = new Vector2(0.5f, 0.65f);
            instrRT.anchorMax = new Vector2(0.5f, 0.65f);
            instrRT.pivot = new Vector2(0.5f, 0.5f);
            instrRT.anchoredPosition = Vector2.zero;
            instrRT.sizeDelta = new Vector2(700, 150);
            var instrTMP = instrGO.AddComponent<TextMeshProUGUI>();
            instrTMP.text = "Tap the topmost shape\nto unstack!";
            instrTMP.fontSize = 42;
            instrTMP.alignment = TextAlignmentOptions.Center;
            instrTMP.color = Color.white;
            instrTMP.raycastTarget = false;

            // Arrow indicator
            var arrowGO = new GameObject("Arrow");
            arrowGO.transform.SetParent(tutGO.transform, false);
            var arrowRT = arrowGO.AddComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRT.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRT.pivot = new Vector2(0.5f, 0.5f);
            arrowRT.anchoredPosition = Vector2.zero;
            arrowRT.sizeDelta = new Vector2(60, 60);
            var arrowTMP = arrowGO.AddComponent<TextMeshProUGUI>();
            arrowTMP.text = "\u2193"; // down arrow
            arrowTMP.fontSize = 60;
            arrowTMP.alignment = TextAlignmentOptions.Center;
            arrowTMP.color = Color.white;
            arrowTMP.raycastTarget = false;

            var tutorialOverlay = tutGO.AddComponent<TutorialOverlay>();
            typeof(TutorialOverlay).GetField("overlayBackground", Flags).SetValue(tutorialOverlay, tutBg);
            typeof(TutorialOverlay).GetField("instructionText", Flags).SetValue(tutorialOverlay, instrTMP);
            typeof(TutorialOverlay).GetField("arrowTransform", Flags).SetValue(tutorialOverlay, arrowRT);
            tutGO.SetActive(false);

            // --- Pause Button (top-right, below score) ---
            var pauseBtnGO = new GameObject("PauseButton");
            pauseBtnGO.transform.SetParent(_canvasTransform, false);
            var pauseBtnRT = pauseBtnGO.AddComponent<RectTransform>();
            pauseBtnRT.anchorMin = new Vector2(1, 1);
            pauseBtnRT.anchorMax = new Vector2(1, 1);
            pauseBtnRT.pivot = new Vector2(1, 1);
            pauseBtnRT.anchoredPosition = new Vector2(-40, -130);
            pauseBtnRT.sizeDelta = new Vector2(80, 80);

            var pauseBtnImg = pauseBtnGO.AddComponent<Image>();
            pauseBtnImg.color = new Color(1, 1, 1, 0.3f);
            var pauseBtnComp = pauseBtnGO.AddComponent<Button>();
            pauseBtnComp.targetGraphic = pauseBtnImg;

            var pauseLabelGO = new GameObject("Label");
            pauseLabelGO.transform.SetParent(pauseBtnGO.transform, false);
            var pauseLabelRT = pauseLabelGO.AddComponent<RectTransform>();
            pauseLabelRT.anchorMin = Vector2.zero;
            pauseLabelRT.anchorMax = Vector2.one;
            pauseLabelRT.offsetMin = Vector2.zero;
            pauseLabelRT.offsetMax = Vector2.zero;
            var pauseLabelTMP = pauseLabelGO.AddComponent<TextMeshProUGUI>();
            pauseLabelTMP.text = "||";
            pauseLabelTMP.fontSize = 36;
            pauseLabelTMP.alignment = TextAlignmentOptions.Center;
            pauseLabelTMP.color = Color.white;

            // Pause panel
            var pausePanelGO = new GameObject("PausePanel");
            pausePanelGO.transform.SetParent(_canvasTransform, false);
            var pausePanelRT = pausePanelGO.AddComponent<RectTransform>();
            pausePanelRT.anchorMin = Vector2.zero;
            pausePanelRT.anchorMax = Vector2.one;
            pausePanelRT.offsetMin = Vector2.zero;
            pausePanelRT.offsetMax = Vector2.zero;

            var pausePanelBg = pausePanelGO.AddComponent<Image>();
            pausePanelBg.color = new Color(0, 0, 0, 0.7f);

            var pauseTitleGO = new GameObject("PauseTitle");
            pauseTitleGO.transform.SetParent(pausePanelGO.transform, false);
            var pauseTitleRT = pauseTitleGO.AddComponent<RectTransform>();
            pauseTitleRT.anchorMin = new Vector2(0.5f, 0.65f);
            pauseTitleRT.anchorMax = new Vector2(0.5f, 0.65f);
            pauseTitleRT.pivot = new Vector2(0.5f, 0.5f);
            pauseTitleRT.anchoredPosition = Vector2.zero;
            pauseTitleRT.sizeDelta = new Vector2(600, 100);
            var pauseTitleTMP = pauseTitleGO.AddComponent<TextMeshProUGUI>();
            pauseTitleTMP.text = "Paused";
            pauseTitleTMP.fontSize = 64;
            pauseTitleTMP.alignment = TextAlignmentOptions.Center;
            pauseTitleTMP.color = Color.white;

            var resumeBtn = CreateButton(pausePanelGO.transform, "ResumeButton", "Resume", 0.45f);
            var pSettingsBtn = CreateButton(pausePanelGO.transform, "PauseSettingsButton", "Settings", 0.35f);
            pausePanelGO.SetActive(false);

            var pauseButton = pauseBtnGO.AddComponent<PauseButton>();
            typeof(PauseButton).GetField("pauseBtn", Flags).SetValue(pauseButton, pauseBtnComp);
            typeof(PauseButton).GetField("pausePanel", Flags).SetValue(pauseButton, pausePanelGO);
            typeof(PauseButton).GetField("resumeButton", Flags).SetValue(pauseButton, resumeBtn);
            typeof(PauseButton).GetField("settingsButton", Flags).SetValue(pauseButton, pSettingsBtn);

            return new MenuRefs
            {
                mainMenuPanel = mainMenuPanel,
                settingsPanel = settingsPanel,
                tutorialOverlay = tutorialOverlay,
                pauseButton = pauseButton
            };
        }

        private static void WireUIManager(UIRefs ui, MenuRefs menus, VisualRefs visuals)
        {
            var uiManagerGO = new GameObject("UIManager");
            var uiManager = uiManagerGO.AddComponent<UIManager>();
            var uiType = typeof(UIManager);

            uiType.GetField("heartsDisplay", Flags).SetValue(uiManager, ui.heartsDisplay);
            uiType.GetField("levelDisplay", Flags).SetValue(uiManager, ui.levelDisplay);
            uiType.GetField("gameOverPanel", Flags).SetValue(uiManager, ui.gameOverPanel);
            uiType.GetField("levelClearPanel", Flags).SetValue(uiManager, ui.levelClearPanel);
            uiType.GetField("scoreDisplay", Flags).SetValue(uiManager, ui.scoreDisplay);
            uiType.GetField("progressBar", Flags).SetValue(uiManager, ui.progressBar);
            uiType.GetField("mainMenuPanel", Flags).SetValue(uiManager, menus.mainMenuPanel);
            uiType.GetField("settingsPanel", Flags).SetValue(uiManager, menus.settingsPanel);
            uiType.GetField("tutorialOverlay", Flags).SetValue(uiManager, menus.tutorialOverlay);
            uiType.GetField("pauseButton", Flags).SetValue(uiManager, menus.pauseButton);
            uiType.GetField("screenEffects", Flags).SetValue(uiManager, visuals.screenEffects);
        }

        private static void BootstrapEventSystem()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        // --- Helper Methods ---

        private static GameObject CreatePanel(Transform parent, string name, string title)
        {
            var panelGO = new GameObject(name);
            panelGO.transform.SetParent(parent, false);

            var panelRT = panelGO.AddComponent<RectTransform>();
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;

            var bg = panelGO.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panelGO.transform, false);
            var titleRT = titleGO.AddComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0.5f, 0.6f);
            titleRT.anchorMax = new Vector2(0.5f, 0.6f);
            titleRT.pivot = new Vector2(0.5f, 0.5f);
            titleRT.anchoredPosition = Vector2.zero;
            titleRT.sizeDelta = new Vector2(600, 120);

            var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
            titleTMP.text = title;
            titleTMP.fontSize = 72;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = Color.white;

            return panelGO;
        }

        private static Button CreateButton(Transform parent, string name, string label, float anchorY = 0.35f)
        {
            var btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent, false);

            var btnRT = btnGO.AddComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.5f, anchorY);
            btnRT.anchorMax = new Vector2(0.5f, anchorY);
            btnRT.pivot = new Vector2(0.5f, 0.5f);
            btnRT.anchoredPosition = Vector2.zero;
            btnRT.sizeDelta = new Vector2(300, 80);

            var btnImage = btnGO.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.6f, 0.9f);

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(btnGO.transform, false);
            var labelRT = labelGO.AddComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = Vector2.zero;
            labelRT.offsetMax = Vector2.zero;

            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = label;
            labelTMP.fontSize = 42;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;

            return btn;
        }

        private static Slider CreateSlider(Transform parent, string label, float anchorY)
        {
            // Container
            var containerGO = new GameObject($"Slider_{label}");
            containerGO.transform.SetParent(parent, false);
            var containerRT = containerGO.AddComponent<RectTransform>();
            containerRT.anchorMin = new Vector2(0.5f, anchorY);
            containerRT.anchorMax = new Vector2(0.5f, anchorY);
            containerRT.pivot = new Vector2(0.5f, 0.5f);
            containerRT.anchoredPosition = Vector2.zero;
            containerRT.sizeDelta = new Vector2(600, 80);

            // Label
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(containerGO.transform, false);
            var labelRT = labelGO.AddComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 0.5f);
            labelRT.anchorMax = new Vector2(0.35f, 0.5f);
            labelRT.pivot = new Vector2(0, 0.5f);
            labelRT.anchoredPosition = Vector2.zero;
            labelRT.sizeDelta = new Vector2(0, 50);
            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = label;
            labelTMP.fontSize = 28;
            labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            labelTMP.color = Color.white;

            // Slider
            var sliderGO = new GameObject("Slider");
            sliderGO.transform.SetParent(containerGO.transform, false);
            var sliderRT = sliderGO.AddComponent<RectTransform>();
            sliderRT.anchorMin = new Vector2(0.38f, 0.5f);
            sliderRT.anchorMax = new Vector2(1f, 0.5f);
            sliderRT.pivot = new Vector2(0.5f, 0.5f);
            sliderRT.anchoredPosition = Vector2.zero;
            sliderRT.sizeDelta = new Vector2(0, 30);

            // Background
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(sliderGO.transform, false);
            var bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = new Color(0.3f, 0.3f, 0.3f);

            // Fill Area
            var fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(sliderGO.transform, false);
            var fillAreaRT = fillAreaGO.AddComponent<RectTransform>();
            fillAreaRT.anchorMin = Vector2.zero;
            fillAreaRT.anchorMax = new Vector2(1, 1);
            fillAreaRT.offsetMin = new Vector2(5, 0);
            fillAreaRT.offsetMax = new Vector2(-5, 0);

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform, false);
            var fillRT = fillGO.AddComponent<RectTransform>();
            fillRT.anchorMin = Vector2.zero;
            fillRT.anchorMax = Vector2.one;
            fillRT.offsetMin = Vector2.zero;
            fillRT.offsetMax = Vector2.zero;
            var fillImg = fillGO.AddComponent<Image>();
            fillImg.color = new Color(0.2f, 0.6f, 0.9f);

            // Handle Slide Area
            var handleAreaGO = new GameObject("Handle Slide Area");
            handleAreaGO.transform.SetParent(sliderGO.transform, false);
            var handleAreaRT = handleAreaGO.AddComponent<RectTransform>();
            handleAreaRT.anchorMin = Vector2.zero;
            handleAreaRT.anchorMax = Vector2.one;
            handleAreaRT.offsetMin = new Vector2(10, 0);
            handleAreaRT.offsetMax = new Vector2(-10, 0);

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(handleAreaGO.transform, false);
            var handleRT = handleGO.AddComponent<RectTransform>();
            handleRT.sizeDelta = new Vector2(30, 30);
            var handleImg = handleGO.AddComponent<Image>();
            handleImg.color = Color.white;

            var slider = sliderGO.AddComponent<Slider>();
            slider.fillRect = fillRT;
            slider.handleRect = handleRT;
            slider.targetGraphic = handleImg;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            return slider;
        }

        private static void InjectPanelFields(Component panel, GameObject panelGO, Button button,
            string titleFieldName, string buttonFieldName)
        {
            var type = panel.GetType();
            var titleTMP = panelGO.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            type.GetField(titleFieldName, Flags).SetValue(panel, titleTMP);
            type.GetField(buttonFieldName, Flags).SetValue(panel, button);
        }
    }
}
