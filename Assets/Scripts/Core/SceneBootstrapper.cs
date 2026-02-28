using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.UI;
using Unstack.InputHandling;

namespace Unstack.Core
{
    /// <summary>
    /// Automatically sets up the entire game scene at runtime.
    /// Attach this to an empty GameObject or let it create everything from scratch.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            // Don't bootstrap if GameManager already exists (manual setup)
            if (GameManager.Instance != null) return;

            // Create GameConfig asset at runtime
            var config = ScriptableObject.CreateInstance<GameConfig>();

            // --- Game Manager ---
            var gmGO = new GameObject("GameManager");
            gmGO.AddComponent<LevelManager>();  // LevelManager first so GameManager.Awake() can find it

            // Inject config BEFORE adding GameManager (Awake runs immediately on AddComponent)
            var configField = typeof(GameManager).GetField("config",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var gm = gmGO.AddComponent<GameManager>();
            configField.SetValue(gm, config);

            // --- Input Handler ---
            var inputGO = new GameObject("InputHandler");
            inputGO.AddComponent<TouchInputHandler>();

            // --- Canvas ---
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // --- Hearts Display (top-left) ---
            var heartsGO = new GameObject("HeartsDisplay");
            heartsGO.transform.SetParent(canvasGO.transform, false);
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
            Image[] heartImages = new Image[config.maxHearts];
            for (int i = 0; i < config.maxHearts; i++)
            {
                var heartGO = new GameObject($"Heart_{i}");
                heartGO.transform.SetParent(heartsGO.transform, false);
                var heartRT = heartGO.AddComponent<RectTransform>();
                heartRT.sizeDelta = new Vector2(60, 60);

                var heartImg = heartGO.AddComponent<Image>();
                heartImg.color = new Color(0.91f, 0.30f, 0.24f);
                heartImages[i] = heartImg;
            }

            // Inject heartImages
            var heartImagesField = typeof(HeartsDisplay).GetField("heartImages",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            heartImagesField.SetValue(heartsDisplay, heartImages);

            // --- Level Display (top-center) ---
            var levelGO = new GameObject("LevelDisplay");
            levelGO.transform.SetParent(canvasGO.transform, false);
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
            var levelTextField = typeof(LevelDisplay).GetField("levelText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            levelTextField.SetValue(levelDisplay, levelText);

            // --- Game Over Panel ---
            var gameOverPanelGO = CreatePanel(canvasGO.transform, "GameOverPanel", "Game Over");
            var gameOverPanel = gameOverPanelGO.AddComponent<GameOverPanel>();
            var retryBtn = CreateButton(gameOverPanelGO.transform, "RetryButton", "Retry");
            InjectPanelFields(gameOverPanel, gameOverPanelGO, retryBtn, "titleText", "retryButton");
            gameOverPanelGO.SetActive(false);

            // --- Level Clear Panel ---
            var levelClearPanelGO = CreatePanel(canvasGO.transform, "LevelClearPanel", "Level Clear!");
            var levelClearPanel = levelClearPanelGO.AddComponent<LevelClearPanel>();
            var nextBtn = CreateButton(levelClearPanelGO.transform, "NextButton", "Next");
            InjectPanelFields(levelClearPanel, levelClearPanelGO, nextBtn, "titleText", "nextButton");
            levelClearPanelGO.SetActive(false);

            // --- UI Manager ---
            var uiManagerGO = new GameObject("UIManager");
            var uiManager = uiManagerGO.AddComponent<UIManager>();
            var uiType = typeof(UIManager);
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            uiType.GetField("heartsDisplay", flags).SetValue(uiManager, heartsDisplay);
            uiType.GetField("levelDisplay", flags).SetValue(uiManager, levelDisplay);
            uiType.GetField("gameOverPanel", flags).SetValue(uiManager, gameOverPanel);
            uiType.GetField("levelClearPanel", flags).SetValue(uiManager, levelClearPanel);

            // --- EventSystem (for UI interaction) ---
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private static GameObject CreatePanel(Transform parent, string name, string title)
        {
            var panelGO = new GameObject(name);
            panelGO.transform.SetParent(parent, false);

            var panelRT = panelGO.AddComponent<RectTransform>();
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;

            // Semi-transparent background
            var bg = panelGO.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            // Title text
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

        private static Button CreateButton(Transform parent, string name, string label)
        {
            var btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent, false);

            var btnRT = btnGO.AddComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.5f, 0.35f);
            btnRT.anchorMax = new Vector2(0.5f, 0.35f);
            btnRT.pivot = new Vector2(0.5f, 0.5f);
            btnRT.anchoredPosition = Vector2.zero;
            btnRT.sizeDelta = new Vector2(300, 80);

            var btnImage = btnGO.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.6f, 0.9f);

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            // Button label
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

        private static void InjectPanelFields(Component panel, GameObject panelGO, Button button,
            string titleFieldName, string buttonFieldName)
        {
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var type = panel.GetType();

            var titleTMP = panelGO.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            type.GetField(titleFieldName, flags).SetValue(panel, titleTMP);
            type.GetField(buttonFieldName, flags).SetValue(panel, button);
        }
    }
}
