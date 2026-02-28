using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.Core;

namespace Unstack.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button retryButton;

        private void Awake()
        {
            if (retryButton != null)
                retryButton.onClick.AddListener(OnRetry);
        }

        public void Show()
        {
            if (titleText != null)
                titleText.text = "Game Over";
        }

        private void OnRetry()
        {
            gameObject.SetActive(false);
            GameManager.Instance.StartGame();
        }
    }
}
