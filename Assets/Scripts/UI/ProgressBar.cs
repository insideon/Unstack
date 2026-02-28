using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unstack.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI remainingText;

        private int _totalShapes;

        public void SetTotal(int total)
        {
            _totalShapes = total;
            UpdateDisplay(total);
        }

        public void UpdateRemaining(int remaining)
        {
            UpdateDisplay(remaining);
        }

        private void UpdateDisplay(int remaining)
        {
            if (fillImage != null && _totalShapes > 0)
            {
                float cleared = _totalShapes - remaining;
                fillImage.fillAmount = cleared / _totalShapes;
            }

            if (remainingText != null)
                remainingText.text = remaining > 0 ? $"{remaining} left" : "";
        }
    }
}
