using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unstack.Core;

namespace Unstack.UI
{
    public class LevelClearPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button nextButton;

        private void Awake()
        {
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNext);
        }

        public void Show()
        {
            if (titleText != null)
                titleText.text = "Level Clear!";
        }

        private void OnNext()
        {
            gameObject.SetActive(false);
            GameManager.Instance.NextLevel();
        }
    }
}
